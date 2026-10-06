using System.Diagnostics;
using WV.Configs;
using WV.Core.Pluging;
using WV.Interfaces;

namespace WV.Core
{
    public abstract class WebViewCore : Plugin, IWebView
    {
        private static bool First = true;

        #region Fields

        protected bool _IsMain { get; private set; }
        protected IWindow? _Window { get; private set; }
        protected IBrowser? _Browser { get; private set; }
        protected IPlugins? _Plugins { get; private set; }
        protected IPrintManager? _PrintManager { get; private set; }

        #endregion

        #region Properties

        public bool IsMain 
        { 
            get
            {
                var txt = nameof(IsMain);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return _IsMain;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        public IWindow Window 
        { 
            get
            {
                var txt = nameof(Window);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return _Window!;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        public IBrowser Browser
        {
            get
            {
                var txt = nameof(Browser);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return _Browser!;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        public IPlugins Plugins
        {
            get
            {
                var txt = nameof(Plugins);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return _Plugins!;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        public IPrintManager PrintManager
        {
            get
            {
                var txt = nameof(PrintManager);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return _PrintManager!;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        #endregion

        protected WebViewCore(IPluginContext context, object? wvConfig = null) : base(context)
        {
            var txt = $"{nameof(WebViewCore)} constructor";
            LogCalling(txt);
            try
            {
                var wvconfig = ParseObj(wvConfig);
                var ctx = context;
                var source = Logger.Source;
                var wv = this;
                _IsMain = First;
                First = false;
                _Window = CreateWindow(Utils.CreateContext(wv, Logger, nameof(Window), source), wvconfig.Window);
                _Browser = CreateBrowser(Utils.CreateContext(wv, Logger, nameof(Browser), source), wvconfig.Browser);
                _PrintManager = CreatePrintManager(Utils.CreateContext(wv, Logger, nameof(PrintManager), source), wvconfig.PrintManager);
                _Plugins = CreatePlugins(Utils.CreateContext(wv, Logger, nameof(Plugins), source), wvconfig.Plugins);
                Initialize(context, wvconfig);
                
            }
            catch (Exception ex)
            {
                LogFail(txt , ex);
                throw;
            }
        }

        protected abstract IWindow CreateWindow(IPluginContext context, WindowConfig windowConfig);

        protected abstract IBrowser CreateBrowser(IPluginContext context, BrowserConfig browserConfig);

        protected abstract IPrintManager CreatePrintManager(IPluginContext context, PrintManagerConfig printManagerConfig);

        protected abstract IPlugins CreatePlugins(IPluginContext context, PluginsConfig pluginsConfig);

        protected abstract void Initialize(IPluginContext context, WebViewConfig wvConfig);

        public void Restart()
        {
            var txt = $"{nameof(Restart)}()";
            LogCalling(txt);
            try
            {
                ThrowIfDisposed();
                RestartCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected virtual void RestartCore()
        {
            if (!_IsMain)
                throw new InvalidOperationException("Only the main WebView can restart the application.");

            var startInfo = Utils.BuildRestartStartInfo();
            Logger.Info($"Restarting application: {startInfo.FileName}");

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to launch new process", ex);
                throw;
            }

            // Cierre ordenado del proceso actual.
            //RequestShutdown();
            // Finalizar proceso actual
            Environment.Exit(0);
        }

        protected abstract WebViewConfig GetWebViewConfigFromJSObject(object jsObject);

        protected sealed override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Plugins?.Dispose();
                _PrintManager?.Dispose();
                _Browser?.Dispose();
                _Window?.Dispose();
                
                _Window = null;
                _Browser = null;
                _Plugins = null;
                _PrintManager = null;
            }
            DisposeCore(disposing);
        }

        protected abstract void DisposeCore(bool disposing);

        #region Helpers

        private WebViewConfig ParseObj(object? obj)
        {
            if(obj == null)
                return new WebViewConfig();
            
            var type = obj.GetType();

            if(typeof(WebViewConfig) == type)
                return (WebViewConfig)obj;

            if(typeof(string) == type)
                return Utils.GetWebViewConfig((string)obj) ?? new WebViewConfig();

            if(type.IsCOMObject)
                return GetWebViewConfigFromJSObject(obj);

            return new WebViewConfig();
        }

        private void LogCalling(string txt, bool isProp = false)
        {
            Logger.Info(Utils.CallingMsg(txt, isProp));
        }

        private void LogFail(string txt, Exception ex, bool isProp = false)
        {
            Logger.Error(Utils.FailMsg(txt, isProp), ex);
        }

        #endregion

    }
}