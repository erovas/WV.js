using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Float32Array : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new float[] CSValue => (float[])_CSValue;

        protected Float32Array() 
        {
            _JSType = JSType.Float32Array;
        }
    }
}
