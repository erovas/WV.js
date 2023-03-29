using System.Numerics;
using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class BigUint64Array : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new BigInteger[] CSValue => (BigInteger[])_CSValue;

        protected BigUint64Array() 
        {
            _JSType = JSType.BigUint64Array;
        }
    }
}
