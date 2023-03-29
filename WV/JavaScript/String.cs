using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class String : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new string CSValue => _CSValue + "";

        protected String()
        {
            _CSValue = string.Empty;
            _JSType = JSType.String;
        }
    }
}
