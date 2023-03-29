using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Undefined : Value
    {
        protected Undefined()
        {
            _JSType = JSType.undefined;
            _Stringified = _JSType.ToString();
        }
    }
}