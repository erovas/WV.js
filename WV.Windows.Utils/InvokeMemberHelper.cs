using System.Reflection;
using System.Runtime.InteropServices;

namespace WV.Windows.Utils
{
    //https://github.com/MicrosoftEdge/WebView2Feedback/issues/75#issuecomment-711443711  InvokeHelper
    //https://github.com/MicrosoftEdge/WebView2Feedback/issues/347#issuecomment-663014887 Examples, Constants
    //Example
    //InvokeMemberHelper.InvokeDispMethod(fn, null, "Arg0", "Arg1", 123, 4321);

    //https://github.com/MicrosoftEdge/WebView2Feedback/issues/918   other example
    //https://gist.github.com/yizhang82/a1268d3ea7295a8a1496e01d60ada816

    public static class InvokeMemberHelper
    {
        public enum MethodKind
        {
            Method,
            PropertyPut,
            PropertyPutRef,
            PropertyGet
        }

        public static object InvokeDispPropertyGet(
            this object target,
            string name,
            params object[] args)
        {
            return target.InvokeDispMember(
                name, args, null, MethodKind.PropertyGet);
        }

        public static object InvokeDispPropertyPut(
            this object target,
            string name,
            params object[] args)
        {
            return target.InvokeDispMember(
                name, args, null, MethodKind.PropertyPut);
        }

        public static object InvokeDispMethod(this object target, string name, params object[] args)
        {
            return target.InvokeDispMember(name, args, null, MethodKind.Method);
        }

        private const int LOCALE_USER_DEFAULT = 0x0400;
        private const int DISPID_UNKNOWN = unchecked((int)0xFFFFFFFF);

        unsafe public static object InvokeDispMember(this object target, string name, object[] args, ParameterModifier[] modifiers = null, MethodKind methodKind = MethodKind.Method)
        {
            // There should be either no modifiers or exactly one modifier
            if (modifiers != null && modifiers.Length > 1)
                throw new ArgumentException();

            //
            // Obtain IDispatch interface
            //
            IDispatch disp = (IDispatch)target;

            // 
            // Retrieve DISPID
            //
            //uint dispId = GetDispID(disp, name);

            IntPtr pVariantArgArray = IntPtr.Zero;
            IntPtr pDispIDArray = IntPtr.Zero;

            int argCount = args == null ? 0 : args.Length;
            int variantSize = Marshal.SizeOf<Variant>();
            object result;

