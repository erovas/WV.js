namespace WV.Windows.JavaScript
{
    public class Uint8Array : WV.JavaScript.Uint8Array
    {
        public Uint8Array(object raw, byte[] csvalue) : base() 
        { 
            _JSValue = raw;
            _CSValue = csvalue;
            _Stringified = csvalue.ToString();
        }
    }
}