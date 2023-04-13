namespace WV.Windows.Constants
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest
    /// </summary>
    public static class WM_NCHitTest
    {
        public const int

            // In the border of a window that does not have a sizing border.
            HTBORDER = 18,

            /*
            In the lower-horizontal border of a resizable window 
            (the user can click the mouse to resize the window vertically).
             */
            HTBOTTOM = 15,

            /*
            In the lower-left corner of a border of a resizable window 
            (the user can click the mouse to resize the window diagonally).
             */
            HTBOTTOMLEFT = 16,

            /*
            In the lower-right corner of a border of a resizable window 
            (the user can click the mouse to resize the window diagonally).
             */
            HTBOTTOMRIGHT = 17,

            // In a title bar.
            HTCAPTION = 2,

            // In a client area.
            HTCLIENT = 1,

            // In a Close button.
            HTCLOSE = 20,

            /*
            On the screen background or on a dividing line between windows 
            (same as HTNOWHERE, except that the DefWindowProc function 
            produces a system beep to indicate an error).
            */
            HTERROR = -1,

            // In a size box (same as HTSIZE).
            HTGROWBOX = 4,

            // In a Help button.
            HTHELP = 21,

            // In a horizontal scroll bar.
            HTHSCROLL = 6,

            /*
            In the left border of a resizable window 
            (the user can click the mouse to resize the window horizontally).
             */
            HTLEFT = 10,

            // In a menu.
            HTMENU = 5,

            // In a Maximize button.
            HTMAXBUTTON = 9,

            // In a Minimize button.
            HTMINBUTTON = 8,

            // On the screen background or on a dividing line between windows.
            HTNOWHERE = 0,

            // In a Minimize button.
            HTREDUCE = 8,

            /*
            In the right border of a resizable window 
            (the user can click the mouse to resize the window horizontally).
             */
            HTRIGHT = 11,

            // In a size box (same as HTGROWBOX).
            HTSIZE = 4,

            // In a window menu or in a Close button in a child window.
            HTSYSMENU = 3,

            // In the upper-horizontal border of a window.
            HTTOP = 12,

            // In the upper-left corner of a window border.
            HTTOPLEFT = 13,

            // In the upper-right corner of a window border.
            HTTOPRIGHT = 14,

            /*
            In a window currently covered by another window in the same thread 
            (the message will be sent to underlying windows in the same thread 
            until one of them returns a code that is not HTTRANSPARENT).
             */
            HTTRANSPARENT = -1,

            // In the vertical scroll bar.
            HTVSCROLL = 7,

            // In a Maximize button.
            HTZOOM = 9;
    }
}
