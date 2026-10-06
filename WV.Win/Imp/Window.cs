using WV.Enums;
using WV.Win.Win32;
using WV.Interfaces;
using System.Drawing;
using WV.Win.Win32.Enums;
using WV.Win.Win32.Structs;
using Microsoft.Web.WebView2.Core;
using System.Runtime.InteropServices;
using WV.Core.Windowing;
using WV.Configs;

namespace WV.Win.Imp
{
    public sealed class Window : WindowCore, IWindow
    {
        private WebView WV => (WebView)this.WebView;
        internal IntPtr Handle { get; private set; }
        internal UIThreadSyncCtx? WVUIContext { get; private set; }
        internal Rect InternalRect => (Rect)_Rect!;

        //=======================================//

        #region Fields

        // State
        private WindowState _State = WindowState.None;
        internal WindowState _LastState = WindowState.None;
        private bool _StateChange = false;


        private string _Title = string.Empty;
        private bool _TopMost = false;
        private bool _Enabled = true;
        private bool _IsVisible = false;
        private bool _PreventClose;
        private bool _ClickThrough;
        private readonly WindowConfig _Config;
        private readonly bool _IsMain;

        #endregion

        public Window(IPluginContext ctx, WindowConfig windowConfig) : base(ctx, windowConfig)
        {
            _Config = windowConfig;
            _IsMain = WV.InternalIsMain;
        }

        protected override IRect CreateRect(IPluginContext context, RectConfig rectConfig)
        {
            return new Rect(Utils.CreateContext(WebView, Logger, nameof(Rect), Logger.Source), rectConfig, this);
        }


        protected override void Initialize(IPluginContext context, WindowConfig windowConfig)
        {
            Handle = Utils32.CreateMainWindows(WV, windowConfig.Title);

            WVUIContext = new UIThreadSyncCtx(Handle);
            SynchronizationContext.SetSynchronizationContext(WVUIContext);
        }

        //=======================================//

        #region Flags

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento State
        /// </summary>
        internal bool _PreventStateEvent;

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento Close
        /// </summary>
        //internal bool PreventCloseEvent { get; set; }

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento Position
        /// </summary>
        internal bool _PreventPositionEvent;

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento Activate
        /// </summary>
        internal bool _PreventActivateEvent;

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento Enable
        /// </summary>
        internal bool _PreventEnableEvent;

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento Visible
        /// </summary>
        internal bool _PreventVisibleEvent;

        /// <summary>
        /// Para indicar que no se quiere prevenir lanzar evento Size
        /// </summary>
        internal bool _PreventSizeEvent;

        #endregion

        #region Properties

        protected override WindowState StateCore
        {
            get
            {
                if (_State == WindowState.None)
                    _State = Utils32.GetCurrentState(this.Handle);

                return _State;
            }
            set
            {
                if (!this.IsVisibleCore)
                    return;

                if (this.StateCore == value)
                    return;

                switch (value)
                {
                    case WindowState.Minimized:
                        this.MinimizeCore();
                        break;
                    case WindowState.Normalized:
                        this.NormalizeCore();
                        break;
                    case WindowState.Maximized:
                        this.MaximizeCore();
                        break;
                }
            }
        }

        protected override string StateTextCore
        {
            get => this.StateCore.ToString();
            set
            {
                if (Enum.TryParse(value, out WindowState myStates))
                    this.StateCore = myStates;
            }
        }

        protected override string TitleCore
        {
            get
            {
                return _Title;
            }
            set
            {
                if (_Title == value)
                    return;

                if (!User32.SetWindowText(this.Handle, value))
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

                _Title = value;
            }
        }

        protected override bool TopMostCore
        {
            get
            {
                return _TopMost;
            }
            set
            {
                if (value == _TopMost)
                    return;

                Utils32.WindowTopMost(this.Handle, value);
                _TopMost = value;
            }
        }

        protected override bool EnabledCore
        {
            get
            {
                return _Enabled;
            }
            set
            {
                if (value == _Enabled)
                    return;

                User32.EnableWindow(this.Handle, value);
                _Enabled = value;
            }
        }

        protected override bool IsVisibleCore => _IsVisible;

        protected override bool PreventCloseCore
        {
            get => _PreventClose;
            set => _PreventClose = value;
        }

        protected override bool IsActiveCore => User32.GetForegroundWindow() == this.Handle;

