using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Number : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new double CSValue => (double)_CSValue;

        protected Number()
        {
            _CSValue = 0;
            _JSValue = 0;
            _JSType = JSType.Number;
        }
    }
}
