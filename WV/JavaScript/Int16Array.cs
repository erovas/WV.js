using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Int16Array : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new short[] CSValue => (short[])_CSValue;

        protected Int16Array() 
        {
            _JSType = JSType.Int16Array;
        }
    }
}
