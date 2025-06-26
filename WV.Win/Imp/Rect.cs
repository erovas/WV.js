using WV.Enums;
using WV.Win.Win32;
using WV.Interfaces;
using WV.Win.Win32.Structs;
using System.Runtime.InteropServices;

namespace WV.Win.Imp
{
    public class Rect : IRect
    {
        #region Helpers

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        internal static RECT GetRECT(WebView wv)
        {
            Window win = wv.InternalWindow;

            // La ventana está actualmente Minimizada
            if (win.State == WindowState.Minimized)
            {
                WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                placement.length = Marshal.SizeOf(placement);
                User32.GetWindowPlacement(wv.Handle, ref placement);

                // Estaba Normalizada antes de ser minimizada
                if (win.LastState == WindowState.Normalized)
                    return placement.rcNormalPosition;

                // Estaba Maximizada antes de ser minimizada
                //POINT ptMax = placement.ptMaxPosition;    // Siempre es X = 0 y Y = 0, No sirve
                //RECT normalRect = placement.rcNormalPosition;
                //POINT ptMax = new POINT
                //{
                //    x = (normalRect.Left + normalRect.Right) / 2,
                //    y = (normalRect.Top + normalRect.Bottom) / 2
                //};

                // Encontrar el monitor asociado a ptMaxPosition
                //IntPtr hMonitor = User32.MonitorFromPoint(ptMax, MONITOR_DEFAULTTONEAREST);
                IntPtr hMonitor = User32.MonitorFromWindow(wv.Handle, MONITOR_DEFAULTTONEAREST);
                MONITORINFO monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
                User32.GetMonitorInfo(hMonitor, ref monitorInfo);
                return monitorInfo.rcWork;
            }

            // La ventana esta Normalizada o Maximizada
            User32.GetWindowRect(wv.Handle, out RECT rect);

            if (wv.WVController != null) 
            {
                rect.Right = rect.X + wv.WVController.Bounds.Width;
                rect.Bottom = rect.Y + wv.WVController.Bounds.Height;
            }

            return rect;
        }

        private static void SetRECT(WebView wv, int X, int Y, int Width, int Height)
        {
            User32.MoveWindow(wv.Handle, X, Y, Width, Height, true);
        }

        #endregion

        private WebView WV { get; }
        private Window Win => this.WV.InternalWindow;

        public Rect(WebView wv)
        {
            WV = wv;
        }

        //-------------------------------------------//

        #region Position

        public int X
        {
            get 
            {
                Plugin.ThrowDispose(this.WV);
                return GetRECT(WV).X;
            } 
            set
            {
                Plugin.ThrowDispose(this.WV);

                if(this.Win.State == WindowState.Minimized)
                    return;

                RECT rect = GetRECT(this.WV);

                if (rect.X == value)
                    return;

                this.Win.PreventPositionEvent = true;
                SetRECT(WV, value, rect.Y, rect.Width, rect.Height);
                this.Win.PreventPositionEvent = false;
            }
        }

        public int Y
        {
            get 
            {
                Plugin.ThrowDispose(this.WV);
                return GetRECT(WV).Y;
            } 
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (this.Win.State == WindowState.Minimized)
                    return;

                RECT rect = GetRECT(this.WV);

                if (rect.Y == value)
                    return;

                this.Win.PreventPositionEvent = true;
                SetRECT(WV, rect.X, value, rect.Width, rect.Height);
                this.Win.PreventPositionEvent = false;
            }
        }

        #endregion

        //-------------------------------------------//

        #region Size

        public int Width
        {
            //get => GetRECT(WV).Width;
            get
            {
                Plugin.ThrowDispose(this.WV);
                return GetRECT(WV).Width;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (this.Win.State == WindowState.Minimized)
                    return;

                RECT rect = GetRECT(this.WV);

                if (rect.Width == value)
                    return;

                if (value > MaxWidth)
                    value = MaxWidth;
                else if (value < MinWidth)
                    value = MinWidth;

                this.Win.PreventSizeEvent = true;
                SetRECT(WV, rect.X, rect.Y, value, rect.Height);
                this.Win.PreventSizeEvent = false;
            }
        }

        public int Height
        {
            //get => GetRECT(WV).Height;
            get
            {
                Plugin.ThrowDispose(this.WV);              
                // La ventana que contiene el WebView es 1 pixel mas bajo cuando está maximizado
                return GetRECT(WV).Height;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (this.Win.State == WindowState.Minimized)
                    return;

                RECT rect = GetRECT(this.WV);

                if (rect.Width == value)
                    return;

                if (value > MaxHeight)
                    value = MaxHeight;
                else if (value < MinHeight)
                    value = MinHeight;

                this.Win.PreventSizeEvent = true;
                SetRECT(WV, rect.X, rect.Y, rect.Width, value);
                this.Win.PreventSizeEvent = false;
            }
        }

