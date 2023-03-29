using System.Numerics;

namespace WV.Windows.JavaScript
{
    public class BigInt64Array : WV.JavaScript.BigInt64Array
    {
        public BigInt64Array(object raw, BigInteger[] csValue) : base() 
        {
            _JSValue = raw;
            _CSValue = csValue;
            _Stringified = csValue.ToString();
        }
    }
}