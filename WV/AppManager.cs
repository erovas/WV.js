using System.Runtime.InteropServices;
using WV.Interfaces;

namespace WV
{
    public static class AppManager
    {
        #region Delegates

        public delegate void WVEventHandler(IWebView sender);
        public delegate void WVEventHandler<T1>(IWebView sender, T1 arg);
        public delegate void WVEventHandler<T1, T2>(IWebView sender, T1 arg1, T2 arg2);
        public delegate void WVEventHandler<T1, T2, T3>(IWebView sender, T1 arg1, T2 arg2, T3 arg3);
        public delegate void WVEventHandler<T1, T2, T3, T4>(IWebView sender, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        public delegate void WVSysEventHandler(IWebView sender, object[] args, ref bool handled);

        #endregion

        #region Constants

        public const int MaxWindowWidth = int.MaxValue;
        public const int MaxWindowHeight = int.MaxValue;
        public const int MinWindowWidth = 136;
        public const int MinWindowHeight = 39;
        public const string Domain = "WV.js";

        #endregion

        #region Properties

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
        /// ["mySharedVar", "MySharedValue"]
        /// </summary>
        public static Dictionary<string, object?> DataStorage { get; }

        /// <summary>
        /// Platform name
        /// </summary>
        public static string Platform { get; }

        /// <summary>
        /// Gets a boolean that indicates whether the application is in debug mode
        /// </summary>
        public static bool IsDebugging { get; }

        /// <summary>
        /// Gets a boolean indicating that the application has started
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        public static void Initialized()
        {
            IsInitialized = true;
        }

        #endregion

        static AppManager()
        {
            IsDebugging = System.Diagnostics.Debugger.IsAttached;
            DataStorage = new Dictionary<string, object?>();
      
            string current = Directory.GetCurrentDirectory();
            SrcPath = current + "/src";
            PluginsPath = current + "/plugins";
            UserDataPath = current + "/userdata";
            //UserDataPath = Path.GetTempPath() + "wvjs_userdata";

            if(!Directory.Exists(SrcPath))
                Directory.CreateDirectory(SrcPath);

            if (!Directory.Exists(PluginsPath))
                Directory.CreateDirectory(PluginsPath);

            string platform = OSPlatform.Windows.ToString();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                platform = OSPlatform.FreeBSD.ToString();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                platform = OSPlatform.Linux.ToString();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                platform = OSPlatform.OSX.ToString();

            Platform = platform;
        }
    }
}