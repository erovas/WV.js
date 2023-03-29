using System.Numerics;

namespace WV.Windows.JavaScript
{
    public class BigUint64Array : WV.JavaScript.BigUint64Array
    {
        public BigUint64Array(object raw, BigInteger[] csValue) : base() 
        {
            _JSValue = raw;
            _CSValue = csValue;
            _Stringified = csValue.ToString();
        }
    }
}