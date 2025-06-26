using System.Runtime.InteropServices;

namespace WV.Win.Invoke.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct EXCEPINFO
    {
        public short code;

        public short reserved;
        
        public IntPtr strSource;
        
        public IntPtr strDescription;
        
        public IntPtr strHelpFile;
        
        public uint helpContext;
        
        public IntPtr pvReserved;
        
        public IntPtr pfnDeferredFillIn;
        
        public int scode;
    }
}