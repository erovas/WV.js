using System.Numerics;

namespace WV.Windows.JavaScript
{
    public class BigInt : WV.JavaScript.BigInt
    {
        public BigInt(object raw, BigInteger csValue, string stringified) : base() 
        {
            _CSValue = csValue;
            _JSValue = raw;
            _Stringified = stringified;
        }
    }
}