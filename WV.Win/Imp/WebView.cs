using Microsoft.Web.WebView2.Core;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using WV.Attributes;
using WV.Configs;
using WV.Core;
using WV.Core.Pluging;
using WV.Enums;
using WV.Interfaces;
using WV.Win.Classes;
using WV.Win.Scripts;
using WV.Win.Win32;
using WV.Win.Win32.Enums;
using WV.Win.Win32.Structs;

namespace WV.Win.Imp
{
    public sealed class WebView : WebViewCore
    {

        #region INTERNAL

        internal bool InternalIsMain => _IsMain;

        internal WebView InternalWebView => (WebView)WebView;

        internal Window InternalWindow => (Window)_Window!;

        internal Browser InternalBrowser => (Browser)_Browser!;

        internal Plugins InternalPlugins => (Plugins)_Plugins!;

        internal PrintManager InternalPrintManager => (PrintManager)_PrintManager!;

        

        #endregion

        //==========================================//

        #region CONSTRUCTORS

        public WebView(IPluginContext ctx, object? wvConfig = null) : base(ctx, wvConfig)
        {
        }

        protected override IWindow CreateWindow(IPluginContext context, WindowConfig windowConfig)
        {
            return new Window(Utils.CreateContext(this, Logger, nameof(Window), Logger.Source), windowConfig);
        }

        protected override IBrowser CreateBrowser(IPluginContext context, BrowserConfig browserConfig)
        {
            return new Browser(Utils.CreateContext(this, Logger, nameof(Browser), Logger.Source), browserConfig);
        }

        protected override IPrintManager CreatePrintManager(IPluginContext context, PrintManagerConfig printManagerConfig)
        {
            return new PrintManager(Utils.CreateContext(this, Logger, nameof(PrintManager), Logger.Source), printManagerConfig);
        }

        protected override IPlugins CreatePlugins(IPluginContext context, PluginsConfig pluginsConfig)
        {
            return new Plugins(Utils.CreateContext(this, Logger, nameof(Plugins), Logger.Source), pluginsConfig);
        }

        protected override void Initialize(IPluginContext context, WebViewConfig wvConfig)
        {
            // Bucle de mensajes, que hacen funcionar la ventana
            Utils32.RunMessageLoop();
        }

        protected override WebViewConfig GetWebViewConfigFromJSObject(object jsObject)
        {
            var browserConfig = new BrowserConfig();
            object? objBrowser = ExtractProperty(jsObject, nameof(Browser));

            if(objBrowser != null)
            {
                var language = ExtractValue<string>(objBrowser, "language");
                var directory = ExtractValue<string>(objBrowser, "directory");
                var uri = ExtractValue<string>(objBrowser, "uri");
                var hotReload = ExtractValue<bool>(objBrowser, "hotreload");
                var acceleratorKeys = ExtractValue<bool>(objBrowser, "acceleratorKeys");
                var swipeNavigation = ExtractValue<bool>(objBrowser, "swipeNavigation");
                var zoomFactor = ExtractValue<double>(objBrowser, "zoomFactor");
                var muted = ExtractValue<bool>(objBrowser, "muted");
                var statusBarEnabled = ExtractValue<bool>(objBrowser, "statusBarEnabled");
                var colorScheme = ExtractValue<BrowserColorScheme>(objBrowser, "colorScheme");

                browserConfig = new BrowserConfig()
                {
                    Language = language,
                    Directory = directory!,
                    Uri = uri!,
                    HotReload = hotReload,
                    AcceleratorKeys = acceleratorKeys,
                    SwipeNavigation = swipeNavigation,
                    ZoomFactor = zoomFactor,
                    Muted = muted,
                    StatusBarEnabled = statusBarEnabled,
                    ColorScheme = colorScheme
                };
            }

            var pluginsConfig = new PluginsConfig();
            object? objPlugins = ExtractProperty(jsObject, nameof(Plugins));

            if(objPlugins != null)
            {
                var directory = ExtractValue<string>(objPlugins, "directory");
                var load = JsonSerializer.Deserialize<string[]>(ExtractValue<string>(objPlugins, "load") ?? "[]")!;
                pluginsConfig = new PluginsConfig()
                {
                    Directory = directory!,
                    Load = load
                };
            }

            var windowConfig = new WindowConfig();
            object? objWindow = ExtractProperty(jsObject, nameof(Window));

            if (objWindow != null)
            {
                var title = ExtractValue<string>(objWindow, "Title");
                var state = ExtractValue<WindowState>(objWindow, "State");
                var centerScreen = ExtractValue<bool>(objWindow, "centerScreen");
                var rect = new RectConfig();
                var objRect = ExtractProperty(objWindow, "Rect");

                if(objRect != null)
                {
                    var x = ExtractValue<int>(objRect, "x");
                    var y = ExtractValue<int>(objRect, "y");
                    var width = ExtractValue<int>(objRect, "width");
                    var height = ExtractValue<int>(objRect, "height");
                    var maxWidth = ExtractValue<int>(objRect, "maxWidth");
                    var maxHeight = ExtractValue<int>(objRect, "maxHeight");
                    var minWidth = ExtractValue<int>(objRect, "minWidth");
                    var minHeight = ExtractValue<int>(objRect, "minHeight");

                    rect = new RectConfig(x, y, width, height, maxWidth, maxHeight, minWidth, minHeight);
                }

                windowConfig = new WindowConfig()
                {
                    Title = title!,
                    State = state,
                    CenterScreen = centerScreen,
                    Rect = rect
                };
            }

            var printManagerConfig = new PrintManagerConfig();
            object? objPrintManager = ExtractProperty(jsObject, nameof(PrintManager));

            if (objPrintManager != null) 
            {
                var marginBottom = ExtractValue<double>(objPrintManager, "marginBottom");
                var marginLeft = ExtractValue<double>(objPrintManager, "MarginLeft");
                var marginRight = ExtractValue<double>(objPrintManager, "MarginRight");
                var marginTop = ExtractValue<double>(objPrintManager, "MarginTop");
                var pageWidth = ExtractValue<double>(objPrintManager, "PageWidth");
                var pageHeight = ExtractValue<double>(objPrintManager, "PageHeight");
                var scaleFactor = ExtractValue<double>(objPrintManager, "ScaleFactor");

                var printBackgrounds = ExtractValue<bool>(objPrintManager, "PrintBackgrounds");
                var printSelectionOnly = ExtractValue<bool>(objPrintManager, "PrintSelectionOnly");
                var printHeaderAndFooter = ExtractValue<bool>(objPrintManager, "PrintHeaderAndFooter");

                var footerUri = ExtractValue<string>(objPrintManager, "FooterUri");
                var pagesPerSide = ExtractValue<int>(objPrintManager, "PagesPerSide");

                var colorMode = ExtractValue<PrintColorMode>(objPrintManager, "ColorMode");
                var collation = ExtractValue<PrintCollation>(objPrintManager, "Collation");
                var orientation = ExtractValue<PrintOrientation>(objPrintManager, "Orientation");

                printManagerConfig = new PrintManagerConfig(marginBottom, marginLeft, marginRight, marginTop, pageWidth, pageHeight, scaleFactor, printBackgrounds, printSelectionOnly, printHeaderAndFooter, footerUri, pagesPerSide, colorMode, collation, orientation);
            }

            var config = new WebViewConfig()
            {
                Browser = browserConfig,
                Plugins = pluginsConfig,
                Window = windowConfig,
                PrintManager = printManagerConfig
            };

            return config;
        }

