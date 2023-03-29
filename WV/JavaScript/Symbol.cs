using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Symbol : Value
    {
        protected Symbol()
        {
            _JSType = JSType.Symbol;
        }
    }
}
