using WV.Enums;
using WV.Win.Win32;
using WV.Interfaces;
using System.Drawing;
using WV.Win.Win32.Enums;
using WV.Win.Win32.Structs;
using static WV.AppManager;
using Microsoft.Web.WebView2.Core;
using System.Runtime.InteropServices;

namespace WV.Win.Imp
{
    public class Window : Plugin, IWindow
    {
        private WebView WV => (WebView)this.WebView;
        private IntPtr Handle => this.WV.Handle;
        internal Rect InternalRect { get; }

        //=======================================//

        #region Events

        public event WVEventHandler<WindowState, string>? StateChanged;
        
        public event WVEventHandler? Closing;

        public event WVEventHandler<int, int>? PositionChanged;

        public event WVEventHandler<bool>? Activated;

        public event WVEventHandler<bool>? EnabledEvent;

        public event WVEventHandler<bool>? Visible;

        public event WVEventHandler<int, int>? SizeChanged;

        public event WVSysEventHandler? Raw;

        #endregion

        //=======================================//

        #region Fields

        // State
        private WindowState _State { get; set; } = WindowState.None;
        internal WindowState LastState { get; set; } = WindowState.None;
        private bool StateChangeInternal { get; set; } = false;


        private string _Title = string.Empty;
        private bool _TopMost = false;
        private bool _Enabled = true;
        private bool _IsVisible = false;
        private bool _PreventClose;
        private bool _ClickThrough;

        #endregion

        public Window(IContext ctx) : base(ctx)
        {
            this.InternalRect = new Rect(this.WV);
        }

        //=======================================//

        #region Properties

        #region Flags

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

        public IRect Rect
        {
            get
            {
                ThrowIfDisposed();
                return this.InternalRect;
            }
        }

        public WindowState State
        {
            get
            {
                ThrowIfDisposed();

                if (this._State == WindowState.None)
                    this._State = Helpers.GetCurrentState(this.Handle);

                return this._State;
            }
            set
            {
                ThrowIfDisposed();

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

        public string Title
        {
            get
            {
                ThrowIfDisposed();
                return _Title;
            }
            set
            {
                ThrowIfDisposed();

                if (_Title == value)
                    return;

                if (!User32.SetWindowText(this.Handle, value))
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

                _Title = value;
            }
        }

        public bool TopMost
        {
            get
            {
                ThrowIfDisposed();
                return _TopMost;
            }
            set
            {
                ThrowIfDisposed();

                if (value == _TopMost)
                    return;

                Utils32.WindowTopMost(this.Handle, value);
                _TopMost = value;
            }
        }

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return _Enabled;
            }
            set
            {
                ThrowIfDisposed();

                if (value == _Enabled)
                    return;

                User32.EnableWindow(this.Handle, value);
                _Enabled = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                ThrowIfDisposed();
                return _IsVisible;
            }
            private set => _IsVisible = value;
        }

        public bool PreventClose
        {
            get
            {
                ThrowIfDisposed();
                return _PreventClose;
            }
            set
            {
                ThrowIfDisposed();
                _PreventClose = value;
            }
        }

        public bool IsActive
        {
            get
            {
                ThrowIfDisposed();
                return User32.GetForegroundWindow() == this.Handle;
            }
        }

        public bool AllowSnap
        {
            get
            {
                ThrowIfDisposed();
                return Utils32.HasWindowStyle(this.Handle, WinStyles.WS_MAXIMIZEBOX);
            }
            set
            {
                ThrowIfDisposed();
                Utils32.SetWinStyle(this.Handle, WinStyles.WS_MAXIMIZEBOX, value);
            }
        }

