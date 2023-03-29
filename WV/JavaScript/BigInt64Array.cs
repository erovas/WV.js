using System.Numerics;
using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class BigInt64Array : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new BigInteger[] CSValue => (BigInteger[])_CSValue;

        protected BigInt64Array() 
        {
            _JSType = JSType.BigInt64Array;
        }
    }
}