        #endregion

        //==========================================//

        #region LIFETIME



        protected override void DisposeCore(bool disposing)
        {
            //this.CleanMyself();
            
        }

        #endregion

        //==========================================//

        private static T? ExtractValue<T>(object obj, string name)
        {
            var ex = ExtractProperty(obj, name);

            if (ex == null)
                return default;

            if (typeof(T).IsEnum)
                ex = Enum.Parse(typeof(T), ex.ToString()!, ignoreCase: true);
            
            return (T)ex;
        }

        private static object? ExtractProperty(object obj, string name)
        {
            foreach (var combo in CasePermutations(name))
            {
                var ex = Invoke.Invoker.PropertyGet(obj, combo);
                if(ex != null)
                    return ex;
            }
                
            return null;
        }

        private static string[] CasePermutations(string s)
        {
            int n = s.Length;
            int total = 1 << n; // 2^n
            var list = new List<string>();

            for (int mask = 0; mask < total; mask++)
            {
                var chars = s.ToCharArray();
                for (int i = 0; i < n; i++)
                {
                    if ((mask & (1 << i)) != 0)
                        chars[i] = char.ToUpperInvariant(chars[i]);
                    else
                        chars[i] = char.ToLowerInvariant(chars[i]);
                }
                list.Add(new string(chars));
            }

            return list.ToArray();
        }

        public void ASD(object asd)
        {
            // { a: "texto", b: { ba: "sub texto", bb: { bba: "sub sub texto" } } }
            object? asd_get = Invoke.Invoker.PropertyGet(asd, "a");  // Texto   = "texto"
            object? asd_get2 = Invoke.Invoker.PropertyGet(asd, "b"); // Sub objeto

            object? asd_get3 = Invoke.Invoker.PropertyGet(asd_get2, "ba"); // Texto del sub objeto   = "sub texto"
            object? asd_get4 = Invoke.Invoker.PropertyGet(asd_get2, "bb"); // Sub sub objeto
            object? asd_get5 = Invoke.Invoker.PropertyGet(asd_get4, "bba"); // Texto del Sub sub objeto  == "sub sub texto"
        }

        public object getVer(int major, int minor, int build, int revision)
        {
            // Version NO se deja pasar a JS
            var asd = new Version(major, minor, build, revision);
            var objs = new List<object>();
            objs.Add(asd);
            objs.Add(123);
            //return objs.ToArray();
            return new
            {
                Id = "com.example.test",
                Name = "Test",
                Version = "1.0.0"
            };
        }

       
    }
}