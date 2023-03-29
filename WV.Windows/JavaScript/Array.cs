namespace WV.Windows.JavaScript
{
    public class Array : WV.JavaScript.Array
    {
        public Array(object[] raw, object[] csValue, string stringified) : base() 
        {
            _JSValue = raw;
            _CSValue = csValue;
            _Stringified = stringified;
        }
    }
}