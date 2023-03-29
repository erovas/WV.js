namespace WV.Windows.JavaScript
{
    public class Proxy : WV.JavaScript.Proxy
    {
        public Proxy(object raw, string stringified) : base() 
        { 
            _JSValue = raw;
            _Stringified = stringified;
        }
    }
}