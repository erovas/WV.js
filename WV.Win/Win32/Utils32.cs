using WV.Win.Win32.Enums;
using WV.Win.Win32.Structs;
using System.Runtime.InteropServices;

namespace WV.Win.Win32
{
    internal static class Utils32
    {
        public static nint HInstance { get; } = User32.GetModuleHandle(null);
        public static int IDI_APPLICATION { get; } = 32512;
        public static int IDC_ARROW { get; } = 32512;
        public static int CW_USEDEFAULT { get; } = unchecked((int)0x80000000);


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

        public static bool HasWindowStyle(IntPtr hWnd, WinStyles style)
        {
            int currentStyle = User32.GetWindowLong(hWnd, (int)GetWinLong.GWL_STYLE);
            return (currentStyle & (int)style) != 0;
        }

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



















        //public static bool WindowIsOccluded(IntPtr hwnd)
        //{
        //    User32.GetCursorPos(out POINT p);
        //    const uint GW_HWNDNEXT = 2;
        //    for (IntPtr h = User32.GetTopWindow(IntPtr.Zero); h != IntPtr.Zero; h = User32.GetWindow(h, GW_HWNDNEXT))
        //    {
        //        if (!User32.IsWindowVisible(h))
        //            continue;

        //        if (!User32.GetWindowRect(h, out RECT r))
        //            continue;

        //        if (p.x >= r.Left && p.x <= r.Right && p.y >= r.Top && p.y <= r.Bottom)
        //        {
        //            if (h == hwnd)
        //                return false;

        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //public static bool IsPointInsideWindow(IntPtr hwnd)
        //{
        //    if (!User32.GetCursorPos(out POINT p))
        //        return false;

        //    if (!User32.GetWindowRect(hwnd, out RECT r))
        //        return false;

        //    return p.x >= r.Left && p.x <= r.Right && p.y >= r.Top && p.y <= r.Bottom;
        //}

    }
}