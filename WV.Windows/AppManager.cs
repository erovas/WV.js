using Microsoft.Web.WebView2.Core;
using System.IO;

namespace WV.Windows
{
    public static class AppManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static CoreWebView2Environment? Environment { get; set; }

        /// <summary>
        /// UserData development folder path
        /// </summary>
        public static string UserDataPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string SrcPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string PluginsPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string HostObjectName { get; }

        /// <summary>
        /// ["UID", PluginInstance]
        /// </summary>
        public static Dictionary<string, object> PluginInstaces { get; }
        
        /// <summary>
        ///  ["PluginName", PluginType]
        /// </summary>
        public static Dictionary<string, Type> PluginTypes { get; }

        /// <summary>
        /// 
        /// </summary>
        public static List<string> JScripts { get; }


        static AppManager()
        {
            PluginInstaces = new Dictionary<string, object>();
            PluginTypes = new Dictionary<string, Type>();
            JScripts = new List<string>();
            //HostObjectName = "-_-wv-_-";
            HostObjectName = "-_-" + Guid.NewGuid().ToString() + "-_-";

            string current = Directory.GetCurrentDirectory();
            SrcPath = current + "/Src";
            PluginsPath = current + "/Plugins";
            UserDataPath = current + "/UserData";
        }

    }
}