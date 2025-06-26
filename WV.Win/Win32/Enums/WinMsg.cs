namespace WV.Win.Win32.Enums
{
    // https://learn.microsoft.com/es-es/windows/win32/winmsg/window-notifications
    // https://www.cnblogs.com/shangdawei/p/4014535.html

    /// <summary>
    /// IntPtr WndProc(IntPtr hWnd, WinMsg msg, WM_SysCmd wParam, IntPtr lParam)
    /// </summary>
    internal enum WinMsg : uint
    {
        /// <summary>
        /// Sent when a window is being destroyed. It is sent to the window procedure of 
        /// the window being destroyed after the window is removed from the screen.
        /// This message is sent first to the window being destroyed and then to the 
        /// child windows (if any) as they are destroyed. During the processing of the 
        /// message, it can be assumed that all child windows still exist.
        /// </summary>
        WM_DESTROY = 2,

        /// <summary>
        /// Sent to a window after its size has changed.
        /// </summary>
        WM_SIZE = 5,

        /// <summary>
        /// Sent to a window that the user is resizing. By processing this message, 
        /// an application can monitor the size and position of the drag rectangle and, 
        /// if needed, change its size or position.
        /// </summary>
        WM_SIZING = 0x0214,

        /// <summary>
        /// Sent as a signal that a window or an application should terminate.
        /// </summary>
        WM_CLOSE = 16,

        /// <summary>
        /// A window receives this message when the user chooses a command from 
        /// the Window menu (formerly known as the system or control menu) or when 
        /// the user chooses the maximize button, minimize button, restore button, 
        /// or close button.
        /// </summary>
        WM_SYSCOMMAND = 274,

        /// <summary>
        /// This message is sent when the system or another application makes a 
        /// request to paint a portion of an application's window
        /// </summary>
        WM_PAINT = 0x000F,

        WM_USER = 1024,

        /// <summary>
        /// Posted when the user presses the left mouse button while the cursor is within the nonclient area of a window. 
        /// This message is posted to the window that contains the cursor. 
        /// If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCLBUTTONDOWN = 0x00A1,

        /// <summary>
        /// Sent to a window in order to determine what part of the window corresponds 
        /// to a particular screen coordinate. This can happen, for example, when the 
        /// cursor moves, when a mouse button is pressed or released, or in response 
        /// to a call to a function such as WindowFromPoint. If the mouse is not captured,
        /// the message is sent to the window beneath the cursor. Otherwise, the message 
        /// is sent to the window that has captured the mouse.
        /// </summary>
        WM_NCHITTEST = 0x0084,

        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_SETICON = 0x0080,

        /// <summary>
        /// Sent to a window when the size or position of the window is about to change. 
        /// An application can use this message to override the window's default maximized 
        /// size and position, or its default minimum or maximum tracking size.
        /// </summary>
        WM_GETMINMAXINFO = 0x0024,

        /// <summary>
        /// 
        /// </summary>
        WM_ACTIVATE = 0x0006,

        /// <summary>
        /// Sent when the size and position of a window's client area must be calculated. 
        /// By processing this message, an application can control the content of the window's 
        /// client area when the size or position of the window changes.
        /// </summary>
        WM_NCCALCSIZE = 0x0083,

        

        WM_LBUTTONDBLCLK = 0x0203,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,

        /// <summary>
        /// Sent to a window if the mouse causes the cursor to move within a window and mouse input is not captured.
        /// </summary>
        WM_SETCURSOR = 0x0020,


        WM_MOUSEMOVE = 0x0200,

        WM_MOUSEWHEEL = 0x020A,

        /// <summary>
        /// Sent to a window whose size, position, or place in the Z order has changed as a result of a call 
        /// to the SetWindowPos function or another window-management function.
        /// </summary>
        WM_WINDOWPOSCHANGED = 0x47,

        /// <summary>
        /// Sent to a window whose size, position, or place in the Z order is about to change as a 
        /// result of a call to the SetWindowPos function or another window-management function.
        /// </summary>
        WM_WINDOWPOSCHANGING = 0x0046,

        /// <summary>
        /// Sent when an application changes the enabled state of a window. It is sent to the window 
        /// whose enabled state is changing
        /// </summary>
        WM_ENABLE = 0x000A,

        /// <summary>
        /// Sent to a window when the window is about to be hidden or shown.
        /// </summary>
        WM_SHOWWINDOW = 0x0018,

        /// <summary>
        /// 
        /// </summary>
        WM_CREATE = 0x0001,

        /// <summary>
        /// Sent after a window has been moved.
        /// </summary>
        WM_MOVE = 0x0003,

        WM_ERASEBKGND = 0x0014,

        WM_INPUT = 0x00FF,
    }
}