using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using WV.JavaScript.Enums;
using WV.WebView;
using WV.Windows.JavaScript;

namespace WV.Windows.HostObject
{
    public abstract class HOBase : Plugin
    {
        #region PRIVATE PROPERTIES

        [ComVisible(false)]
        public bool InnerEnablePlugins { get; set; }
        protected bool InnerIsFrame { get; set; }

        #endregion

        #region PUBLIC PROPERTIES

        public bool IsPluginsEnabled => this.InnerEnablePlugins;
        public string Platform => App.Platform;
        public string Version => App.Version;
        public bool IsMainWebView => this.WebView.IsMain;
        public bool IsFrame => this.InnerIsFrame;
        public IScreen CurrentScreen => this.WebView.Screen;
        public IScreen[] Screens
        {
            get
            {
                Screen[] screens = Screen.AllScreens;
                List<Webview.Screen> result = new();

                foreach (var item in screens)
                    result.Add(new Webview.Screen(item));

                return result.ToArray();
            }
        }

        #endregion

        protected HOBase(IWebView webView) : base(webView)
        {
        }

        /// <summary>
        /// Execute constructor
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="jsArgs"></param>
        /// <returns></returns>
        public object EC(string pluginName, params object[] jsArgs)
        {
            if (!this.IsPluginsEnabled)
                return "Plugins are disabled";

            if (!AppManager.PluginTypes.ContainsKey(pluginName))
                return "Plugin [" + pluginName + "] no exists";

            Type pluginType = AppManager.PluginTypes[pluginName];
            Plugin pluginObj;
            object? pluginInstance;

            try
            {
                List<object> Args = jsArgs.ToList();
                Args.Insert(0, this.WebView);
                pluginInstance = Activator.CreateInstance(pluginType, Args.ToArray());

                if (pluginInstance == null)
                    return "Impossible to build plugin instance [" + pluginName + "]";

                pluginObj = (Plugin)pluginInstance;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return pluginInstance;
        }

        /// <summary>
        /// Execute transform
        /// </summary>
        /// <param name="jsTypeName"></param>
        /// <param name="raw"></param>
        /// <param name="csParsed"></param>
        /// <param name="stringified"></param>
        /// <returns></returns>
        public object TF(string jsTypeName, object raw, object csParsed, string stringified)
        {
            JSType jsType = JSType.Custom;

            try
            {
                jsType = (JSType)Enum.Parse(typeof(JSType), jsTypeName);
            }
            catch (Exception ex) 
            {
                return ex.Message;
            }

            switch (jsType)
            {
                case JSType.@null:
                    return new Null();
                case JSType.undefined:
                    return new Undefined();

                case JSType.Boolean:
                    return new JavaScript.Boolean((bool)raw);

                case JSType.Number:
                    return new Number((double)raw);

                case JSType.String:
                    return new JavaScript.String(raw + "");

                //case JSType.Object:
                //    break;

                case JSType.Function:
                    return new Function(raw, stringified);

                case JSType.AsyncFunction:
                    return new Function(raw, stringified, true);

                case JSType.Array:
                    return new JavaScript.Array((object[])raw, (object[])csParsed, stringified);

                case JSType.BigInt:
                    return new BigInt(raw, BigInteger.Parse(csParsed + ""), csParsed + "");

                case JSType.Symbol:
                    return new Symbol(raw, stringified);

                case JSType.Proxy:
                    return new Proxy(raw, stringified);

                case JSType.Date:
                    return new Date((DateTime)raw, DateTime.ParseExact(csParsed + "", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToUniversalTime(), stringified);

                case JSType.Custom:
                    return new Custom(raw, stringified);

                case JSType.Uint8Array:
                    return new Uint8Array(raw, ((object[])csParsed).Cast<byte>().ToArray());

                case JSType.Uint8ClampedArray:
                    return new Uint8ClampedArray(raw, ((object[])csParsed).Cast<byte>().ToArray());

                case JSType.Int8Array:
                    return new Int8Array(raw, ((object[])csParsed).Cast<sbyte>().ToArray());

                case JSType.Uint16Array:
                    return new Uint16Array(raw, ((object[])csParsed).Cast<ushort>().ToArray());

                case JSType.Int16Array:
                    return new Int16Array(raw, ((object[])csParsed).Cast<short>().ToArray());

                case JSType.Uint32Array:
                    return new Uint32Array(raw, ((object[])csParsed).Cast<uint>().ToArray());

                case JSType.Int32Array:
                    return new Int32Array(raw, ((object[])csParsed).Cast<int>().ToArray());

                case JSType.Float32Array:
                    return new Float32Array(raw, ((object[])csParsed).Cast<float>().ToArray());

                case JSType.Float64Array:
                    return new Float64Array(raw, ((object[])csParsed).Cast<double>().ToArray());

                case JSType.BigUint64Array:
                    return new BigUint64Array(raw, ((object[])csParsed).Cast<string>().Select(BigInteger.Parse).ToArray());

                case JSType.BigInt64Array:
                    return new BigInt64Array(raw, ((object[])csParsed).Cast<string>().Select(BigInteger.Parse).ToArray());

                default:
                    return "JSType [" + jsTypeName + "] not exists";
            }
        }

        /// <summary>
        /// Execute transform for js plain object
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public object TFO(object raw, string stringified, params object[] args)
        {
            Dictionary<string, object> csValue = new();

            for (int i = 0; i < args.Length; i += 2)
                csValue[args[i] + ""] = args[i + 1];

            return new JavaScript.Object(raw, csValue, stringified);
        }

        /// <summary>
        /// Shutdown application
        /// </summary>
        public void Shutdown()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Restart application
        /// </summary>
        public void Restart()
        {
            Application.Restart();
            Shutdown();
        }

    }
}