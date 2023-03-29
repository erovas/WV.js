using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingThings
{
    public abstract class ClaseDerivadaAbstracta : ClaseAbstracta
    {
        public new int Value 
        { 
            get => (int)_Value; 
            set=> _Value = value; 
        }

        public ClaseDerivadaAbstracta() 
        {
            _Value = 0;
        }
    }
}
