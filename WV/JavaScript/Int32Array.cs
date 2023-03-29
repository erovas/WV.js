using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Int32Array : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new int[] CSValue => (int[])_CSValue;

        protected Int32Array()
        {
            _JSType = JSType.Int32Array;
        }
    }
}
