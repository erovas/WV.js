using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WV.JavaScript;

namespace WV.Windows.JavaScript
{
    public class Int32Array : WV.JavaScript.Int32Array
    {
        public Int32Array(object raw, int[] csValue) :base() 
        {
            _JSValue = raw;
            _CSValue = csValue;
            _Stringified = csValue.ToString();
        }
    }
}
