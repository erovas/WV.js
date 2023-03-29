using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Date : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new DateTime CSValue => (DateTime)_CSValue;

        protected Date() 
        {
            _JSType = JSType.Date;
        }
    }
}
