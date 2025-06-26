using System.Runtime.InteropServices;

namespace WV.Win.Invoke.Structs
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Variant
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
}