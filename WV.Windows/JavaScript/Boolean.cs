namespace WV.Windows.JavaScript
{
    public class Boolean : WV.JavaScript.Boolean
    {
        public Boolean(bool value) : base()
        { 
            _CSValue = value;
            _JSValue = value;
            _Stringified = value.ToString().ToLower();
        }
    }
}
