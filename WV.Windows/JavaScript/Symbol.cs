namespace WV.Windows.JavaScript
{
    public class Symbol : WV.JavaScript.Symbol
    {
        public Symbol(object raw, string stringified) : base() 
        {
            _JSValue = raw;
            _Stringified = stringified;
        }
    }
}