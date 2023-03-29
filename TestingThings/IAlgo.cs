using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestingThings
{
    public interface IAlgo
    {

        string Name { get; set; }
        string Description { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        UriKind tipo { get; set; }

    }
}
