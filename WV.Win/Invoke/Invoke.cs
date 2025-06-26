using System.Reflection;
using WV.Win.Invoke.Enums;
using WV.Win.Invoke.Structs;
using System.Runtime.InteropServices;
using VarEnum = WV.Win.Invoke.Enums.VarEnum;

namespace WV.Win.Invoke
{
    //https://gist.github.com/yizhang82/a1268d3ea7295a8a1496e01d60ada816                    // InvokeMemberHelper
    //https://github.com/MicrosoftEdge/WebView2Feedback/issues/75#issuecomment-711443711    // Modifications
    // Related
    //https://github.com/MicrosoftEdge/WebView2Feedback/issues/347#issuecomment-663014887   
    //https://github.com/MicrosoftEdge/WebView2Feedback/issues/918
    internal static class Invoker
    {

        public static object? PropertyGet(object target, string name, params object[] args)
        {
            return InvokeDispMember(target, name, args, null, MethodKind.PropertyGet);
        }

        public static object? PropertySet(object target, string name, params object[] args)
        {
            return InvokeDispMember(target, name, args, null, MethodKind.PropertyPut);
        }

        public static object? PropertySetRef(object target, string name, params object[] args)
        {
            return InvokeDispMember(target, name, args, null, MethodKind.PropertyPutRef);
        }

        public static object? ExecuteMethod(object target, string name, params object[] args)
        {
            return InvokeDispMember(target, name, args);
        }

        #region PRIVATE

        private const int LOCALE_USER_DEFAULT = 0x0400;
        private const uint DISPID_UNKNOWN = unchecked((uint)0xFFFFFFFF);
        private const int LCID_DEFAULT = 0x0409;
        private const int DISPID_PROPERTYPUT = -3;
        private const int DISP_E_EXCEPTION = unchecked((int)0x80020009);
        private static Guid IID_NULL = Guid.Empty; //new Guid();

        [DllImport("oleaut32.dll")]
        private static extern void VariantClear(IntPtr pVariant);

        unsafe private static object? InvokeDispMember(object target, string name, object?[] args, ParameterModifier[]? modifiers = null, MethodKind methodKind = MethodKind.Method)
        {
           
            if(target == null)
                throw new ArgumentNullException(nameof(target) + " is null");

            if (!target.GetType().IsCOMObject)
                throw new ArgumentException(nameof(target) + " is not a COM object");

            // There should be either no modifiers or exactly one modifier
            if (modifiers != null && modifiers.Length > 1)
                throw new ArgumentException();

            // Obtain IDispatch interface
            IDispatch disp = (IDispatch)target;

            // Retrieve DISPID
            uint dispId = GetDispID(disp, name, LOCALE_USER_DEFAULT);

            IntPtr pVariantArgArray = IntPtr.Zero;
            IntPtr pDispIDArray = IntPtr.Zero;

            int argCount = args.Length;
            int variantSize = Marshal.SizeOf<Variant>();
            object result;

            try
            {
                // Package arguments
                if (argCount > 0)
                {
                    pVariantArgArray = Marshal.AllocCoTaskMem(variantSize * argCount);
                    for (int i = 0; i < argCount; ++i)
                    {
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
                    // For property putters, the first DISPID argument needs to be DISPID_PROPERTYPUT
                    pVariantArgArray = Marshal.AllocCoTaskMem(variantSize * argCount);
                    Marshal.WriteInt32(pVariantArgArray, DISPID_PROPERTYPUT);

                    paramArray[0].namedArgCount = 1;
                    paramArray[0].namedArgDispIds = pVariantArgArray;
                }
                else
                {
                    // Otherwise, no named parameters are necessary
                    paramArray[0].namedArgCount = 0;
                    paramArray[0].namedArgDispIds = IntPtr.Zero;
                }

                // Make the call
                EXCEPINFO info = default(EXCEPINFO);
                uint err;
                InvokeFlags flags;

                if (methodKind == MethodKind.Method)
                    flags = InvokeFlags.DISPATCH_METHOD;
                else if (methodKind == MethodKind.PropertyPut)
                    flags = InvokeFlags.DISPATCH_PROPERTYPUT;
                else if (methodKind == MethodKind.PropertyPutRef)
                    flags = InvokeFlags.DISPATCH_PROPERTYPUTREF;
                else if (methodKind == MethodKind.PropertyGet)
                    flags = InvokeFlags.DISPATCH_PROPERTYGET;
                else
                    throw new ArgumentException("Invalid " + nameof(methodKind));

                if (disp == null)
                    throw new ArgumentNullException(nameof(target) + " like IDispatch is null");

                try
                {
                    disp.Invoke(
                        dispId,
                        IID_NULL,
                        LOCALE_USER_DEFAULT,
                        flags,
                        paramArray,
                        out result,
                        out info,
                        out err);

                }
                catch (Exception ex)
                {
                    if (ex.HResult != DISP_E_EXCEPTION)
                        throw;

                    Exception? realException = null;
                    string? realErrorMessage = null;

                    if (info.scode != 0)
                        realException = Marshal.GetExceptionForHR(info.scode, IntPtr.Zero);
                    else
                        realException = Marshal.GetExceptionForHR((int)info.code, IntPtr.Zero);

                    //--------------------//

                    if (info.strDescription != IntPtr.Zero)
                        realErrorMessage = Marshal.PtrToStringBSTR(info.strDescription);
                    // @TODO - find a way to return this to the caller

                    //--------------------//

                    if (realException == null && string.IsNullOrEmpty(realErrorMessage))
                        throw;

                    else if (realException == null && !string.IsNullOrEmpty(realErrorMessage))
                        throw new Exception(realErrorMessage);

                    else if (realException != null && string.IsNullOrEmpty(realErrorMessage))
                        throw realException;

                    else
                        throw new Exception(realErrorMessage, realException);
                }

                // Now back propagate the by-ref arguments
                for (int i = 0; i < argCount; ++i)
                {
                    // !! The arguments should be in REVERSED order!!
                    int actualIndex = (argCount - i - 1);

                    // If need to pass by ref, back propagate 
                    if (modifiers != null && modifiers[0][i])
                        args[i] = Marshal.GetObjectForNativeVariant(pVariantArgArray + (actualIndex * variantSize));

                }

                return result;
            }
            finally
            {
                // Free memory
                if (pVariantArgArray != IntPtr.Zero)
                {
                    for (int i = 0; i < argCount; ++i)
                        VariantClear(pVariantArgArray + i * variantSize);

                    Marshal.FreeCoTaskMem(pVariantArgArray);
                }

                if (pDispIDArray != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pDispIDArray);
            }

            
        }

        
        /// <summary>
        /// https://learn.microsoft.com/en-us/previous-versions/windows/desktop/automat/dispid-constants
        /// </summary>
        /// <param name="disp"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static uint GetDispID(IDispatch disp, string name, int lcid)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DISPID_UNKNOWN;

            uint[] dispid = new uint[1];
            disp.GetIDsOfNames(IID_NULL, new string[] { name }, 1, lcid, dispid);
            
            return dispid[0];
        }

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

        

        #endregion
    }
}
