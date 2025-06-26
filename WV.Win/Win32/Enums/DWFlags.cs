// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setlayeredwindowattributes
namespace WV.Win.Win32.Enums
{
    internal enum DWFlags : uint
    {
        /// <summary>
        /// Use pbAlpha to determine the opacity of the layered window.
        /// </summary>
        LWA_COLORKEY = 0x00000001,

        /// <summary>
        /// Use pcrKey as the transparency color.
        /// </summary>
        LWA_ALPHA = 0x00000002
    }
}