using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Uint8Array : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new byte[] CSValue => (byte[])_CSValue;

        protected Uint8Array()
        {
            _JSType = JSType.Uint8Array;
        }
    }
}
