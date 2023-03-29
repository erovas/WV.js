using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Uint16Array : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new ushort[] CSValue => (ushort[])_CSValue;

        protected Uint16Array() 
        {
            _JSType = JSType.Uint16Array;
        }
    }
}