        #endregion

        //-------------------------------------------//

        #region Max Size

        private int _MaxWidth = AppManager.MaxWindowWidth;
        private int _MaxHeight = AppManager.MaxWindowHeight;
       
        public int MaxWidth
        {
            get 
            { 
                Plugin.ThrowDispose(this.WV); 
                return _MaxWidth; 
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (this.Win.State == WindowState.Minimized)
                    return;

                if (value < this.MinWidth)
                    value = this.MinWidth;

                _MaxWidth = value;

                if (this.MaxWidth < this.Width)
                    this.Width = this.MaxWidth;
            }
        }

        public int MaxHeight
        {
            get 
            { 
                Plugin.ThrowDispose(this.WV); 
                return _MaxHeight; 
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (this.Win.State == WindowState.Minimized)
                    return;

                if (value < this.MinHeight)
                    value = this.MinHeight;

                _MaxHeight = value;

                if (this.MaxHeight < this.Height)
                    this.Height = this.MaxHeight;
            }
        }

        #endregion

        //-------------------------------------------//

        #region Min Size

        private const int LowMinWidth = AppManager.MinWindowWidth;
        private const int LowMinHeight = AppManager.MinWindowHeight;

        private int _MinWidth = LowMinWidth;
        private int _MinHeight = LowMinHeight;
        public int MinWidth
        {
            get 
            { 
                Plugin.ThrowDispose(this.WV); 
                return _MinWidth; 
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (this.Win.State == WindowState.Minimized)
                    return;

                if (value < LowMinWidth)
                    value = LowMinWidth;

                _MinWidth = value;

                if (this.MinWidth > this.MaxWidth)
                    this.MaxWidth = this.MinWidth;

                if (this.MinWidth > this.Width)
                    this.Width = this.MinWidth;
            }
        }

        public int MinHeight
        {
            get 
            { 
                Plugin.ThrowDispose(this.WV); 
                return _MinHeight; 
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (this.Win.State == WindowState.Minimized)
                    return;

                if (value < LowMinHeight)
                    value = LowMinHeight;

                _MinHeight = value;

                if (this.MinHeight > this.MaxHeight)
                    this.MaxHeight = this.MinHeight;

                if (this.MinHeight > this.Height)
                    this.Height = this.MinHeight;
            }
        }

        #endregion

        //-------------------------------------------//

        #region METHODS

        public void SetSize(int width, int height)
        {
            Plugin.ThrowDispose(this.WV);

            if (this.Win.State == WindowState.Minimized)
                    return;

            RECT rect = GetRECT(this.WV);

            if (rect.Width == width && rect.Height == height)
                return;

            if (width > this.MaxWidth)
                width = this.MaxWidth;
            else if (width < this.MinWidth)
                width = this.MinWidth;

            if (height > this.MaxHeight)
                height = this.MaxHeight;
            else if (height < this.MinHeight)
                height = this.MinHeight;

            this.Win.PreventSizeEvent = true;
            SetRECT(WV, rect.X, rect.Y, width, height);
            this.Win.PreventSizeEvent = false;
        }

        public int[] GetSize()
        {
            Plugin.ThrowDispose(this.WV);
            RECT rect = GetRECT(this.WV);
            return [rect.Width, rect.Height];
        }

        public void SetPosition(int x, int y)
        {
            Plugin.ThrowDispose(this.WV);

            if (this.Win.State == WindowState.Minimized)
                return;

            RECT rect = GetRECT(this.WV);

            if (rect.X == x && rect.Y == y)
                return;

            this.Win.PreventPositionEvent = true;
            SetRECT(this.WV, x, y, rect.Width, rect.Height);
            this.Win.PreventPositionEvent = false;
        }

        public int[] GetPosition()
        {
            Plugin.ThrowDispose(this.WV);
            RECT rect = GetRECT(this.WV);
            return [rect.X, rect.Y];
        }

        public void SetPositionAndSize(int x, int y, int width, int height)
        {
            Plugin.ThrowDispose(this.WV);

            if (width > this.MaxWidth)
                width = this.MaxWidth;
            else if (width < this.MinWidth)
                width = this.MinWidth;

            if (height > this.MaxHeight)
                height = this.MaxHeight;
            else if (height < this.MinHeight)
                height = this.MinHeight;

            this.Win.PreventSizeEvent = true;
            this.Win.PreventPositionEvent = true;
            SetRECT(this.WV, x, y, width, height);
            this.Win.PreventSizeEvent = false;
            this.Win.PreventPositionEvent = false;
        }

        public int[] GetPositionAndSize()
        {
            Plugin.ThrowDispose(this.WV);
            RECT rect = GetRECT(this.WV);
            return [rect.X, rect.Y, rect.Width, rect.Height];
        }

        #endregion

    }
}