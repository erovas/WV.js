﻿using System.Runtime.Intrinsics.X86;

namespace WV.Win.Win32.Enums
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-messagebox
    /// </summary>
    internal enum MsgBoxStyle : uint
    {
        #region BUTTONS

        /// <summary>
        /// The message box contains three push buttons: Abort, Retry, and Ignore.
        /// </summary>
        MB_ABORTRETRYIGNORE = 0x00000002,

        /// <summary>
        /// The message box contains three push buttons: Cancel, Try Again, Continue. 
        /// Use this message box type instead of MB_ABORTRETRYIGNORE.
        /// </summary>
        MB_CANCELTRYCONTINUE = 0x00000006,

        /// <summary>
        /// Adds a Help button to the message box. When the user clicks the Help button or presses F1, 
        /// the system sends a WM_HELP message to the owner.
        /// </summary>
        MB_HELP = 0x00004000,

        /// <summary>
        /// The message box contains one push button: OK. This is the default.
        /// </summary>
        MB_OK = 0x00000000,

        /// <summary>
        /// The message box contains two push buttons: OK and Cancel.
        /// </summary>
        MB_OKCANCEL = 0x00000001,

        /// <summary>
        /// The message box contains two push buttons: Retry and Cancel.
        /// </summary>
        MB_RETRYCANCEL = 0x00000005,

        /// <summary>
        /// The message box contains two push buttons: Yes and No.
        /// </summary>
        MB_YESNO = 0x00000004,

        /// <summary>
        /// The message box contains three push buttons: Yes, No, and Cancel.
        /// </summary>
        MB_YESNOCANCEL = 0x00000003,

        #endregion

        #region ICONS

        /// <summary>
        /// An exclamation-point icon appears in the message box.
        /// </summary>
        MB_ICONEXCLAMATION = 0x00000030,

        /// <summary>
        /// An exclamation-point icon appears in the message box.
        /// </summary>
        MB_ICONWARNING = 0x00000030,

        /// <summary>
        /// An icon consisting of a lowercase letter i in a circle appears in the message box.
        /// </summary>
        MB_ICONINFORMATION = 0x00000040,

        /// <summary>
        /// An icon consisting of a lowercase letter i in a circle appears in the message box.
        /// </summary>
        MB_ICONASTERISK = 0x00000040,

        /// <summary>
        /// A question-mark icon appears in the message box. The question-mark message icon is no longer 
        /// recommended because it does not clearly represent a specific type of message and because the 
        /// phrasing of a message as a question could apply to any message type. In addition, users can 
        /// confuse the message symbol question mark with Help information. Therefore, do not use this 
        /// question mark message symbol in your message boxes. The system continues to support its 
        /// inclusion only for backward compatibility.
        /// </summary>
        MB_ICONQUESTION = 0x00000020,

        /// <summary>
        /// A stop-sign icon appears in the message box.
        /// </summary>
        MB_ICONSTOP = 0x00000010,

        /// <summary>
        /// A stop-sign icon appears in the message box.
        /// </summary>
        MB_ICONERROR = 0x00000010,

        /// <summary>
        /// A stop-sign icon appears in the message box.
        /// </summary>
        MB_ICONHAND = 0x00000010,

        /// <summary>
        /// 
        /// </summary>
        MB_USERICON = 0x00000080,

        #endregion

        #region DEFAULT BUTTON

        /// <summary>
        /// The first button is the default button.
        /// MB_DEFBUTTON1 is the default unless MB_DEFBUTTON2, MB_DEFBUTTON3, or MB_DEFBUTTON4 is specified.
        /// </summary>
        MB_DEFBUTTON1 = 0x00000000,

        /// <summary>
        /// The second button is the default button.
        /// </summary>
        MB_DEFBUTTON2 = 0x00000100,

        /// <summary>
        /// The third button is the default button.
        /// </summary>
        MB_DEFBUTTON3 = 0x00000200,

        /// <summary>
        /// The fourth button is the default button.
        /// </summary>
        MB_DEFBUTTON4 = 0x00000300,

        #endregion

        #region MODALITY

        /// <summary>
        /// The user must respond to the message box before continuing work in the window identified by the hWnd parameter. 
        /// However, the user can move to the windows of other threads and work in those windows.
        /// Depending on the hierarchy of windows in the application, the user may be able to move to other windows within 
        /// the thread. All child windows of the parent of the message box are automatically disabled, but pop-up windows are not.
        ///
        /// MB_APPLMODAL is the default if neither MB_SYSTEMMODAL nor MB_TASKMODAL is specified.
        /// </summary>
        MB_APPLMODAL = 0x00000000,

        /// <summary>
        /// Same as MB_APPLMODAL except that the message box has the WS_EX_TOPMOST style. Use system-modal message boxes to notify 
        /// the user of serious, potentially damaging errors that require immediate attention (for example, running out of memory). 
        /// This flag has no effect on the user's ability to interact with windows other than those associated with hWnd.
        /// </summary>
        MB_SYSTEMMODAL = 0x00001000,

        /// <summary>
        /// Same as MB_APPLMODAL except that all the Top-level windows belonging to the current thread are disabled if the hWnd 
        /// parameter is NULL. Use this flag when the calling application or library does not have a window handle available but 
        /// still needs to prevent input to other windows in the calling thread without suspending other threads.
        /// </summary>
        MB_TASKMODAL = 0x00002000,

        #endregion

        #region OTHER OPTIONS

        /// <summary>
        /// Same as desktop of the interactive window station. For more information, see Window Stations.
        /// If the current input desktop is not the default desktop, MessageBox does not return until the user switches to the default desktop.
        /// </summary>
        MB_DEFAULT_DESKTOP_ONLY = 0x00020000,

        /// <summary>
        /// The text is Right-justified.
        /// </summary>
        MB_RIGHT = 0x00080000,

        /// <summary>
        /// Displays message and caption text using Right-to-Left reading order on Hebrew and Arabic systems.
        /// </summary>
        MB_RTLREADING = 0x00100000,

        /// <summary>
        /// The message box becomes the foreground window. Internally, the system calls the SetForegroundWindow function for the message box.
        /// </summary>
        MB_SETFOREGROUND = 0x00010000,

        /// <summary>
        /// The message box is created with the WS_EX_TOPMOST window style.
        /// </summary>
        MB_TOPMOST = 0x00040000,

        /// <summary>
        /// The caller is a service notifying the user of an event. The function displays a message box on the current active desktop, 
        /// even if there is no user logged on to the computer.
        /// Terminal Services: If the calling thread has an impersonation token, the function directs the message box to the session 
        /// specified in the impersonation token.
        /// If this flag is set, the hWnd parameter must be NULL. This is so that the message box can appear on a desktop other than 
        /// the desktop corresponding to the hWnd.
        /// For information on security considerations in regard to using this flag, see Interactive Services.In particular, be aware 
        /// that this flag can produce interactive content on a locked desktop and should therefore be used for only a very limited 
        /// set of scenarios, such as resource exhaustion.
        /// </summary>
        MB_SERVICE_NOTIFICATION = 0x00200000,

        #endregion

        MB_NOFOCUS = 0x00008000,
        MB_SERVICE_NOTIFICATION_NT3X = 0x00040000,
        MB_TYPEMASK = 0x0000000F,
        MB_ICONMASK = 0x000000F0,
        MB_DEFMASK = 0x00000F00,
        MB_MODEMASK = 0x00003000,
        MB_MISCMASK = 0x0000C000,
    }
}