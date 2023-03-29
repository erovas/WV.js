namespace WV.Windows.JavaScript
{
    public class Float32Array : WV.JavaScript.Float32Array
    {
        public Float32Array(object raw, float[] csValue) : base() 
        {
            _JSValue = raw;
            _CSValue = csValue;
            _Stringified = csValue.ToString();
        }
    }
}