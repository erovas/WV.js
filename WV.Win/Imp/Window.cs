using WV.Enums;
using WV.Win.Win32;
using WV.Interfaces;
using System.Drawing;
using WV.Win.Win32.Enums;
using WV.Win.Win32.Structs;
using static WV.AppManager;
using System.Runtime.InteropServices;
using Microsoft.Web.WebView2.Core;

namespace WV.Win.Imp
{
    public class Window : Plugin, IWindow
    {
        private WebView WV { get; }
        private IntPtr Handle => this.WV.Handle;
        internal Rect InternalRect { get; }

        //-------------------------------------------//

        #region CS Events

        private event WVEventHandler<WindowState, string>? stateChangedEvent;
        public event WVEventHandler<WindowState, string>? StateChanged
        {
            add
            {
                Plugin.ThrowDispose(this.WV);
                stateChangedEvent += value;
            }
            remove
            {
                Plugin.ThrowDispose(this.WV);
                stateChangedEvent -= value;
            }
        }

        private event WVEventHandler? closeEvent;
        public event WVEventHandler? Closing
        {
            add
            {
                Plugin.ThrowDispose(this.WV);
                closeEvent += value;
            }
            remove
            {
                Plugin.ThrowDispose(this.WV);
                closeEvent -= value;
            }
        }

        private event WVEventHandler<int, int>? positionChangedEvent;
        public event WVEventHandler<int, int>? PositionChanged
        {
            add
            {
                Plugin.ThrowDispose(this.WV);
                positionChangedEvent += value;
            }
            remove
            {
                Plugin.ThrowDispose(this.WV);
                positionChangedEvent -= value;
            }
        }

        private event WVEventHandler<bool>? activatedEvent;
        public event WVEventHandler<bool>? Activated
        {
            add
            {
                Plugin.ThrowDispose(this.WV);
                activatedEvent += value;
            }
            remove
            {
                Plugin.ThrowDispose(this.WV);
                activatedEvent -= value;
            }
        }

        private event WVEventHandler<bool>? enabledEvent;
        public event WVEventHandler<bool>? EnabledEvent
        {
            add
            {
                Plugin.ThrowDispose(this.WV);
                enabledEvent += value;
            }
            remove
            {
                Plugin.ThrowDispose(this.WV);
                enabledEvent -= value;
            }
        }

        private event WVEventHandler<bool>? visibleEvent;
        public event WVEventHandler<bool>? Visible
        {
            add
            {
                Plugin.ThrowDispose(this.WV);
                visibleEvent += value;
            }
            remove
            {
                Plugin.ThrowDispose(this.WV);
                visibleEvent -= value;
            }
        }

        private event WVEventHandler<int, int>? sizeChangedEvent;
        public event WVEventHandler<int, int>? SizeChanged
        {
            add
            {
                Plugin.ThrowDispose(this.WV);
                sizeChangedEvent += value;
            }
            remove
            {
                Plugin.ThrowDispose(this.WV);
                sizeChangedEvent -= value;
            }
        }

        public event WVSysEventHandler? raw;
        public event WVSysEventHandler? Raw
        {
            add 
            {
                Plugin.ThrowDispose(this.WV);
                raw += value;
            }
            remove 
            {
                Plugin.ThrowDispose(this.WV);
                raw -= value;
            }

        }

        public Window(WebView wv) : base(wv)
        {
            this.WV = wv;
            this.InternalRect = new Rect(wv);
        }

