using System.Runtime.InteropServices;
using System.Collections.Immutable;
using WV.Interfaces;
using WV.Enums;

namespace WV
{
    public abstract class App
    {
        public static class Logger
        {
            public const string Directory = "logs";

            public const string Source = "WV";

            public const bool Enabled = true;

            public const LogLevel MinimumLevel = LogLevel.Info;

            public const long MaxFileBytes = 5 * 1024 * 1024;  // 5MB
        }

        public static class Plugins
        {
            public const string Directory = "plugins";
        }

        public static class Window
        {
            public const string Title = "WV.js";
            public const WindowState State = WindowState.None;
            public const bool CenterScreen = false;

            public static class Rect
            {
                public const int Width = 800;
                public const int Height = 600;
                public const int MinWidth = 136;
                public const int MinHeight = 39;
                public const int MaxWidth = int.MaxValue;
                public const int MaxHeight = int.MaxValue;
            }
        }

        public static class Browser
        {
            public const string Directory = "src";

            public const string? Language = null;

            public const string Uri = "index.html";

            public const bool HotReload = false;

            public const bool AcceleratorKeys = true;

            public const bool SwipeNavigation = false;

            public const double MinZoomFactor = 0.25;

            public const double ZoomFactor = 1;

            public const double MaxZoomFactor = 5;

            public const bool Muted = false;

            public const bool StatusBarEnabled = true;

            public const BrowserColorScheme ColorScheme = BrowserColorScheme.Auto;

        }

        public static class PrintManager 
        {
            public const double MarginBottom = 1;

            public const double MarginLeft = 1;

            public const double MarginRight = 1;

            public const double MarginTop = 1;

            public const double MinPageWidth = 0.1;

            public const double PageWidth = 21;

            public const double MaxPageWidth  = 101.6;

            public const double MinPageHeight = 0.1;

            public const double PageHeight = 29.7;

            public const double MaxPageHeight = 142.24;

            public const double MinScaleFactor = 0.1;

            public const double ScaleFactor = 1;

            public const double MaxScaleFactor = 2;

            public const bool PrintBackgrounds = false;

            public const bool PrintSelectionOnly = false;

            public const bool PrintHeaderAndFooter = false;

            public const string? FooterUri = null;

            public const int PagesPerSide = 1;

            public static readonly ImmutableArray<int> PagesPerSideValues = ImmutableArray.Create(1, 2, 4, 6, 9, 16);

            public const PrintColorMode ColorMode = PrintColorMode.Default;

            public const PrintCollation Collation = PrintCollation.Default;

            public const PrintOrientation Orientation = PrintOrientation.Portrait;
    }

        public static class Delegates
        {
            public delegate void WVEventHandler(IWebView sender);
            public delegate void WVEventHandler<in T1>(IWebView sender, T1 arg);
            public delegate void WVEventHandler<in T1, in T2>(IWebView sender, T1 arg1, T2 arg2);
            public delegate void WVEventHandler<in T1, in T2, in T3>(IWebView sender, T1 arg1, T2 arg2, T3 arg3);
            public delegate void WVEventHandler<in T1, in T2, in T3, in T4>(IWebView sender, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
            public delegate void WVRawEventHandler(IWebView sender, object[] args, ref bool handled);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly string Directory;

        /// <summary>
        /// ["mySharedVar", "MySharedValue"]
        /// </summary>
        public static readonly Dictionary<string, object?> DataStorage;

        /// <summary>
        /// Platform name
        /// </summary>
        public static readonly string Platform;


        private static bool _Initialized;

        /// <summary>
        /// Gets a boolean indicating that the application has started.
        /// </summary>
        public static bool IsInitialized 
        {
            get 
            {
                if(_Initialized)
                    return true;

                _Initialized = true;
                return false; 
            }
        }

        /// <summary>
        /// Gets a boolean that indicates whether the application is in debug mode
        /// </summary>
        public static readonly bool IsDebugging;

        static App()
        {
            Directory = AppContext.BaseDirectory;
            DataStorage = new();

            string platform = OSPlatform.Windows.ToString();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                platform = OSPlatform.FreeBSD.ToString();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                platform = OSPlatform.Linux.ToString();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                platform = OSPlatform.OSX.ToString();

            Platform = platform;
            IsDebugging = System.Diagnostics.Debugger.IsAttached;
        }
    }
}