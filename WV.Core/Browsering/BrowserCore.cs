
using WV.Configs;
using WV.Enums;
using WV.Interfaces;
using static WV.App.Delegates;

namespace WV.Core.Browsering
{
    public abstract class BrowserCore : Plugin, IBrowser
    {
        #region Fields

        protected IContextMenu? _ContextMenu;

        #endregion

        #region Events

        public event WVEventHandler<bool>? PlayingAudio;
        public event WVEventHandler<bool>? MutedEvent;
        public event WVEventHandler<double>? ZoomFactorChanged;
        public event WVEventHandler<string>? StatusBarTextChanged;

        #endregion

        protected BrowserCore(IPluginContext context, BrowserConfig browserConfig) : base(context)
        {
            var txt = $"{nameof(BrowserCore)} constructor";
            LogCalling(txt);
            try
            {
                Directory = Utils.GetFullDirectory(App.Directory, browserConfig.Directory);
                _ContextMenu = CreateContextMenu(context);
                Initialize(context, browserConfig);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void Initialize(IPluginContext context, BrowserConfig browserConfig);

        protected abstract IContextMenu CreateContextMenu(IPluginContext context);

        #region Properties

        public string Uri
        {
            get
            {
                var txt = nameof(Uri);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return UriCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract string UriCore { get; }

        public bool HotReload
        {
            get
            {
                var txt = nameof(HotReload);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return HotReloadCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(HotReload)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    HotReloadCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool HotReloadCore { get; set; }

        public bool CanGoBack
        {
            get
            {
                var txt = nameof(CanGoBack);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CanGoBackCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool CanGoBackCore { get; }

        public bool CanGoForward
        {
            get
            {
                var txt = nameof(CanGoForward);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CanGoForwardCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool CanGoForwardCore { get; }

        public bool IsPlayingAudio
        {
            get
            {
                var txt = nameof(IsPlayingAudio);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return IsPlayingAudioCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool IsPlayingAudioCore { get; }

        public bool AcceleratorKeys
        {
            get
            {
                var txt = nameof(AcceleratorKeys);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return AcceleratorKeysCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(AcceleratorKeys)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    AcceleratorKeysCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool AcceleratorKeysCore { get; set; }

        public bool SwipeNavigation
        {
            get
            {
                var txt = nameof(SwipeNavigation);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return SwipeNavigationCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(SwipeNavigation)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    SwipeNavigationCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool SwipeNavigationCore { get; set; }

        public double ZoomFactor
        {
            get
            {
                var txt = nameof(ZoomFactor);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return ZoomFactorCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(ZoomFactor)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    ZoomFactorCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract double ZoomFactorCore { get; set; }

        public double MaxZoomFactor
        {
            get
            {
                var txt = nameof(MaxZoomFactor);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return MaxZoomFactorCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract double MaxZoomFactorCore { get; }

        public double MinZoomFactor
        {
            get
            {
                var txt = nameof(MinZoomFactor);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return MinZoomFactorCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract double MinZoomFactorCore { get; }

        //public bool ResetWebViewOnReload
        //{
        //    get
        //    {
        //        var txt = nameof(ResetWebViewOnReload);
        //        LogCalling(txt, true);
        //        try
        //        {
        //            ThrowIfDisposed();
        //            return ResetWebViewOnReloadCore;
        //        }
        //        catch (Exception ex)
        //        {
        //            LogFail(txt, ex, true);
        //            throw;
        //        }
        //    }
        //    set
        //    {
        //        var txt = $"{nameof(ResetWebViewOnReload)} = {value}";
        //        LogCalling(txt, true);
        //        try
        //        {
        //            ThrowIfDisposed();
        //            ResetWebViewOnReloadCore = value;
        //        }
        //        catch (Exception ex)
        //        {
        //            LogFail(txt, ex, true);
        //            throw;
        //        }
        //    }
        //}

        //protected abstract bool ResetWebViewOnReloadCore { get; set; }

        public IContextMenu ContextMenu
        {
            get
            {
                var txt = nameof(ContextMenu);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return _ContextMenu!;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        public bool Muted
        {
            get
            {
                var txt = nameof(Muted);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return MutedCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Muted)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    MutedCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool MutedCore { get; set; }

        public string StatusBarText
        {
            get
            {
                var txt = nameof(StatusBarText);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return StatusBarTextCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract string StatusBarTextCore { get; }

        public bool StatusBarEnabled
        {
            get
            {
                var txt = nameof(StatusBarEnabled);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return StatusBarEnabledCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(StatusBarEnabled)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    StatusBarEnabledCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool StatusBarEnabledCore { get; set; }

        public string Language
        {
            get
            {
                var txt = nameof(Language);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return LanguageCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract string LanguageCore { get; }

        public BrowserColorScheme ColorScheme
        {
            get
            {
                var txt = nameof(ColorScheme);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return ColorSchemeCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(ColorScheme)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    ColorSchemeCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract BrowserColorScheme ColorSchemeCore { get; set; }

        public string ColorSchemeText
        {
            get
            {
                var txt = nameof(ColorSchemeText);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return ColorSchemeTextCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(ColorSchemeText)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    ColorSchemeTextCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract string ColorSchemeTextCore { get; set; }

        public string Directory { get; }

        #endregion

        #region Methods

        public Task<string> CallDevToolsProtocolAsync(string method, string? parametersAsJson = null)
        {
            var txt = $"{nameof(CallDevToolsProtocolAsync)}(\"{method}\", \"{parametersAsJson}\")";
            LogCalling(txt);
            try
            {
                return CallDevToolsProtocolAsyncCore(method, parametersAsJson);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract Task<string> CallDevToolsProtocolAsyncCore(string method, string? parametersAsJson = null);

        public Task<string>? ExecuteScriptAsync(string javaScript)
        {
            var txt = $"{nameof(ExecuteScriptAsync)}()";
            LogCalling(txt);
            try
            {
                return ExecuteScriptAsyncCore(javaScript);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract Task<string>? ExecuteScriptAsyncCore(string javaScript);

        public void GoBack()
        {
            var txt = $"{nameof(GoBack)}()";
            LogCalling(txt);
            try
            {
                GoBackCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void GoBackCore();

        public void GoForward()
        {
            var txt = $"{nameof(GoForward)}()";
            LogCalling(txt);
            try
            {
                GoForwardCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void GoForwardCore();

        public void HardReload()
        {
            var txt = $"{nameof(HardReload)}()";
            LogCalling(txt);
            try
            {
                HardReloadCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void HardReloadCore();

        public void Navigate(string uri)
        {
            var txt = $"{nameof(Navigate)}(\"{uri}\")";
            LogCalling(txt);
            try
            {
                NavigateCore(uri);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void NavigateCore(string uri);

        public void OpenDevTools()
        {
            var txt = $"{nameof(OpenDevTools)}()";
            LogCalling(txt);
            try
            {
                OpenDevToolsCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void OpenDevToolsCore();

        public void Reload()
        {
            var txt = $"{nameof(Reload)}()";
            LogCalling(txt);
            try
            {
                ReloadCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void ReloadCore();

        #endregion

        protected sealed override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _ContextMenu?.Dispose();
                _ContextMenu = null;
            }
            DisposeCore(disposing);
        }

        protected abstract void DisposeCore(bool disposing);

        protected virtual void FireMutedEvent(bool muted)
        {
            if (this.Disposed)
                return;

            this.MutedEvent?.Invoke(WebView, muted);
        }

        protected virtual void FirePlayingAudioEvent(bool isPlayingAudio)
        {
            if (this.Disposed)
                return;

            this.PlayingAudio?.Invoke(WebView, isPlayingAudio);
        }

        protected virtual void FireStatusBarTextChangedEvent(string text)
        {
            if (this.Disposed)
                return;

            this.StatusBarTextChanged?.Invoke(WebView, text);
        }

        protected virtual void FireZoomFactorChangedEvent(double factor)
        {
            if (this.Disposed)
                return;

            this.ZoomFactorChanged?.Invoke(WebView, factor);
        }


        #region Helpers

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