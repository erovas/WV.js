using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Int8Array : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new sbyte[] CSValue => (sbyte[])_CSValue;

        protected Int8Array() 
        {
            _JSType = JSType.Int8Array;
        }
    }
}