            try
            {
                //
                // Package arguments
                //
                if (argCount > 0)
                {
                    pVariantArgArray = Marshal.AllocCoTaskMem(variantSize * argCount);
                    for (int i = 0; i < argCount; ++i)
                    {
                        object arg = args[i];

                        // !! The arguments should be in REVERSED order!!
                        int actualIndex = (argCount - i - 1);

                        // If need to pass by ref, create a by-ref variant 
                        if (modifiers != null && modifiers[0][i])
                        {
                            // Create a VARIANT that the by-ref VARIANT points to
                            IntPtr pTmpVariant = Marshal.AllocCoTaskMem(variantSize);
                            Marshal.GetNativeVariantForObject(args[i], pTmpVariant);

                            // Create the by-ref VARIANT
                            MakeByRefVariant(pVariantArgArray + actualIndex * variantSize, pTmpVariant);
                        }
                        else
                        {
                            Marshal.GetNativeVariantForObject(args[i], pVariantArgArray + actualIndex * variantSize);
                        }
                    }
                }

                DISPPARAMS[] paramArray = new DISPPARAMS[1];
                paramArray[0].varArgs = pVariantArgArray;
                paramArray[0].argCount = argCount;

                if (methodKind == MethodKind.PropertyPut || methodKind == MethodKind.PropertyPutRef)
                {
                    //
                    // For property putters, the first DISPID argument needs to be DISPID_PROPERTYPUT
                    //
                    pVariantArgArray = Marshal.AllocCoTaskMem(variantSize * argCount);
                    Marshal.WriteInt32(pVariantArgArray, DISPID_PROPERTYPUT);

                    paramArray[0].namedArgCount = 1;
                    paramArray[0].namedArgDispIds = pVariantArgArray;
                }
                else
                {
                    //
                    // Otherwise, no named parameters are necessary
                    //
                    paramArray[0].namedArgCount = 0;
                    paramArray[0].namedArgDispIds = IntPtr.Zero;
                }

                //
                // Make the call
                //
                EXCEPINFO info = default(EXCEPINFO);
                uint err;
                short flags;
                if (methodKind == MethodKind.Method)
                    flags = (short)Flags.DISPATCH_METHOD;
                else if (methodKind == MethodKind.PropertyPut)
                    flags = (short)Flags.DISPATCH_PROPERTYPUT;
                else if (methodKind == MethodKind.PropertyPutRef)
                    flags = (short)Flags.DISPATCH_PROPERTYPUTREF;
                else if (methodKind == MethodKind.PropertyGet)
                    flags = (short)Flags.DISPATCH_PROPERTYGET;
                else
                    throw new ArgumentException();

                try
                {
                    disp.Invoke(
                        //(uint)dispId,
                        unchecked((uint)0xFFFFFFFF),
                        IID_NULL,
                        //LCID_DEFAULT,
                        LOCALE_USER_DEFAULT,
                        flags,
                        paramArray,
                        out result,
                        out info,
                        out err);
                }
                catch (Exception ex)
                {
                    if (ex.HResult == DISP_E_EXCEPTION)
                    {
                        Exception realException;
                        if (info.scode != 0)
                            realException = Marshal.GetExceptionForHR(info.scode, IntPtr.Zero);
                        else
                            realException = Marshal.GetExceptionForHR((int)info.code, IntPtr.Zero);

                        if (info.strDescription != IntPtr.Zero)
                        {
                            string realErrorMessage = Marshal.PtrToStringBSTR(info.strDescription);

                            // @TODO - find a way to return this to the caller
                        }

                        throw realException;
                    }

                    throw;
                }
                //
                // Now back propagate the by-ref arguments
                //
                for (int i = 0; i < argCount; ++i)
                {
                    object arg = args[i];

                    // !! The arguments should be in REVERSED order!!
                    int actualIndex = (argCount - i - 1);

                    // If need to pass by ref, back propagate 
                    if (modifiers != null && modifiers[0][i])
                    {
                        args[i] = Marshal.GetObjectForNativeVariant(pVariantArgArray + actualIndex * variantSize);
                    }
                }

                return result;
            }
            finally
            {
                //
                // Free memory
                //
                if (pVariantArgArray != IntPtr.Zero)
                {
                    for (int i = 0; i < argCount; ++i)
                    {
                        VariantClear(pVariantArgArray + i * variantSize);
                    }

                    Marshal.FreeCoTaskMem(pVariantArgArray);
                }

                if (pDispIDArray != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pDispIDArray);
                }
            }
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/previous-versions/windows/desktop/automat/dispid-constants
        /// 
        /// </summary>
        /// <param name="disp"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        static uint GetDispID(IDispatch disp, string name)
        {
            if (name == null)
                return unchecked((uint)0xFFFFFFFF);


            uint[] dispid = new uint[1];
            try
            {
                disp.GetIDsOfNames(IID_NULL, new string[] { name }, 1, LCID_DEFAULT, dispid);
            }
            catch (Exception ex)
            {
                var asd = ex.Message;
                throw;
            }


            return dispid[0];
        }

