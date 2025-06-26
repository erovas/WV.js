using System.Runtime.InteropServices;
using WV.Win.Invoke.Enums;
using WV.Win.Invoke.Structs;

namespace WV.Win.Invoke
{
    [Guid("00020400-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface IDispatch
    {
        [PreserveSig]  // Agregado
        void GetTypeInfoCount(out int typeInfoCount);

        [PreserveSig]  // Agregado
        void GetTypeInfo(int info, int lcid, out IntPtr typeInfo);

        //[PreserveSig]  // Agregado
        void GetIDsOfNames(
            [MarshalAs(UnmanagedType.LPStruct)] Guid iid,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] names,
            int nameCount,
            int lcid,
            [MarshalAs(UnmanagedType.LPArray)][Out] uint[] DISPID
            );

        //[PreserveSig]  // Agregado
        void Invoke(
            uint dispid, 
            [MarshalAs(UnmanagedType.LPStruct)] Guid iid, 
            int lcid,
            InvokeFlags flags,
            [MarshalAs(UnmanagedType.LPArray)][In, Out] DISPPARAMS[] paramArray,
            out object result,
            out EXCEPINFO excepInfo,
            out uint err
            );
    }
}