using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WV.JavaScript;

namespace WV.NotificationIcon.Windows
{
    internal static class Utils
    {
        public const string CLICK = "click";
        public const string DOUBLE_CLICK = "doubleclick";

        public static void RemoveInList(List<Function> list, Function? fn)
        {
            if (fn == null)
                return;

            Function? item = list.Where(x => x.JSValue == fn.JSValue).FirstOrDefault();

            if (item != null)
                list.Remove(item);
        }
    }
}
