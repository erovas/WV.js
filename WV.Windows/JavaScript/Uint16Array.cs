namespace WV.Windows.JavaScript
{
    public class Uint16Array : WV.JavaScript.Uint16Array
    {
        public Uint16Array(object raw, ushort[] csValue) : base() 
        {
            _JSValue = raw;
            _CSValue = csValue;
            _Stringified = csValue.ToString();
        }
    }
}