        protected override bool AllowSnapCore
        {
            get => Utils32.HasWindowStyle(this.Handle, WinStyles.WS_MAXIMIZEBOX);
            set => Utils32.SetWinStyle(this.Handle, WinStyles.WS_MAXIMIZEBOX, value);
        }

        protected override bool ClickThroughCore
        {
            get
            {
                return _ClickThrough;
            }
            set
            {
                if (value == _ClickThrough)
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

        protected override void ToCenterCore()
        {
            if (this.StateCore != WindowState.Normalized && this.StateCore != WindowState.None)
                return;

            RECT workArea = Utils32.GetWorkArea(this.Handle);
            RECT wvSize = Utils32.GetRECT(this.WV);

            int X = Math.Max(workArea.X, workArea.X + (workArea.Width - wvSize.Width) / 2);
            int Y = Math.Max(workArea.Y, workArea.Y + (workArea.Height - wvSize.Height) / 2);

            User32.MoveWindow(this.Handle, X, Y, wvSize.Width, wvSize.Height, true);
        }

        protected override void CloseCore()
        {
            User32.SendMessage(this.Handle, (uint)WinMsg.WM_CLOSE, 0, 0);
        }

        protected override void ShowBehindCore()
        {
            if (this.IsVisibleCore)
                return;

            this._PreventStateEvent = true;
            this._PreventPositionEvent = true;
            User32.ShowWindow(this.Handle, ShowWinCmd.SW_SHOWNA);
            this._PreventStateEvent = false;
            this._PreventPositionEvent = false;
        }

        protected override void ShowCore()
        {
            if (this.IsVisibleCore)
                return;

            this._PreventStateEvent = true;
            this._PreventPositionEvent = true;
            User32.ShowWindow(this.Handle, ShowWinCmd.SW_SHOW);
            this._PreventStateEvent = false;
            this._PreventPositionEvent = false;
        }

        protected override void HideCore()
        {
            if (!this.IsVisibleCore)
                return;

            User32.ShowWindow(this.Handle, ShowWinCmd.SW_HIDE);
        }

        protected override void DragCore()
        {
            WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTCAPTION, 0);
            }, null);
        }

        #region RESIZE

