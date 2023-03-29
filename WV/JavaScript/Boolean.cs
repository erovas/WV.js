using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Boolean : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new bool CSValue => (bool)_CSValue;

        protected Boolean() 
        {
            _JSType = JSType.Boolean;
            _CSValue = false;
            _JSValue = false;
        }
    }
}
