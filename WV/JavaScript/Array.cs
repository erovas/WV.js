using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Array : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new object[] CSValue => (object[])_CSValue;

        protected Array() 
        {
            _JSType = JSType.Array;
        }
    }
}
