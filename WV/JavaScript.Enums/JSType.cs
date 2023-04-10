using System.Text.Json.Serialization;

namespace WV.JavaScript.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum JSType
    {
        /// <summary>
        /// null
        /// </summary>
        @null = 0,

        /// <summary>
        /// null - There is not equivalent
        /// </summary>
        undefined,

        /// <summary>
        /// dynamic || Dictionary<string, object>
        /// </summary>
        Object,

        /// <summary>
        /// null - There is not equivalent
        /// </summary>
        Function,

        /// <summary>
        /// null - There is not equivalent
        /// </summary>
        AsyncFunction,

        /// <summary>
        /// bool == Boolean
        /// </summary>
        Boolean,

        /// <summary>
        /// int == Int32 || double == Double
        /// </summary>
        Number,

        /// <summary>
        /// string == String
        /// </summary>
        String,

        /// <summary>
        /// object[] = Object[]
        /// </summary>
        Array,

        /// <summary>
        /// BigInteger
        /// </summary>
        BigInt,

        /// <summary>
        /// null - There is not equivalent
        /// </summary>
        Symbol,

        /// <summary>
        /// object - There is not equivalent
        /// </summary>
        Proxy,

        /// <summary>
        /// DateTime => "MM/DD/YYYY hh:mm:ss"
        /// </summary>
        Date,

        /// <summary>
        /// object - There is not equivalent
        /// </summary>
        Custom,



        /// <summary>
        /// byte[] == Byte[]
        /// </summary>
        Uint8Array,

        /// <summary>
        /// byte[] == Byte[]
        /// </summary>
        Uint8ClampedArray,

        /// <summary>
        /// sbyte[] == SByte[]
        /// </summary>
        Int8Array,

        /// <summary>
        /// ushort[] == UInt16[]
        /// </summary>
        Uint16Array,

        /// <summary>
        /// short[] == Int16[]
        /// </summary>
        Int16Array,

        /// <summary>
        /// uint[] == UInt32[]
        /// </summary>
        Uint32Array,

        /// <summary>
        /// int[] == Int32[]
        /// </summary>
        Int32Array,

        /// <summary>
        /// float[] == Single[]
        /// </summary>
        Float32Array,

        /// <summary>
        /// double[] == Double[]
        /// </summary>
        Float64Array,

        /// <summary>
        /// ulong[] == UInt64[]
        /// </summary>
        BigUint64Array,

        /// <summary>
        /// long[] == Int64[]
        /// </summary>
        BigInt64Array
    }
}