        public enum VarEnum : ushort
        {
            VT_EMPTY = 0,
            VT_NULL = 1,
            VT_I2 = 2,
            VT_I4 = 3,
            VT_R4 = 4,
            VT_R8 = 5,
            VT_CY = 6,
            VT_DATE = 7,
            VT_BSTR = 8,
            VT_DISPATCH = 9,
            VT_ERROR = 10,
            VT_BOOL = 11,
            VT_VARIANT = 12,
            VT_UNKNOWN = 13,
            VT_DECIMAL = 14,
            VT_I1 = 16,
            VT_UI1 = 17,
            VT_UI2 = 18,
            VT_UI4 = 19,
            VT_I8 = 20,
            VT_UI8 = 21,
            VT_INT = 22,
            VT_UINT = 23,
            VT_VOID = 24,
            VT_HRESULT = 25,
            VT_PTR = 26,
            VT_SAFEARRAY = 27,
            VT_CARRAY = 28,
            VT_USERDEFINED = 29,
            VT_LPSTR = 30,
            VT_LPWSTR = 31,
            VT_RECORD = 36,
            VT_FILETIME = 64,
            VT_BLOB = 65,
            VT_STREAM = 66,
            VT_STORAGE = 67,
            VT_STREAMED_OBJECT = 68,
            VT_STORED_OBJECT = 69,
            VT_BLOB_OBJECT = 70,
            VT_CF = 71,
            VT_CLSID = 72,
            VT_VECTOR = 0x1000,
            VT_ARRAY = 0x2000,
            VT_BYREF = 0x4000
        }

        /*
        const byte[] SizeOfVariant = new map[] 
        {
            0,                      // VT_EMPTY
            0,                      // VT_NULL
            2,                      // VT_I2
            4,                      // VT_I4
            4,                      // VT_R4
            8,                      // VT_R8
            8,                      // VT_CY
            IntPtr.Size,            // VT_DATE
            IntPtr.Size,            // VT_BSTR
            IntPtr.Size,            // VT_DISPATCH
            4,                      // VT_ERROR
            2,                      // VT_BOOL
            IntPtr.Size,            // VT_VARIANT
            IntPtr.Size,            // VT_UNKNOWN
            IntPtr.Size,            // VT_DECIMAL
            0,                      // unused
            1,                      // VT_I1
            1,                      // VT_UI1
            2,                      // VT_UI2
            4,                      // VT_UI4
            8,                      // VT_I8
            8,                      // VT_UI8
            4,                      // VT_INT 
            4,                      // VT_UINT
            0,                      // VT_VOID
            4,                      // VT_HRESULT
            IntPtr.Size,            // VT_PTR
            IntPtr.Size,            // VT_SAFEARRAY
            IntPtr.Size,            // VT_CARRAY
            IntPtr.Size,            // VT_USERDEFINED
            IntPtr.Size,            // VT_LPSTR
            IntPtr.Size,            // VT_LPWSTR
        };
        */

        private static unsafe void MakeByRefVariant(IntPtr pDestVariant, IntPtr pSrcVariant)
        {
            Variant* psrcvar = (Variant*)pSrcVariant;
            Variant* pdestvar = (Variant*)pDestVariant;

            switch ((VarEnum)psrcvar->_typeUnion._vt)
            {
                case VarEnum.VT_EMPTY:
                    // BY REF VT_EMPTY is not valid
                    throw new ArgumentException();

                case VarEnum.VT_RECORD:
                    // Representation of record is the same with or without byref
                    pdestvar->_typeUnion._unionTypes._record._record = psrcvar->_typeUnion._unionTypes._record._record;
                    pdestvar->_typeUnion._unionTypes._record._recordInfo = psrcvar->_typeUnion._unionTypes._record._recordInfo;
                    break;

                case VarEnum.VT_VARIANT:
                    pdestvar->_typeUnion._unionTypes._byref = new IntPtr(psrcvar);
                    break;

                case VarEnum.VT_DECIMAL:
                    pdestvar->_typeUnion._unionTypes._byref = new IntPtr(&(psrcvar->_decimal));
                    break;

                default:
                    // All the other cases start at the same offset so using &_i4 should work
                    pdestvar->_typeUnion._unionTypes._byref = new IntPtr(&(psrcvar->_typeUnion._unionTypes._i4));
                    break;
            }

            pdestvar->_typeUnion._vt = (ushort)(psrcvar->_typeUnion._vt | (ushort)VarEnum.VT_BYREF);
        }

        #region Implementation Details 
        static int LCID_DEFAULT = 0x0409;
        static int DISPID_PROPERTYPUT = -3;
        static int DISP_E_EXCEPTION = unchecked((int)0x80020009);
        static Guid IID_NULL = new Guid();

