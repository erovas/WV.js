namespace WV.Windows.JavaScript
{
    public class Float64Array : WV.JavaScript.Float64Array
    {
        public Float64Array(object raw, double[] csValue) : base() 
        {
            _JSValue = raw;
            _CSValue = csValue;
            _Stringified = csValue.ToString();
        }
    }
}