using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Threading;
using WV.WebView;
using WV.WebView.Enums;
using WV.Windows.Utils;

namespace WV.Windows.Webview
{
    public class TaskBar : ITaskBar
    {
        #region PRIVATE

        private const int MaxValue = 100;
        private const int MinValue = 0;

        private ITaskbarList4 InnerTaskbarList { get; }
        private WebView InnerWV { get; }
        private TaskBarStatus InnerStatus { get; set; }
        private int InnerProgress { get; set; }
        private WindowInteropHelper InnerWinInterop { get; }
        private IntPtr InnerHandle => this.InnerWinInterop.Handle;

        #endregion

        #region INTERFACE

        public bool Visible 
        {
            get => InnerWV.ShowInTaskbar;
            set
            {
                if (InnerWV.ShowInTaskbar == value)
                    return;

                InnerWV.ShowInTaskbar = value;

                if (value)
                    this.Status = this.Status;
            }
        }

        public TaskBarStatus Status 
        {
            get => InnerStatus;
            set
            {
                if(InnerStatus == TaskBarStatus.Indeterminate && value == TaskBarStatus.Normal)
                {
                    InnerWV.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        InnerTaskbarList.SetProgressState(this.InnerHandle, eeTaskBarStatus.None);
                    }));
                }

                InnerStatus = value;

                InnerWV.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    InnerTaskbarList.SetProgressState(this.InnerHandle, (eeTaskBarStatus)Enum.Parse(typeof(eeTaskBarStatus), InnerStatus.ToString(), true));
                    this.Progress = this.Progress;
                }));
            }
        }

        public int Progress 
        {
            get => InnerProgress;
            set
            {
                if (value < MinValue)
                    value = MinValue;
                else if (value > MaxValue)
                    value = MaxValue;

                InnerProgress = value;

                if (InnerStatus == TaskBarStatus.Indeterminate || InnerStatus == TaskBarStatus.None)
                    return;

                InnerWV.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    InnerTaskbarList.SetProgressValue(
                        InnerHandle,
                        Convert.ToUInt64(value),
                        Convert.ToUInt64(MaxValue));
                }));
            }
        }

        public void Flash()
        {
            _ = User32.FlashWindow(this.InnerHandle, true);
        }

        #endregion

        public TaskBar(WebView wv)
        {
            if (!IsSupported())
                throw new Exception("Taskbar functions not available");

            InnerStatus = TaskBarStatus.None;
            InnerTaskbarList = (ITaskbarList4)new CTaskbarList();
            InnerTaskbarList.HrInit();
            InnerWV = wv;
            InnerWinInterop = new WindowInteropHelper(wv);
        }

        private static bool IsSupported()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version.CompareTo(new Version(6, 1)) >= 0;
        }

        #region BLACK MAGIC

        //https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nn-shobjidl_core-itaskbarlist3

        internal enum HResult
        {
            Ok = 0x0000

            // Add more constants here, if necessary
        }

        internal enum ThumbButtonMask
        {
            Bitmap = 0x1,
            Icon = 0x2,
            Tooltip = 0x4,
            THB_FLAGS = 0x8
        }

        [Flags]
        internal enum ThumbButtonOptions
        {
            Enabled = 0x00000000,
            Disabled = 0x00000001,
            DismissOnClick = 0x00000002,
            NoBackground = 0x00000004,
            Hidden = 0x00000008,
            NonInteractive = 0x00000010
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct ThumbButton
        {
            ///
            /// WPARAM value for a THUMBBUTTON being clicked.
            ///
            internal const int Clicked = 0x1800;

            [MarshalAs(UnmanagedType.U4)]
            internal ThumbButtonMask Mask;
            internal uint Id;
            internal uint Bitmap;
            internal IntPtr Icon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string Tip;
            [MarshalAs(UnmanagedType.U4)]
            internal ThumbButtonOptions Flags;
        }

        internal enum SetTabPropertiesOption
        {
            None = 0x0,
            UseAppThumbnailAlways = 0x1,
            UseAppThumbnailWhenActive = 0x2,
            UseAppPeekAlways = 0x4,
            UseAppPeekWhenActive = 0x8
        }

        internal enum eeTaskBarStatus
        {
            None = 0,
            Indeterminate = 0x1,
            Normal = 0x2,
            Error = 0x4,
            Paused = 0x8
        }

        // using System.Runtime.InteropServices
        [ComImportAttribute()]
        [GuidAttribute("c43dc798-95d1-4bea-9030-bb99e2983a1a")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface ITaskbarList4
        {
            // ITaskbarList
            [PreserveSig]
            void HrInit();
            [PreserveSig]
            void AddTab(IntPtr hwnd);
            [PreserveSig]
            void DeleteTab(IntPtr hwnd);
            [PreserveSig]
            void ActivateTab(IntPtr hwnd);
            [PreserveSig]
            void SetActiveAlt(IntPtr hwnd);

            // ITaskbarList2
            [PreserveSig]
            void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

            // ITaskbarList3
            [PreserveSig]
            void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
            [PreserveSig]
            void SetProgressState(IntPtr hwnd, eeTaskBarStatus tbpFlags);
            [PreserveSig]
            void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
            [PreserveSig]
            void UnregisterTab(IntPtr hwndTab);
            [PreserveSig]
            void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
            [PreserveSig]
            void SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, uint dwReserved);
            [PreserveSig]
            HResult ThumbBarAddButtons(
                IntPtr hwnd,
                uint cButtons,
                [MarshalAs(UnmanagedType.LPArray)] ThumbButton[] pButtons);
            [PreserveSig]
            HResult ThumbBarUpdateButtons(
                IntPtr hwnd,
                uint cButtons,
                [MarshalAs(UnmanagedType.LPArray)] ThumbButton[] pButtons);
            [PreserveSig]
            void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
            [PreserveSig]
            void SetOverlayIcon(
              IntPtr hwnd,
              IntPtr hIcon,
              [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
            [PreserveSig]
            void SetThumbnailTooltip(
                IntPtr hwnd,
                [MarshalAs(UnmanagedType.LPWStr)] string pszTip);
            [PreserveSig]
            void SetThumbnailClip(
                IntPtr hwnd,
                IntPtr prcClip);

            // ITaskbarList4
            void SetTabProperties(IntPtr hwndTab, SetTabPropertiesOption stpFlags);
        }

        [GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")]
        [ClassInterfaceAttribute(ClassInterfaceType.None)]
        [ComImportAttribute()]
        internal class CTaskbarList { }

        #endregion
    }
}
