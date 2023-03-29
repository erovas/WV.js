namespace WV.Windows.JavaScript
{
    public class Int8Array : WV.JavaScript.Int8Array
    {
        public Int8Array(object raw, sbyte[] csValue) : base() 
        {
            _JSValue = raw;
            _CSValue = csValue;
            _Stringified = csValue.ToString();
        }
    }
}