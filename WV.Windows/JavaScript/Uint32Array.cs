namespace WV.Windows.JavaScript
{
    public class Uint32Array : WV.JavaScript.Uint32Array
    {
        public Uint32Array(object raw, uint[] csValue) : base() 
        {
            _JSValue = raw;
            _CSValue = csValue;
            _Stringified = csValue.ToString();
        }
    }
}