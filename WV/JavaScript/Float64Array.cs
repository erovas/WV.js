
using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Float64Array : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new double[] CSValue => (double[])_CSValue;

        protected Float64Array()
        {
            _JSType = JSType.Float64Array;
        }
    }
}
