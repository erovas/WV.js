using System.Collections.Immutable;
using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Object : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new ImmutableDictionary<string, object> CSValue => (ImmutableDictionary<string, object>)_CSValue;

        protected Object() 
        {
            _JSType = JSType.Object;
        }
    }
}
