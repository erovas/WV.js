namespace WV.Windows.JavaScript
{
    public class Custom : WV.JavaScript.Custom
    {
        public Custom(object raw, string stringified) : base() 
        { 
            _JSValue = raw;
            _Stringified = stringified;
        }
    }
}