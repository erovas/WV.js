using System.Runtime.InteropServices;

namespace WV
{
    public class App
    {
        /// <summary>
        /// Application name
        /// </summary>
        public static string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        public static string Domain { get; }

        /// <summary>
        /// SDK Version
        /// </summary>
        public static string Version { get; }

        /// <summary>
        /// Platform name
        /// </summary>
        public static string Platform { get; }

        /// <summary>
        /// Application storage
        /// </summary>
        public static Dictionary<string, object?> Storage { get; }

        /// <summary>
        /// 
        /// </summary>
        public static string ConfigFilePath { get; }

        static App()
        {
            Name = "wv";
            Domain = Name.ToUpper() + ".js";
            Version = "1.0.0";

            string platform = OSPlatform.Windows.ToString();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                platform = OSPlatform.FreeBSD.ToString();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                platform = OSPlatform.Linux.ToString();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                platform = OSPlatform.OSX.ToString();

            Platform = platform;
            Storage = new Dictionary<string, object?>();
            ConfigFilePath = Directory.GetCurrentDirectory() + "/config.json";
        }
    }
}
