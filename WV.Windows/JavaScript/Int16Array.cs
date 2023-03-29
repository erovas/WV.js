namespace WV.Windows.JavaScript
{
    internal class Int16Array : WV.JavaScript.Int16Array
    {
        public Int16Array(object raw, short[] csValue) :base() 
        {
            _JSValue = raw;
            _CSValue = csValue;
            _Stringified = csValue.ToString();
        }
    }
}