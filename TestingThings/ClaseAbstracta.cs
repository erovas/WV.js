using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingThings
{
    public abstract class ClaseAbstracta
    {
        protected object _Value;

        public object Value 
        { 
            get => _Value; 
            set => _Value = value; 
        }

        public string Name { get; set; }
    }
}
