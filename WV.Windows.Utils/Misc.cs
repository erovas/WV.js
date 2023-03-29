using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WV.Windows.Utils
{
    public static class Misc
    {
        public static string ParamsToJson(params object[] args)
        {
            return ArrayToJson(args);
        }

        public static string ArrayToJson(object[] args)
        {
            Dictionary<string, object> rawJson = new();

            for (int i = 0; i < args.Length; i += 2)
                rawJson[args[i] + ""] = args[i + 1];

            return JsonSerializer.Serialize(rawJson);
        }

        public static string StringReplace(string text, string mark, params string[] args)
        {
            for (int i = 0; i < args.Length; i += 2)
                text = text.Replace(mark + args[i] + mark, args[i + 1]);

            return text;
        }
    }
}