        [DllImport("oleaut32.dll")]
        static extern void VariantClear(IntPtr pVariant);

        [Guid("00020400-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport]
        interface IDispatch
        {
            void GetTypeInfoCount(out int typeInfoCount);
            void GetTypeInfo(int info, int lcid, out IntPtr typeInfo);
            void GetIDsOfNames(
                [MarshalAs(UnmanagedType.LPStruct)] Guid iid,
                [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] names,
                int nameCount,
                int lcid,
                [MarshalAs(UnmanagedType.LPArray)][Out] uint[] DISPID);
            void Invoke(uint dispid, [MarshalAs(UnmanagedType.LPStruct)] Guid iid, int lcid, short flags,
                [MarshalAs(UnmanagedType.LPArray)][In, Out] DISPPARAMS[] paramArray,
                out object result,
                out EXCEPINFO excepInfo,
                out uint err
                );
        }

        struct DISPPARAMS
        {
            public IntPtr varArgs;
            public IntPtr namedArgDispIds;
            public int argCount;
            public int namedArgCount;
        }

        struct EXCEPINFO
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

        enum Flags : short
        {
            DISPATCH_METHOD = 0x1,
            DISPATCH_PROPERTYGET = 0x2,
            DISPATCH_PROPERTYPUT = 0x4,
            DISPATCH_PROPERTYPUTREF = 0x8
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Variant
        {
            // Most of the data types in the Variant are carried in _typeUnion
            [FieldOffset(0)]
            internal TypeUnion _typeUnion;

            // Decimal is the largest data type and it needs to use the space that is normally unused in TypeUnion._wReserved1, etc.
            // Hence, it is declared to completely overlap with TypeUnion. A Decimal does not use the first two bytes, and so
            // TypeUnion._vt can still be used to encode the type.
            [FieldOffset(0)]
            internal Decimal _decimal;

            [StructLayout(LayoutKind.Explicit)]
            internal struct TypeUnion
            {
                [FieldOffset(0)]
                internal ushort _vt;
                [FieldOffset(2)]
                internal ushort _wReserved1;
                [FieldOffset(4)]
                internal ushort _wReserved2;
                [FieldOffset(6)]
                internal ushort _wReserved3;
                [FieldOffset(8)]
                internal UnionTypes _unionTypes;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct Record
            {
                internal IntPtr _record;
                internal IntPtr _recordInfo;
            }

            [StructLayout(LayoutKind.Explicit)]
            internal struct UnionTypes
            {
                [FieldOffset(0)]
                internal SByte _i1;
                [FieldOffset(0)]
                internal Int16 _i2;
                [FieldOffset(0)]
                internal Int32 _i4;
                [FieldOffset(0)]
                internal Int64 _i8;
                [FieldOffset(0)]
                internal Byte _ui1;
                [FieldOffset(0)]
                internal UInt16 _ui2;
                [FieldOffset(0)]
                internal UInt32 _ui4;
                [FieldOffset(0)]
                internal UInt64 _ui8;
                [FieldOffset(0)]
                internal Int32 _int;
                [FieldOffset(0)]
                internal UInt32 _uint;
                [FieldOffset(0)]
                internal Int16 _bool;
                [FieldOffset(0)]
                internal Int32 _error;
                [FieldOffset(0)]
                internal Single _r4;
                [FieldOffset(0)]
                internal Double _r8;
                [FieldOffset(0)]
                internal Int64 _cy;
                [FieldOffset(0)]
                internal double _date;
                [FieldOffset(0)]
                internal IntPtr _bstr;
                [FieldOffset(0)]
                internal IntPtr _unknown;
                [FieldOffset(0)]
                internal IntPtr _dispatch;
                [FieldOffset(0)]
                internal IntPtr _pvarVal;
                [FieldOffset(0)]
                internal IntPtr _byref;
                [FieldOffset(0)]
                internal Record _record;
            }
        }

        #endregion

    }
}