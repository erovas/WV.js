using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Value
    {
        protected Type? _CSType;
        protected JSType _JSType = JSType.@null;
        protected object? _CSValue;
        protected object? _JSValue;
        protected string _Stringified = JSType.@null.ToString();

        public Type? CSType => _CSType;
        public JSType JSType => _JSType;
        public object? CSValue => _CSValue;
        public object? JSValue => _JSValue;
        public string Stringified => _Stringified;
    }
}
