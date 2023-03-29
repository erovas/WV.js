namespace WV.Windows.JavaScript
{
    public class Date : WV.JavaScript.Date
    {
        public Date(object raw, DateTime csValue, string stringified) : base() 
        { 
            _JSValue = raw;
            _CSValue = csValue;
            _Stringified = stringified;
        }
    }
}