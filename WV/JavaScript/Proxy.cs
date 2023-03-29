using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Proxy : Value
    {
        protected Proxy() 
        {
            _JSType = JSType.Proxy;
        }
    }
}
