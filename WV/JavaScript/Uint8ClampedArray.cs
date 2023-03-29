using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Uint8ClampedArray : Value
    {
        /// <summary>
        /// C# parse value
        /// </summary>
        public new byte[] CSValue => (byte[])_CSValue;

        protected Uint8ClampedArray()
        {
            _JSType = JSType.Uint8ClampedArray;
        }
    }
}
