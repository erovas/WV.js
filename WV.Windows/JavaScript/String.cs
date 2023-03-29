namespace WV.Windows.JavaScript
{
    public class String : WV.JavaScript.String
    {
        public String(string value) : base()
        { 
            _CSValue = value;
            _JSValue = value;
            _Stringified = value;
        }
    }
}