        public IRect Rect
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.InternalRect;
            }
        }

        #endregion

        //-------------------------------------------//

        #region FLAGS

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento State
        /// </summary>
        internal bool PreventStateEvent { get; set; }

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento Close
        /// </summary>
        //internal bool PreventCloseEvent { get; set; }

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento Position
        /// </summary>
        internal bool PreventPositionEvent { get; set; }

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento Activate
        /// </summary>
        internal bool PreventActivateEvent { get; set; }

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento Enable
        /// </summary>
        internal bool PreventEnableEvent { get; set; }

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento Visible
        /// </summary>
        internal bool PreventVisibleEvent { get; set; }

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento Size
        /// </summary>
        internal bool PreventSizeEvent { get; set; }

        #endregion

        //-------------------------------------------//

        #region PROPS

        #region State

        private WindowState _State { get; set; } = WindowState.None;
        internal WindowState LastState { get; set; } = WindowState.None;
        private bool StateChangeInternal { get; set; } = false;

        public WindowState State
        {
            get
            {
                Plugin.ThrowDispose(this.WV);

                if (this._State == WindowState.None)
                    this._State = Helpers.GetCurrentState(this.Handle);

                return this._State;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (!this.IsVisible)
                    return;

                if (this.State == value)
                    return;

                switch (value)
                {
                    case WindowState.Minimized:
                        this.Maximize();
                        break;
                    case WindowState.Normalized:
                        this.Normalize();
                        break;
                    case WindowState.Maximized:
                        this.Maximize();
                        break;
                }
            }
        }

        public string StateText
        {
            get => this.State.ToString();
            set
            {
                if (Enum.TryParse(value, out WindowState myStates))
                    this.State = myStates;
            }
        }

        internal void UpdateStateFromSystem(WindowState currentState)
        {
            // Es un cambio de State controlado, no hacer nada
            if (this.StateChangeInternal)
                return;

            // La ventana es invisible, es un falso cambio de estado,
            // provocado por redimensionar la ventana mediante HWnd o Rect
            if (!this.IsVisible)
                return;

            //State currentState = this.State;

            if (this.State == currentState)
                return;

            this._State = currentState;

            FireStateChangedEvent(currentState, currentState.ToString());
        }

        #endregion

        #region Title

        private string _Title = string.Empty;
        public string Title
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return _Title;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (_Title == value)
                    return;

                if (!User32.SetWindowText(this.Handle, value))
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

                _Title = value;
            }
        }

        #endregion

        #region TopMost

        private bool _TopMost = false;
        public bool TopMost
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return _TopMost;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (value == _TopMost)
                    return;

                Utils32.WindowTopMost(this.Handle, value);
                _TopMost = value;
            }
        }



        #endregion

        #region Enabled

        private bool _Enabled = true;
        public bool Enabled
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return _Enabled;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (value == _Enabled)
                    return;

                User32.EnableWindow(this.Handle, value);
                _Enabled = value;
            }
        }

        #endregion

        #region Visible

        private bool _IsVisible = false;
        public bool IsVisible
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                //return User32.IsWindowVisible(this.Handle);
                return _IsVisible;
            }
            private set => _IsVisible = value;
        }

        #endregion

        #region PreventClose

        private bool _PreventClose;
        public bool PreventClose
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return _PreventClose;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                _PreventClose = value;
            }
        }

        #endregion

        #region IsActive

        public bool IsActive
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return User32.GetForegroundWindow() == this.Handle;
            }
        }

        #endregion

        public bool AllowSnap
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return Utils32.HasWindowStyle(this.Handle, WinStyles.WS_MAXIMIZEBOX);
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                Utils32.SetWinStyle(this.Handle, WinStyles.WS_MAXIMIZEBOX, value);
            }
        }

        #region ClickThrough

        private bool _ClickThrough;

        public bool ClickThrough
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return _ClickThrough;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (value == this.ClickThrough)
                    return;

                int exStyle = User32.GetWindowLong(this.Handle, (int)GetWinLong.GWL_EXSTYLE);

                if (value)
                    exStyle |= (int)WinStylesEx.WS_EX_TRANSPARENT;
                else
                    exStyle &= ~(int)WinStylesEx.WS_EX_TRANSPARENT;

                User32.SetWindowLong(this.Handle, (int)GetWinLong.GWL_EXSTYLE, exStyle);
                _ClickThrough = value;
            }
        }

        #endregion

        #endregion

        //-------------------------------------------//

        #region METHODS

        public void ToCenter()
        {
            Plugin.ThrowDispose(this.WV);

            if (this.State != WindowState.Normalized && this.State != WindowState.None)
                return;

            RECT workArea = Utils32.GetWorkArea(this.Handle);
            RECT wvSize = Imp.Rect.GetRECT(this.WV);

            int X = Math.Max(workArea.X, workArea.X + (workArea.Width - wvSize.Width) / 2);
            int Y = Math.Max(workArea.Y, workArea.Y + (workArea.Height - wvSize.Height) / 2);

            User32.MoveWindow(this.Handle, X, Y, wvSize.Width, wvSize.Height, true);
        }

        public void Close()
        {
            Plugin.ThrowDispose(this.WV);
            User32.SendMessage(this.Handle, (uint)WinMsg.WM_CLOSE, 0, 0);
        }

        public void ShowBehind()
        {
            Plugin.ThrowDispose(this.WV);

            if (this.IsVisible)
                return;

            this.PreventStateEvent = true;
            this.PreventPositionEvent = true;
            User32.ShowWindow(this.Handle, ShowWinCmd.SW_SHOWNA);
            this.PreventStateEvent = false;
            this.PreventPositionEvent = false;
        }

        public void Show()
        {
            Plugin.ThrowDispose(this.WV);

            if (this.IsVisible)
                return;

            this.PreventStateEvent = true;
            this.PreventPositionEvent = true;
            User32.ShowWindow(this.Handle, ShowWinCmd.SW_SHOW);
            this.PreventStateEvent = false;
            this.PreventPositionEvent = false;
        }

        public void Hide()
        {
            Plugin.ThrowDispose(this.WV);

            if (!this.IsVisible)
                return;

            User32.ShowWindow(this.Handle, ShowWinCmd.SW_HIDE);
        }

        public void Drag()
        {
            Plugin.ThrowDispose(this.WV);
            this.WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTCAPTION, 0);
            }, null);
        }

        #region RESIZE

        public void ResizeTopLeft()
        {
            Plugin.ThrowDispose(this.WV);
            this.WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTTOPLEFT, 0);
            }, null);
        }

        public void ResizeTopRight()
        {
            Plugin.ThrowDispose(WV);
            WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTTOPRIGHT, 0);
            }, null);
        }

        public void ResizeBottomLeft()
        {
            Plugin.ThrowDispose(WV);
            WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTBOTTOMLEFT, 0);
            }, null);
        }

        public void ResizeBottomRight()
        {
            Plugin.ThrowDispose(this.WV);
            this.WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTBOTTOMRIGHT, 0);
            }, null);
        }

        public void ResizeLeft()
        {
            Plugin.ThrowDispose(this.WV);
            this.WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTLEFT, 0);
            }, null);
        }

        public void ResizeRight()
        {
            Plugin.ThrowDispose(this.WV);
            this.WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTRIGHT, 0);
            }, null);
        }

        public void ResizeTop()
        {
            Plugin.ThrowDispose(this.WV);
            this.WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTTOP, 0);
            }, null);
        }

        public void ResizeBottom()
        {
            Plugin.ThrowDispose(this.WV);
            this.WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTBOTTOM, 0);
            }, null);
        }

        #endregion

        public void Minimize()
        {
            this.ChangeState(WindowState.Minimized);
        }

        public void Normalize()
        {
            this.ChangeState(WindowState.Normalized);
        }

        public void Maximize()
        {
            if (this.State == WindowState.Minimized)
                this.PreventPositionEvent = true;

            this.ChangeState(WindowState.Maximized);
            this.PreventPositionEvent = false;
        }

        public void Restore()
        {
            Plugin.ThrowDispose(this.WV);

            switch (this.State)
            {
                case WindowState.Minimized:
                    if (this.LastState == WindowState.Normalized)
                        User32.ShowWindow(this.Handle, ShowWinCmd.SW_RESTORE);
                    else
                        this.Maximize();
                    break;

                case WindowState.Normalized:
                    // Hacer nada
                    break;

                case WindowState.Maximized:
                    this.Normalize();
                    break;
            }
        }

        #endregion

        //-------------------------------------------//

        #region EVENTS

        #region OnStateChanged

        private IJSFunction? OnStateChangedFN { get; set; }

        public object? OnStateChanged
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return OnStateChangedFN?.Raw;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (value == OnStateChangedFN?.Raw)
                    return;

                this.OnStateChangedFN?.Dispose();
                this.OnStateChangedFN = null;

                if (value == null)
                    return;

                this.OnStateChangedFN = IJSFunction.Create(value);
            }
        }

        internal void FireStateChangedEvent(WindowState value, string text)
        {
            if (this.PreventStateEvent)
                return;

            this.OnStateChangedFN?.Execute(value, text);
            this.stateChangedEvent?.Invoke(this.WV, value, text);
        }

        #endregion

        #region OnClose

        private IJSFunction? OnCloseFN { get; set; }

        public object? OnClose
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return OnCloseFN?.Raw; ;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (value == OnCloseFN?.Raw)
                    return;

                this.OnCloseFN?.Dispose();
                this.OnCloseFN = null;

                if (value == null)
                    return;

                this.OnCloseFN = IJSFunction.Create(value);
            }
        }

        internal void FireCloseEvent()
        {
            this.OnCloseFN?.Execute();
            this.closeEvent?.Invoke(this.WV);
        }

        #endregion

        #region OnPositionChanged

        private IJSFunction? OnPositionChangedFN { get; set; }

        public object? OnPositionChanged
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.OnPositionChangedFN?.Raw;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (value == OnPositionChangedFN?.Raw)
                    return;

                this.OnPositionChangedFN?.Dispose();
                this.OnPositionChangedFN = null;

                if (value == null)
                    return;

                this.OnPositionChangedFN = IJSFunction.Create(value);
            }
        }

        internal void FirePositionChangedEvent(int x, int y)
        {
            if (this.PreventPositionEvent)
                return;

            this.OnPositionChangedFN?.Execute(x, y);
            this.positionChangedEvent?.Invoke(this.WV, x, y);
        }

        #endregion

        #region OnActivated

        private IJSFunction? OnActivatedFN { get; set; }

        public object? OnActivated
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.OnActivatedFN?.Raw;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (value == OnActivatedFN?.Raw)
                    return;

                this.OnActivatedFN?.Dispose();
                this.OnActivatedFN = null;

                if (value == null)
                    return;

                this.OnActivatedFN = IJSFunction.Create(value);
            }
        }

        internal void FireActivatedEvent(bool active)
        {
            if (this.PreventActivateEvent)
                return;

            this.OnActivatedFN?.Execute(active);
            this.activatedEvent?.Invoke(this.WV, active);
        }

        #endregion

        #region OnEnabled

        private IJSFunction? OnEnabledFN { get; set; }

        public object? OnEnabled
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return OnEnabledFN?.Raw;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (value == OnEnabledFN?.Raw)
                    return;

                this.OnEnabledFN?.Dispose();
                this.OnEnabledFN = null;

                if (value == null)
                    return;

                this.OnEnabledFN = IJSFunction.Create(value);
            }
        }

        internal void FireEnabledEvent(bool enabled)
        {
            if (this.PreventEnableEvent)
                return;

            this.OnEnabledFN?.Execute(enabled);
            this.enabledEvent?.Invoke(this.WV, enabled);
        }

        #endregion

        #region OnVisible

        private IJSFunction? OnVisibleFN { get; set; }

        public object? OnVisible
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return OnVisibleFN?.Raw;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (value == OnVisibleFN?.Raw)
                    return;

                this.OnVisibleFN?.Dispose();
                this.OnVisibleFN = null;

                if (value == null)
                    return;

                this.OnVisibleFN = IJSFunction.Create(value);
            }
        }

        internal void FireVisibleEvent(bool visible)
        {
            this.IsVisible = visible;

            //if(this.WV.WVController != null)
            //    this.WV.WVController.IsVisible = visible;

            if (this.PreventVisibleEvent)
                return;

            this.OnVisibleFN?.Execute(visible);
            this.visibleEvent?.Invoke(this.WV, visible);
        }

        #endregion

        #region OnSizeChanged

        private IJSFunction? OnSizeChangedFN { get; set; }

        public object? OnSizeChanged
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return OnSizeChangedFN?.Raw;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (value == OnSizeChangedFN?.Raw)
                    return;

                this.OnSizeChangedFN?.Dispose();
                this.OnSizeChangedFN = null;

                if (value == null)
                    return;

                this.OnSizeChangedFN = IJSFunction.Create(value);
            }
        }

        internal void FireSizeChangedEvent(int width, int heigth)
        {
            if (this.PreventSizeEvent)
                return;

            this.OnSizeChangedFN?.Execute(width, heigth);
            this.sizeChangedEvent?.Invoke(this.WV, width, heigth);
        }

        #endregion

        #endregion

        private void ChangeState(WindowState value)
        {
            Plugin.ThrowDispose(this.WV);

            // La ventana DEBE estar visible
            if (!this.IsVisible)
                return;

            // Evitar hacer dos cambios de State al mismo tiempo
            if (this.StateChangeInternal)
                return;

            // Tiene que ser un State distinto
            if (this.State == value)
                return;

            this.StateChangeInternal = true;

            ShowWinCmd action = ShowWinCmd.SW_NORMAL;

            switch (value)
            {
                case WindowState.Minimized:
                    action = ShowWinCmd.SW_MINIMIZE;
                    break;
                case WindowState.Normalized:
                    action = ShowWinCmd.SW_NORMAL;
                    break;
                case WindowState.Maximized:
                    action = ShowWinCmd.SW_MAXIMIZE;
                    break;
            }

            try
            {
                this.LastState = this.State;
                this._State = value;
                User32.ShowWindow(this.Handle, action);
                FireStateChangedEvent(value, value.ToString());
            }
            finally
            {
                this.StateChangeInternal = false;
            }
        }


        internal void ClearEvents()
        {
            // Quita los eventos registrados con AddEventListener desde JS
            this.CleanJSEvents();

            this.stateChangedEvent = null;
            this.closeEvent = null;
            this.positionChangedEvent = null;
            this.activatedEvent = null;
            this.enabledEvent = null;
            this.visibleEvent = null;
            this.sizeChangedEvent = null;

            this.OnStateChangedFN = null;
            this.OnCloseFN = null;
            this.OnPositionChangedFN = null;
            this.OnActivatedFN = null;
            this.OnEnabledFN = null;
            this.OnVisibleFN = null;
            this.OnSizeChangedFN = null;
        }

        protected override void Dispose(bool disposing)
        {
            //throw new NotImplementedException();
        }

        public override void Dispose()
        {
            //throw new Exception("You can't do dispose");
        }

        internal void ToDefault()
        {
            if (!this.IsVisible)
            {
                this.PreventVisibleEvent = true;
                this.ShowBehind();
                this.PreventVisibleEvent = false;
            }

            this.PreventStateEvent = true;
            this.Normalize();
            this.PreventStateEvent = false;

            this.PreventVisibleEvent = true;
            this.Hide();
            this.PreventVisibleEvent = false;

            this.Title = string.Empty;
            this.TopMost = false;
            this.Enabled = true;
            this.PreventClose = false;
            this.ClickThrough = false;

            Rect rect = this.InternalRect;
            rect.X = 0;
            rect.Y = 0;
            rect.MinWidth = AppManager.MinWindowWidth;
            rect.MinHeight = AppManager.MinWindowHeight;
            rect.Width = rect.MinWidth;
            rect.Height = rect.MinHeight;
            rect.MaxWidth = AppManager.MaxWindowWidth;
            rect.MaxHeight = AppManager.MaxWindowHeight;
        }

        #region WNDPROC

        internal IntPtr WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            bool handled = false;
            this.raw?.Invoke(this.WV, new object[] { hWnd, uMsg, wParam, lParam }, ref handled);

            if(handled)
                return IntPtr.Zero;

            switch ((WinMsg)uMsg)
            {
                case WinMsg.WM_CLOSE:
                    //Si se evita cerrar la ventana se dispara el evento OnClose
                    if (this.PreventClose)
                        this.FireCloseEvent();
                    else
                        User32.DestroyWindow(hWnd);

                    return IntPtr.Zero;

                case WinMsg.WM_DESTROY:
                    this.WV.Dispose();

                    // Es el WebView original, se cierra todo
                    if (this.WV.IsMain)
                        User32.PostQuitMessage(0);

                    break;

                case WinMsg.WM_ACTIVATE:
                    this.FireActivatedEvent(wParam != IntPtr.Zero);
                    break;

                case WinMsg.WM_ENABLE:
                    this.FireEnabledEvent(wParam != IntPtr.Zero);
                    break;

                case WinMsg.WM_SHOWWINDOW:
                    this.FireVisibleEvent(wParam != IntPtr.Zero);
                    break;

                case WinMsg.WM_NCCALCSIZE:
                    // Para que el area de cliente cubra la barra de tareas superior
                    if (wParam != IntPtr.Zero)
                    {
                        NCCALCSIZE_PARAMS param = Marshal.PtrToStructure<NCCALCSIZE_PARAMS>(lParam);
                        //param.rgrc0.Top -= 0; // Reducir área no cliente a cero
                        Marshal.StructureToPtr(param, lParam, false);
                        return IntPtr.Zero;
                    }
                    break;

                case WinMsg.WM_SYSCOMMAND:
                    switch ((WM_SYSCOMMAND)wParam)
                    {
                        case WM_SYSCOMMAND.SC_MAXIMIZE:
                            this.Maximize();
                            return IntPtr.Zero;

                        case WM_SYSCOMMAND.SC_MINIMIZE:
                            this.Minimize();
                            return IntPtr.Zero;

                        case WM_SYSCOMMAND.SC_RESTORE:
                            this.Restore();
                            return IntPtr.Zero;
                    }
                    break;

                case WinMsg.WM_GETMINMAXINFO:
                    return HandleGetMinMaxInfo(hWnd, uMsg, wParam, lParam);

                case WinMsg.WM_SIZE:
                    return HandleSize(hWnd, uMsg, wParam, lParam);

                case WinMsg.WM_MOVE:
                    return HandleMove(hWnd, uMsg, wParam, lParam);

                default:

                    if (uMsg == UIThreadSyncCtx.WM_SYNCHRONIZATIONCONTEXT_WORK_AVAILABLE)
                        this.WV.WVUIContext?.RunAvailableWorkOnCurrentThread();

                    break;
            }

            return User32.DefWindowProcW(hWnd, uMsg, wParam, lParam);
        }

        private IntPtr HandleGetMinMaxInfo(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            RECT workArea = Utils32.GetWorkAreaByPosition(hWnd);
            MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

            int MinWidth = this.Rect.MinWidth;
            int MinHeight = this.Rect.MinHeight;

            int MaxWidth = this.Rect.MaxWidth;
            int MaxHeight = this.Rect.MaxHeight;

            int Width = (workArea.Width > MaxWidth ? MaxWidth : workArea.Width);
            int Height = (workArea.Height > MaxHeight ? MaxHeight : workArea.Height - 1); // Se quita 1 pixel, asi se "soluciona" error al maximizar ventana, que se desborda

            // Limitar Ancho y alto de la ventana cuando se maximiza
            mmi.ptMaxSize.x = Width;
            mmi.ptMaxSize.y = Height;
            mmi.ptMaxPosition.x = 0; // workArea.X;
            mmi.ptMaxPosition.y = 0; // workArea.Y;

            // Limitar Ancho y alto de la ventana cuando se redimensiona manualmente ó se hace Snapping
            mmi.ptMinTrackSize.x = MinWidth;
            mmi.ptMinTrackSize.y = MinHeight;
            mmi.ptMaxTrackSize.x = MaxWidth;
            mmi.ptMaxTrackSize.y = MaxHeight;

            Marshal.StructureToPtr(mmi, lParam, true);
            return User32.DefWindowProcW(hWnd, uMsg, wParam, lParam);
        }

        private IntPtr HandleSize(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            CoreWebView2Controller? coreWV = this.WV.WVController;

            if (coreWV == null)
                return User32.DefWindowProcW(hWnd, uMsg, wParam, lParam);

            // Si es un cambio de State controlado, se evita utilizar el Helpers
            WindowState currentState = this.StateChangeInternal ? this.State : Helpers.GetCurrentState(hWnd);

            // Actualizar el State, cuando el usuario iteracciona con la ventana (cambiar State o tamaño ventana NO controlados)
            this.UpdateStateFromSystem(currentState);

            // No redimensionar WebView cuando se minimiza o se esta minimizado
            if (currentState == WindowState.Minimized)
                return User32.DefWindowProcW(hWnd, uMsg, wParam, lParam);

            // Redimensionar control WebView2
            Rectangle currentRect = coreWV.Bounds;
            RECT workArea = Utils32.GetWorkArea(hWnd);

            // Ancho y Alto actual de la ventana (NO del WebView)
            int Width = Utils32.GetLowWord(lParam);
            int Height = Utils32.GetHighWord(lParam);

            // Compensar el pixel faltante de Altura de la ventana en el WebView
            Height = (Height == workArea.Height - 1 ? workArea.Height : Height);

            // Ajustar WebView a la ventana contenedora
            if (currentRect.Width != Width || currentRect.Height != Height)
            {
                coreWV.Bounds = new Rectangle(0, 0, Width, Height);
                this.FireSizeChangedEvent(Width, Height);
            }

            return IntPtr.Zero;
        }

        private IntPtr HandleMove(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            // La ventana esta Minimizada, cualquier movimiento estando minimizado no es valido
            if (this.State == WindowState.Minimized)
                return User32.DefWindowProcW(hWnd, uMsg, wParam, lParam);

            int posX = Utils32.GetLowWord(lParam);
            int posY = Utils32.GetHighWord(lParam);

            // Windows establece X y Y en -32000 cuando se va minimizar.
            if (posX == -32000 && posY == -32000)
                return User32.DefWindowProcW(hWnd, uMsg, wParam, lParam);

            this.FirePositionChangedEvent(posX, posY);

            return User32.DefWindowProcW(hWnd, uMsg, wParam, lParam);
        }

        #endregion

    }
}