using System.Collections.Immutable;
using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Custom : Value
    {
        protected string _JSClassName = "Nameless";

        /// <summary>
        /// C# parse value
        /// </summary>
        public new ImmutableDictionary<string, object> CSValue => (ImmutableDictionary<string, object>)_CSValue;

        /// <summary>
        /// JS object class name
        /// </summary>
        public string JSClassName => _JSClassName;

        protected Custom() 
        {
            _JSType = JSType.Custom;
        }
    }
}
