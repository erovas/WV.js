namespace WV.Windows.JavaScript
{
    public class Uint8ClampedArray : WV.JavaScript.Uint8ClampedArray
    {
        public Uint8ClampedArray(object raw, byte[] csValue) : base() 
        {
            _JSValue = raw;
            _CSValue = csValue;
            _Stringified = csValue.ToString();
        }
    }
}