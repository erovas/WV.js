using System.Collections.Immutable;

namespace WV.Windows.JavaScript
{
    public class Object : WV.JavaScript.Object
    {
        public Object(object raw, Dictionary<string, object> csValue, string stringified) : base() 
        { 
            _JSValue = raw;
            _CSValue = csValue.ToImmutableDictionary();
            _Stringified = stringified;
        }
    }
}