        protected override void ResizeTopLeftCore()
        {
            WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTTOPLEFT, 0);
            }, null);
        }

        protected override void ResizeTopRightCore()
        {
            WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTTOPRIGHT, 0);
            }, null);
        }

        protected override void ResizeBottomLeftCore()
        {
            WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTBOTTOMLEFT, 0);
            }, null);
        }

        protected override void ResizeBottomRightCore()
        {
            WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTBOTTOMRIGHT, 0);
            }, null);
        }

        protected override void ResizeLeftCore()
        {
            WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTLEFT, 0);
            }, null);
        }

        protected override void ResizeRightCore()
        {
            WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTRIGHT, 0);
            }, null);
        }

        protected override void ResizeTopCore()
        {
            WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTTOP, 0);
            }, null);
        }

        protected override void ResizeBottomCore()
        {
            WVUIContext?.Post(x =>
            {
                User32.ReleaseCapture();
                _ = User32.SendMessage(this.Handle, (uint)WinMsg.WM_NCLBUTTONDOWN, (int)WM_NCHITTEST.HTBOTTOM, 0);
            }, null);
        }

        #endregion

        protected override void MinimizeCore()
        {
            this.ChangeState(WindowState.Minimized);
        }

        protected override void NormalizeCore()
        {
            this.ChangeState(WindowState.Normalized);
        }

        protected override void MaximizeCore()
        {
            if (this.StateCore == WindowState.Minimized)
                this._PreventPositionEvent = true;

            this.ChangeState(WindowState.Maximized);
            this._PreventPositionEvent = false;
        }

        protected override void RestoreCore()
        {
            switch (this.StateCore)
            {
                case WindowState.Minimized:
                    if (this._LastState == WindowState.Normalized)
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

        #region Internal Methods

        protected override void FireStateChangedEvent(WindowState value, string text)
        {
            if (this.Disposed)
                return;

            if (this._PreventStateEvent)
                return;

            base.FireStateChangedEvent(value, text);
        }

        protected override void FirePositionChangedEvent(int x, int y)
        {
            if (this.Disposed)
                return;

            if (this._PreventPositionEvent)
                return;

            base.FirePositionChangedEvent(x, y);
        }

        protected override void FireActivatedEvent(bool active)
        {
            if (this.Disposed)
                return;

            if (this._PreventActivateEvent)
                return;

            base.FireActivatedEvent(active);
        }

        protected override void FireEnabledEvent(bool enabled)
        {
            if (this.Disposed)
                return;

            if (this._PreventEnableEvent)
                return;

            base.FireEnabledEvent(enabled);
        }

        protected override void FireVisibleEvent(bool visible)
        {
            if (this.Disposed)
                return;

            _IsVisible = visible;

            if (this._PreventVisibleEvent)
                return;

            base.FireVisibleEvent(visible);
        }

        protected override void FireSizeChangedEvent(int width, int heigth)
        {
            if (this.Disposed)
                return;

            if (this._PreventSizeEvent)
                return;

            base.FireSizeChangedEvent(width, heigth);
        }

        internal void ToDefault()
        {
            this.ClearListeners();
            this.ClearEvents();

            if (!this.IsVisibleCore)
            {
                this._PreventVisibleEvent = true;
                this.ShowBehindCore();
                this._PreventVisibleEvent = false;
            }

            this._PreventStateEvent = true;
            this.NormalizeCore();
            this._PreventStateEvent = false;

            this._PreventVisibleEvent = true;
            this.HideCore();
            this._PreventVisibleEvent = false;

            var config = _Config;

            TitleCore = config.Title;
            TopMostCore = false;
            EnabledCore = true;
            PreventCloseCore = false;
            ClickThroughCore = false;

            InternalRect.ToDefault();

            if(config.CenterScreen)
                ToCenterCore();
        }

        #endregion

        protected override void DisposeCore(bool disposing)
        {
            WVUIContext = null;
            Utils32.WinInstances.Remove(this.Handle);
            CloseCore();
        }

        //=======================================//

        #region Private Methods

        private void ChangeState(WindowState value)
        {
            // La ventana DEBE estar visible
            if (!this.IsVisibleCore)
                return;

            // Evitar hacer dos cambios de State al mismo tiempo
            if (this._StateChange)
                return;

            // Tiene que ser un State distinto
            if (this.StateCore == value)
                return;

            this._StateChange = true;

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
                this._LastState = this.State;
                this._State = value;
                User32.ShowWindow(this.Handle, action);
                FireStateChangedEvent(value, value.ToString());
            }
            finally
            {
                this._StateChange = false;
            }
        }

        #endregion

        //=======================================//

        #region WNDPROC

        internal IntPtr WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            if (WebView.Disposed)
                return IntPtr.Zero;

            bool handled = FireRawEvent(hWnd, uMsg, wParam, lParam);

            if(handled)
                return IntPtr.Zero;

            switch ((WinMsg)uMsg)
            {
                case WinMsg.WM_CLOSE:
                    //Si se evita cerrar la ventana se dispara el evento OnClose
                    if (this.PreventCloseCore)
                        this.FireCloseEvent();
                    else
                        User32.DestroyWindow(hWnd);

                    return IntPtr.Zero;

                case WinMsg.WM_DESTROY:
                    this.WV.Dispose();

                    // Es el WebView original, se cierra todo
                    if (_IsMain)
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
                        WVUIContext?.RunAvailableWorkOnCurrentThread();

                    break;
            }

            return User32.DefWindowProcW(hWnd, uMsg, wParam, lParam);
        }

        private IntPtr HandleGetMinMaxInfo(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            RECT workArea = Utils32.GetWorkAreaByPosition(hWnd);
            MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

            var Rect = _Rect!;

            int MinWidth = Rect.MinWidth;
            int MinHeight = Rect.MinHeight;

            int MaxWidth = Rect.MaxWidth;
            int MaxHeight = Rect.MaxHeight;

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
            CoreWebView2Controller? coreWV = this.WV.InternalBrowser.WVController;

            if (coreWV == null)
                return User32.DefWindowProcW(hWnd, uMsg, wParam, lParam);

            // Si es un cambio de State controlado, se evita utilizar el Helpers
            WindowState currentState = this._StateChange ? this.State : Utils32.GetCurrentState(hWnd);

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
            if (this.StateCore == WindowState.Minimized)
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
            if (this._StateChange)
                return;

            // La ventana es invisible, es un falso cambio de estado,
            // provocado por redimensionar la ventana mediante HWnd o Rect
            if (!this.IsVisibleCore)
                return;

            //State currentState = this.State;

            if (this.StateCore == currentState)
                return;

            this._State = currentState;

            FireStateChangedEvent(currentState, currentState.ToString());
        }

        #endregion

    }
}