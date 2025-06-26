using System.Runtime.InteropServices;

namespace WV.Win.Win32.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;
    }
}