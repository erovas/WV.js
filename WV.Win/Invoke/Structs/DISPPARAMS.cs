using System.Runtime.InteropServices;

namespace WV.Win.Invoke.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DISPPARAMS
    {
        /// <summary>
        /// Array de argumentos (VARIANTARG*)
        /// </summary>
        public IntPtr varArgs;

        /// <summary>
        /// Array de DISPIDs para argumentos nombrados
        /// </summary>
        public IntPtr namedArgDispIds;

        /// <summary>
        /// Número de argumentos
        /// </summary>
        public int argCount;

        /// <summary>
        /// Número de argumentos nombrados
        /// </summary>
        public int namedArgCount;
    }
}