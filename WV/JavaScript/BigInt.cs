using System.Numerics;
using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class BigInt : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new BigInteger CSValue => (BigInteger)_CSValue;

        protected BigInt() 
        {
            _CSValue = 0;
            _JSType = JSType.BigInt;
        }
    }
}
