using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using WV.JavaScript.Enums;
using WV.WebView;
using WV.Windows.JavaScript;

namespace WV.Windows.HostObject
{
    public abstract class HOBase : IDisposable
    {
        #region PRIVATE PROPERTIES

        [ComVisible(false)]
        public bool InnerEnablePlugins { get; set; }

        [ComVisible(false)]
        public bool Disposed { get; private set; }

        protected List<string> InnerUIDS { get; }

        private IWebView InnerWebView { get; }

        private bool InnerAddToGlobal { get; }

        #endregion

        public bool EnablePlugins => this.InnerEnablePlugins;
        public string UID => this.InnerWebView.UID;

        public object Screens
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

        protected HOBase(IWebView webView, bool addToGlobal) 
        {
            this.InnerUIDS = new List<string>();
            this.InnerWebView = webView;
            this.InnerAddToGlobal = addToGlobal;
        }

        /// <summary>
        /// Execute constructor
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="jsArgs"></param>
        /// <returns></returns>
        public object EC(string pluginName, params object[] jsArgs)
        {
            if (!this.EnablePlugins)
                return "Plugins are disabled";

            if (!AppManager.PluginTypes.ContainsKey(pluginName))
                return "Plugin [" + pluginName + "] no exists";

            Type pluginType = AppManager.PluginTypes[pluginName];
            Plugin pluginObj;
            object? pluginInstance;

            try
            {
                List<object> Args = jsArgs.ToList();
                Args.Insert(0, this.InnerWebView);
                pluginInstance = Activator.CreateInstance(pluginType, Args.ToArray());

                if (pluginInstance == null)
                    return "Impossible to build plugin instance [" + pluginName + "]";

                pluginObj = (Plugin)pluginInstance;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            AddGlobalPluginInstance(pluginObj.UID, pluginInstance);
            return pluginInstance;
        }

        /// <summary>
        /// Execute recovery
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public object? ER(string UID)
        {
            if (!this.EnablePlugins)
                return "Plugins are disabled";

            if (!AppManager.PluginInstaces.ContainsKey(UID))
                return null;

            return AppManager.PluginInstaces[UID];
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
            JSType jsType = JSType.@null;
            object? csValue = null;
            object? jsValue;

            try
            {
                jsType = (JSType)Enum.Parse(typeof(JSType), jsTypeName);

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
                    
                    case JSType.Array:
                        return new JavaScript.Array((object[])raw, (object[])csParsed, stringified);

                    case JSType.BigInt:
                        return new BigInt(raw, BigInteger.Parse(csParsed+""), csParsed + "");

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
            catch (Exception ex) 
            {
                return ex.Message;
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
        /// Execute kill
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public bool EK(string UID)
        {
            return AppManager.PluginInstaces.Remove(UID);
        }

        [ComVisible(false)]
        public virtual void Reloaded()
        {
            // Eliminar las instancias de plugins creadas por este WebView
            foreach (string item in this.InnerUIDS)
                if(AppManager.PluginInstaces.ContainsKey(item))
                    ((IDisposable)AppManager.PluginInstaces[item]).Dispose();

            foreach (string item in this.InnerUIDS)
                AppManager.PluginInstaces.Remove(item);

            this.InnerUIDS.Clear();
        }

        #region PRIVATE METHODS

        private void AddGlobalPluginInstance(string UID, object pluginInstance)
        {
            if (!this.InnerAddToGlobal)
                return;

            this.InnerUIDS.Add(UID);
            AppManager.PluginInstaces.Add(UID, pluginInstance);
        }

        [ComVisible(false)]
        public void Dispose()
        {
            Dispose(true);

            // Evitar que el Garbage Collector llame al destructor/Finalizador ~WVPlugin()
            GC.SuppressFinalize(this);
            this.Disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.Disposed)
                return;

            if (disposing)
                Reloaded();
        }

        ~HOBase()
        {
            Dispose(false);
            this.Disposed = true;
        }

        #endregion

    }
}