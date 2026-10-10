using System.Runtime.InteropServices;
using WV.Enums;
using WV.Win.Imp;
using WV.Win.Win32.Enums;
using WV.Win.Win32.Structs;

namespace WV.Win.Win32
{
    internal static class Utils32
    {
        /// <summary>
        /// Instancias de la ventana principal contenedora
        /// </summary>
        public static readonly Dictionary<long, WebView> WinInstances = new();

        #region Constants

        public static nint HInstance { get; } = User32.GetModuleHandle(null);
        public const int IDI_APPLICATION = 32512;
        public const int IDC_ARROW = 32512;
        public const int CW_USEDEFAULT = unchecked((int)0x80000000);
        public const uint TransparencyColor = 0x0000FF;
        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        #endregion

        #region WndProccess avoid GC

        // Delegado para el procedimiento de ventana
        public delegate nint WndProcDelegate(nint hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Mantener una referencia al delegado para evitar que el GC lo elimine
        /// </summary>
        public static WndProcDelegate WinProcDelegate { get; } = WndProc;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public static nint WndProc(nint hWnd, uint msg, IntPtr wParam, nint lParam)
        {
            if (Utils32.WinInstances.ContainsKey(hWnd))
                return Utils32.WinInstances[hWnd].InternalWindow.WndProc(hWnd, msg, wParam, lParam);

            return User32.DefWindowProcW(hWnd, msg, wParam, lParam);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static RECT GetWorkAreaByPosition(IntPtr hWnd)
        {
            RECT workArea = new RECT();

            // Obtener la posición del cursor
            User32.GetCursorPos(out POINT cursorPos);

            // Obtener el monitor donde está el cursor
            IntPtr hMonitor = User32.MonitorFromPoint(cursorPos, 0x2 /* MONITOR_DEFAULTTONEAREST */);

            // Si no se encuentra el monitor, usar el de la ventana como fallback
            if (hMonitor == IntPtr.Zero)
                hMonitor = User32.MonitorFromWindow(hWnd, 0x2 /* MONITOR_DEFAULTTONEAREST */);

            if (hMonitor != IntPtr.Zero)
            {
                // Obtener el área de trabajo del monitor destino
                MONITORINFO monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                User32.GetMonitorInfo(hMonitor, ref monitorInfo);
                workArea = monitorInfo.rcWork;
            }
            else
                User32.SystemParametersInfo(0x0030/*SPI_GETWORKAREA*/, 0, ref workArea, 0);

            return workArea;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static RECT GetWorkArea(IntPtr hWnd)
        {
            IntPtr hMonitor = User32.MonitorFromWindow(hWnd, 2);  // 2 = MONITOR_DEFAULTTONEAREST 
            RECT workArea = new RECT();

            if (hMonitor != IntPtr.Zero)
            {
                // Area de trabajo del monitor de donde se encuentra el WebView
                MONITORINFO mi = new MONITORINFO();
                mi.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                User32.GetMonitorInfo(hMonitor, ref mi);
                workArea = mi.rcWork;
            }
            else
                User32.SystemParametersInfo(0x0030/*SPI_GETWORKAREA*/, 0, ref workArea, 0);

            return workArea;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static bool HasWindowStyle(IntPtr hWnd, WinStyles style)
        {
            int currentStyle = User32.GetWindowLong(hWnd, (int)GetWinLong.GWL_STYLE);
            return (currentStyle & (int)style) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="style"></param>
        /// <param name="add"></param>
        public static void SetWinStyle(IntPtr hWnd, WinStyles style, bool add)
        {
            // Obtener estilos actuales
            int currentStyle = User32.GetWindowLong(hWnd, (int)GetWinLong.GWL_STYLE);

            // Si el estilo ya lo tiene y se quiere agregar, evitar hacer operación
            // Si el estilo no lo tiene y se quiere quitar, evitar hacer operación
            if (((currentStyle & (int)style) != 0) && add)
                return;

            if (add)
                currentStyle |= (int)style;
            else
                currentStyle &= ~(int)style;

            User32.SetWindowLong(hWnd, (int)GetWinLong.GWL_STYLE, currentStyle);

            // Forzar actualización de la ventana
            //SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0,
            //    SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

        /// <summary>
        /// Get posX or Width from lParam
        /// </summary>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public static int GetLowWord(nint lParam)
        {
            uint xy = (uint)lParam;
            int x = unchecked((short)xy);
            return x;
        }

        /// <summary>
        /// Get posY or Height from lParam
        /// </summary>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public static int GetHighWord(nint lParam)
        {
            uint xy = (uint)lParam;
            int y = unchecked((short)(xy >> 16));
            return y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="topMost"></param>
        /// <returns></returns>
        public static bool WindowTopMost(IntPtr hwnd, bool topMost)
        {
            return User32.SetWindowPos(
                hwnd,
                topMost ? new nint(-1) : new nint(-2),
                0, 0, //posX, posY, 
                0, 0, //Width, Height, 
                (uint)(SetWinPos.SWP_NOMOVE | SetWinPos.SWP_NOSIZE | SetWinPos.SWP_NOACTIVATE)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wv"></param>
        /// <returns></returns>
        public static RECT GetRECT(WebView wv)
        {
            Window win = wv.InternalWindow;
            Browser bro = wv.InternalBrowser;

            // La ventana está actualmente Minimizada
            if (win.State == WindowState.Minimized)
            {
                WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                placement.length = Marshal.SizeOf(placement);
                User32.GetWindowPlacement(win.Handle, ref placement);

                // Estaba Normalizada antes de ser minimizada
                if (win._LastState == WindowState.Normalized)
                    return placement.rcNormalPosition;

                // Encontrar el monitor asociado a ptMaxPosition
                IntPtr hMonitor = User32.MonitorFromWindow(win.Handle, MONITOR_DEFAULTTONEAREST);
                MONITORINFO monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
                User32.GetMonitorInfo(hMonitor, ref monitorInfo);
                return monitorInfo.rcWork;
            }

            // La ventana esta Normalizada o Maximizada
            User32.GetWindowRect(win.Handle, out RECT rect);

            if (bro.WVController != null)
            {
                rect.Right = rect.X + bro.WVController.Bounds.Width;
                rect.Bottom = rect.Y + bro.WVController.Bounds.Height;
            }

            return rect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wv"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public static void SetRECT(WebView wv, int X, int Y, int Width, int Height)
        {
            User32.MoveWindow(wv.InternalWindow.Handle, X, Y, Width, Height, true);
        }

        private static bool _MsgLoopRunned = false;
        public static void RunMessageLoop()
        {
            if (_MsgLoopRunned)
                return;

            _MsgLoopRunned = true;

            while (User32.GetMessage(out MSG msg, nint.Zero, 0, 0))
            {
                User32.TranslateMessage(ref msg);
                User32.DispatchMessage(ref msg);
            }
        }

        public static WindowState GetCurrentState(IntPtr hwnd)
        {
            var placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            User32.GetWindowPlacement(hwnd, ref placement);

            WindowState newState = WindowState.Normalized;

            switch (placement.showCmd)
            {
                //case 1: // SW_SHOWNORMAL
                //    newState = State.Normal;
                //    break;
                case 2: // SW_SHOWMINIMIZED
                    newState = WindowState.Minimized;
                    break;
                case 3: // SW_SHOWMAXIMIZED
                    newState = WindowState.Maximized;
                    break;
            }

            return newState;
        }

        public static IntPtr CreateMainWindows(WebView wv, string name)
        {
            string UID = wv.UID;
            IntPtr hwnd = (wv.InternalIsMain ? IntPtr.Zero : wv.InternalWebView.InternalWindow.Handle);

            // Registrar clase de ventana principal
            WNDCLASSEX MainWinClass = new WNDCLASSEX
            {
                cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
                style = 0,
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(Utils32.WinProcDelegate),
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = Utils32.HInstance,
                hIcon = User32.LoadIcon(IntPtr.Zero, Utils32.IDI_APPLICATION),
                hCursor = User32.LoadCursor(IntPtr.Zero, Utils32.IDC_ARROW),
                hbrBackground = User32.CreateSolidBrush(Utils32.TransparencyColor),
                lpszMenuName = null,
                lpszClassName = UID,  // Debe ser unico para cada ventana
                hIconSm = User32.LoadIcon(IntPtr.Zero, Utils32.IDI_APPLICATION)
            };

            if (User32.RegisterClassEx(ref MainWinClass) == 0)
                throw new Exception("!Error al registrar la clase de la ventana principal! Código de error: " + Marshal.GetLastWin32Error());

            // Crear la ventana utilizando CreateWindowExW (versión Unicode explícita)
            IntPtr MainhWnd = User32.CreateWindowExW(
                (int)WinStylesEx.WS_EX_LAYERED, // Habilita ventana con capas para transparencia
                UID,   // Nombre de la clase registrada, debe ser unica
                name, // Título de la ventana, por defecto string vacio
                (uint)(WinStyles.WS_THICKFRAME | WinStyles.WS_SYSMENU | WinStyles.WS_MINIMIZEBOX | WinStyles.WS_MAXIMIZEBOX),
                //(uint)(WinStyles.WS_CAPTION | WinStyles.WS_THICKFRAME | WinStyles.WS_MINIMIZEBOX | WinStyles.WS_MAXIMIZEBOX),
                Utils32.CW_USEDEFAULT,
                Utils32.CW_USEDEFAULT,
                App.Window.Rect.MinWidth,
                App.Window.Rect.MinHeight,
                hwnd,    // hWnd de ventana padre 
                IntPtr.Zero,
                Utils32.HInstance,
                IntPtr.Zero
            );

            if (MainhWnd == IntPtr.Zero)
                throw new Exception("!!!Error al crear la ventana. Código de error: " + Marshal.GetLastWin32Error());

            // Guardando instancia de WebView en diccionario
            Utils32.WinInstances.Add(MainhWnd, wv);

            // Establecer el color clave para la transparencia
            User32.SetLayeredWindowAttributes(MainhWnd, Utils32.TransparencyColor, 0, DWFlags.LWA_COLORKEY);

            return MainhWnd;
        }

        public static void MsgBoxError(IntPtr handle, string msg, Exception? ex = null)
        {
            User32.MessageBox(handle, msg + System.Environment.NewLine + ex?.Message, "Error", (uint)(MsgBoxStyle.MB_OK | MsgBoxStyle.MB_ICONERROR));
        }

        public static bool MsgBoxYesNo(IntPtr handle, string msg, string caption = "")
        {
            var result = User32.MessageBox(handle, msg, caption, (uint)(MsgBoxStyle.MB_YESNO | MsgBoxStyle.MB_ICONINFORMATION));
            return result == (int)MsgBoxResult.IDYES;
        }

        public static void MsgBoxInfo(IntPtr handle, string msg, string caption = "")
        {
            User32.MessageBox(handle, msg, caption, (uint)(MsgBoxStyle.MB_OK | MsgBoxStyle.MB_ICONINFORMATION));
        }

    }
}