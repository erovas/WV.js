using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingThings
{
    public interface IGenerico<T>
    {
        public T Value { get; set; }
        public string Name { get; set; }
    }
}