        public bool ClickThrough
        {
            get
            {
                ThrowIfDisposed();
                return _ClickThrough;
            }
            set
            {
                ThrowIfDisposed();

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

        //=======================================//

        #region Methods

        public void ToCenter()
        {
            ThrowIfDisposed();

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
            ThrowIfDisposed();
            User32.SendMessage(this.Handle, (uint)WinMsg.WM_CLOSE, 0, 0);
        }

        public void ShowBehind()
        {
            ThrowIfDisposed();

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
            ThrowIfDisposed();

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
            ThrowIfDisposed();

            if (!this.IsVisible)
                return;

            User32.ShowWindow(this.Handle, ShowWinCmd.SW_HIDE);
        }

        public void Drag()
        {
            ThrowIfDisposed();
            this.WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTCAPTION, 0);
            }, null);
        }

        #region RESIZE

        public void ResizeTopLeft()
        {
            ThrowIfDisposed();
            this.WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTTOPLEFT, 0);
            }, null);
        }

        public void ResizeTopRight()
        {
            ThrowIfDisposed();
            WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTTOPRIGHT, 0);
            }, null);
        }

        public void ResizeBottomLeft()
        {
            ThrowIfDisposed();
            WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTBOTTOMLEFT, 0);
            }, null);
        }

        public void ResizeBottomRight()
        {
            ThrowIfDisposed();
            this.WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTBOTTOMRIGHT, 0);
            }, null);
        }

        public void ResizeLeft()
        {
            ThrowIfDisposed();
            this.WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTLEFT, 0);
            }, null);
        }

        public void ResizeRight()
        {
            ThrowIfDisposed();
            this.WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTRIGHT, 0);
            }, null);
        }

        public void ResizeTop()
        {
            ThrowIfDisposed();
            this.WV.WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTTOP, 0);
            }, null);
        }

        public void ResizeBottom()
        {
            ThrowIfDisposed();
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
            ThrowIfDisposed();

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

        //=======================================//

        #region Protected Methods

        protected override void ThrowIfDisposed()
        {
            base.ThrowIfDisposed();
            Plugin.ThrowIfDisposed(this.WV);
        }

        #endregion

        //=======================================//

        #region Internal Methods

        internal void FireStateChangedEvent(WindowState value, string text)
        {
            if (this.Disposed)
                return;

            if (this.PreventStateEvent)
                return;

            this.StateChanged?.Invoke(this.WV, value, text);
        }

        internal void FireCloseEvent()
        {
            if(this.Disposed)
                return;

            this.Closing?.Invoke(this.WV);
        }

        internal void FirePositionChangedEvent(int x, int y)
        {
            if (this.Disposed)
                return;

            if (this.PreventPositionEvent)
                return;

            this.PositionChanged?.Invoke(this.WV, x, y);
        }

        internal void FireActivatedEvent(bool active)
        {
            if (this.Disposed)
                return;

            if (this.PreventActivateEvent)
                return;

            this.Activated?.Invoke(this.WV, active);
        }

        internal void FireEnabledEvent(bool enabled)
        {
            if (this.Disposed)
                return;

            if (this.PreventEnableEvent)
                return;

            this.EnabledEvent?.Invoke(this.WV, enabled);
        }

        internal void FireVisibleEvent(bool visible)
        {
            if (this.Disposed)
                return;

            this.IsVisible = visible;

            if (this.PreventVisibleEvent)
                return;

            this.Visible?.Invoke(this.WV, visible);
        }

        internal void FireSizeChangedEvent(int width, int heigth)
        {
            if (this.Disposed)
                return;

            if (this.PreventSizeEvent)
                return;

            this.SizeChanged?.Invoke(this.WV, width, heigth);
        }

        internal void ClearAllEvents()
        {
            // Quita los eventos registrados con AddEventListener desde JS
            this.ClearListeners();
            this.ClearEvents();
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

        #endregion

        //=======================================//

        #region Private Methods

        private void ChangeState(WindowState value)
        {
            ThrowIfDisposed();

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

        #endregion

        //=======================================//

        #region WNDPROC

        internal IntPtr WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            bool handled = false;
            this.Raw?.Invoke(this.WV, new object[] { hWnd, uMsg, wParam, lParam }, ref handled);

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

    }
}