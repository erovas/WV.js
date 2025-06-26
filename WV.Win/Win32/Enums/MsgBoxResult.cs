namespace WV.Win.Win32.Enums
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-messagebox
    /// </summary>
    internal enum MsgBoxResult
    {
        /// <summary>
        /// The OK button was selected.
        /// </summary>
        IDOK = 1,

        /// <summary>
        /// The Cancel button was selected.
        /// </summary>
        IDCANCEL = 2,

        /// <summary>
        /// The Abort button was selected.
        /// </summary>
        IDABORT = 3,

        /// <summary>
        /// The Retry button was selected.
        /// </summary>
        IDRETRY = 4,

        /// <summary>
        /// The Ignore button was selected.
        /// </summary>
        IDIGNORE = 5,

        /// <summary>
        /// The Yes button was selected.
        /// </summary>
        IDYES = 6,

        /// <summary>
        /// The No button was selected.
        /// </summary>
        IDNO = 7,

        /// <summary>
        /// 
        /// </summary>
        IDCLOSE = 8,

        /// <summary>
        /// 
        /// </summary>
        IDHELP = 9,

        /// <summary>
        /// The Try Again button was selected.
        /// </summary>
        IDTRYAGAIN = 10,

        /// <summary>
        /// The Continue button was selected.
        /// </summary>
        IDCONTINUE = 11,

        /// <summary>
        /// 
        /// </summary>
        IDASYNC = 32001,

        /// <summary>
        /// 
        /// </summary>
        IDTIMEOUT = 32000,
    }
}