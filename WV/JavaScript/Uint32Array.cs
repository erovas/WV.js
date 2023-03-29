using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Uint32Array : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new uint[] CSValue => (uint[])_CSValue;

        protected Uint32Array()
        {
            _JSType = JSType.Uint32Array;
        }
    }
}
