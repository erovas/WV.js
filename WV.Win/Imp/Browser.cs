using WV.Enums;
using WV.Interfaces;
using static WV.AppManager;
using Microsoft.Web.WebView2.Core;

namespace WV.Win.Imp
{
    public class Browser : Plugin, IBrowser
    {
        private WebView WV { get; }
        private CoreWebView2? CoreWV2 => this.WV.WVController != null ? this.WV.WVController.CoreWebView2 : null;

        #region CS EVENTS

        private event WVEventHandler<bool>? playingAudioEvent;
        public event WVEventHandler<bool>? PlayingAudio
        {
            add
            {
                ThrowDispose();
                playingAudioEvent += value;
            }
            remove
            {
                ThrowDispose();
                playingAudioEvent -= value;
            }
        }

        private event WVEventHandler<bool>? mutedEvent;
        public event WVEventHandler<bool>? MutedEvent
        {
            add
            {
                ThrowDispose();
                mutedEvent += value;
            }
            remove
            {
                ThrowDispose();
                mutedEvent -= value;
            }
        }

        private event WVEventHandler<double>? zoomFactorChangedEvent;
        public event WVEventHandler<double>? ZoomFactorChanged
        {
            add
            {
                ThrowDispose();
                zoomFactorChangedEvent += value;
            }
            remove
            {
                ThrowDispose();
                zoomFactorChangedEvent -= value;
            }
        }

        private event WVEventHandler<string>? statusBarTextChangedEvent;
        public event WVEventHandler<string>? StatusBarTextChanged
        {
            add 
            {
                ThrowDispose();
                statusBarTextChangedEvent += value;
            }
            remove 
            {
                ThrowDispose();
                statusBarTextChangedEvent -= value;
            }
        }

        #endregion

        internal ContextMenu InternalContextMenu { get; }

        public Browser(WebView wv, string? language) : base(wv)
        {
            this.WV = wv;
            this.Language =  Helpers.GetLanguage(language);
            this.InternalContextMenu = new ContextMenu(wv);
        }

        #region PROPS

        public string Uri 
        { 
            get
            {
                ThrowDispose();
                return this.CoreWV2 != null ? this.CoreWV2.Source : string.Empty;
            }
            
        }

        public bool CanGoBack
        {
            get
            {
                ThrowDispose();
                return this.CoreWV2 != null ? this.CoreWV2.CanGoBack : false;
            }
        }  

        public bool CanGoForward
        {
            get
            {
                ThrowDispose();
                return this.CoreWV2 != null ? this.CoreWV2.CanGoForward : false;
            }
        } 

        public bool IsPlayingAudio
        {
            get
            {
                ThrowDispose();
                return this.CoreWV2 != null ? this.CoreWV2.IsDocumentPlayingAudio : false;
            }
        } 

        public bool StatusBar
        {
            get => this.CoreWV2 != null ? this.CoreWV2.Settings.IsStatusBarEnabled : true;

            set
            {
                if (this.CoreWV2 != null)
                    this.CoreWV2.Settings.IsStatusBarEnabled = value;
            }
        }

        public bool AcceleratorKeys
        {
            get 
            {
                ThrowDispose();
                return this.CoreWV2 != null ? this.CoreWV2.Settings.AreBrowserAcceleratorKeysEnabled : true;
            } 

            set
            {
                ThrowDispose();
                if (this.CoreWV2 != null)
                    this.CoreWV2.Settings.AreBrowserAcceleratorKeysEnabled = value;
            }
        }

        public bool SwipeNavigation
        {
            get 
            {
                ThrowDispose();
                return this.CoreWV2 != null ? this.CoreWV2.Settings.IsSwipeNavigationEnabled : false;
            } 

            set
            {
                ThrowDispose();

                if (this.CoreWV2 != null)
                    this.CoreWV2.Settings.IsSwipeNavigationEnabled = value;
            }
        }

        #region HotReload

        private Timer? _debounceTimer;
        private object _lock = new object();
        private FileSystemWatcher? _watcher;
        public bool _HotReload;
        public bool HotReload
        {
            get 
            {
                ThrowDispose();
                return _HotReload;
            }
            set
            {
                ThrowDispose();

                _HotReload = value;

                if (value)
                {
                    if (_watcher != null)
                        return;

                    string StaticPath = AppManager.SrcPath;

                    try
                    {
                        // En el constructor o método de inicialización:
                        _debounceTimer = new Timer(ExecuteDelayedEvent, null, Timeout.Infinite, Timeout.Infinite);

                        _watcher = new FileSystemWatcher(StaticPath)
                        {
                            
                            NotifyFilter = NotifyFilters.DirectoryName |
                                                NotifyFilters.FileName |
                                                NotifyFilters.LastWrite,
                            //Filter = "*.*",
                            Filter = "",
                            IncludeSubdirectories = true,
                            EnableRaisingEvents = true
                        };

                        
                        _watcher.Changed += HotReloadRefresh;
                        _watcher.Deleted += HotReloadRefresh;
                        _watcher.Renamed += HotReloadRefresh;
                        _watcher.Error += Event_HotReloadError;
                    }
                    catch (Exception)
                    {
                        _watcher = null;
                    }


                    return;
                }

                if (_watcher == null)
                    return;

                _debounceTimer?.Dispose();
                _debounceTimer = null;

                _watcher.Changed -= HotReloadRefresh;
                _watcher.Deleted -= HotReloadRefresh;
                _watcher.Error -= Event_HotReloadError;
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        private void HotReloadRefresh(object sender, FileSystemEventArgs e)
        {
            lock (_lock)
            {
                _debounceTimer?.Change(Timeout.Infinite, Timeout.Infinite); // Cancelar timer existente
                _debounceTimer?.Change(100, Timeout.Infinite); // Reiniciar con 100 ms de delay
            }
        }

        private void ExecuteDelayedEvent(object? state)
        {
            this.WV.WVUIContext?.Post(x => this.HardReload(), null);
        }

        private void Event_HotReloadError(object sender, ErrorEventArgs e)
        {
            this.WV.WVUIContext?.Post(x =>
                this.ExecuteScriptAsync("alert(`" + e.ToString() + "`);")
            , null);
        }

        #endregion

        #region ResetWebViewOnReload

        private bool _ResetWebViewOnReload;
        public bool ResetWebViewOnReload 
        { 
            get
            {
                ThrowDispose();
                return _ResetWebViewOnReload;
            }
            set
            {
                ThrowDispose();
                this._ResetWebViewOnReload = value;
            }
        }

        #endregion

        public bool Muted
        {
            get 
            {
                ThrowDispose();
                return this.CoreWV2 != null ? this.CoreWV2.IsMuted : false;
            } 
            set
            {
                ThrowDispose();
                if (this.CoreWV2 != null)
                    this.CoreWV2.IsMuted = value;
            }
        }

        public IContextMenu ContextMenu => this.InternalContextMenu;

        #region ZoomFactor

        private double _MaxZoomFactor = 0;
        public double MaxZoomFactor
        {
            get
            {
                ThrowDispose();
                return _MaxZoomFactor;
            }
            internal set
            {
                ThrowDispose();
                _MaxZoomFactor = value;
            }
        }

        private double _MinZoomFactor = 0;
        public double MinZoomFactor 
        { 
            get
            {
                ThrowDispose();
                return _MinZoomFactor;
            }
            internal set
            {
                ThrowDispose();
                _MinZoomFactor = value;
            }
        }

        public double ZoomFactor
        {
            get 
            {
                ThrowDispose();
                return this.WV.WVController != null ? this.WV.WVController.ZoomFactor : 1;
            } 
            set
            {
                ThrowDispose();

                if (this.WV.WVController == null)
                    return;

                if (value < this.MinZoomFactor)
                    value = this.MinZoomFactor;

                else if(value > this.MaxZoomFactor)
                    value = this.MaxZoomFactor;

                if (this.WV.WVController.ZoomFactor == value)
                    return;

                this.WV.WVController.ZoomFactor = value;

                // El evento Nativo NO se dispara cuando el ZoomFactor seteado está dentro del rango maximo y minimo
                // Lo disparamos cuando eso suceda
                if(value >= this.MinZoomFactor && value <= this.MaxZoomFactor)
                    this.FireZoomFactorChangedEvent();
            }
        }

        #endregion

        public string StatusBarText
        {
            get
            {
                ThrowDispose();
                return this.CoreWV2 != null ? this.CoreWV2.StatusBarText : string.Empty;
            }
        }

        public string Language { get; }

        #region ColorScheme

        public BrowserColorScheme ColorScheme 
        { 
            get
            {
                ThrowDispose();

                if(this.WV.WVController == null)
                    return BrowserColorScheme.Auto;

                return (BrowserColorScheme)this.WV.WVController.CoreWebView2.Profile.PreferredColorScheme;
            }
            set
            {
                ThrowDispose();

                if (this.WV.WVController == null)
                    return;

                this.WV.WVController.CoreWebView2.Profile.PreferredColorScheme = (CoreWebView2PreferredColorScheme)value;
            }
        }

        public string ColorSchemeText 
        { 
            get => this.ColorScheme.ToString();
            set
            {
                if (Enum.TryParse(value, out BrowserColorScheme myStates))
                    this.ColorScheme = myStates;
            }
        }

        #endregion

        #endregion

        //-------------------------------------------//

        #region METHODS

        public void OpenDevTools()
        {
            ThrowDispose();
            this.CoreWV2?.OpenDevToolsWindow();
        }

        public async Task<string> CallDevToolsProtocolAsync(string method, string? parametersAsJson = null)
        {
            ThrowDispose();

            if (this.CoreWV2 == null)
                return string.Empty;

            if (parametersAsJson == null)
                parametersAsJson = "{}";
            
            return await this.CoreWV2.CallDevToolsProtocolMethodAsync(method, parametersAsJson);
        }

        public void Navigate(string uri)
        {
            ThrowDispose();
            this.CoreWV2?.Navigate(uri);
        }

        public void Reload()
        {
            ThrowDispose();
            Aux_Reload(false);
        }

        public void HardReload()
        {
            ThrowDispose();
            //this.WV.CleanFileCache();
            Aux_Reload(true);
        }

        public Task<string>? ExecuteScriptAsync(string javaScript)
        {
            ThrowDispose();
            return this.CoreWV2?.ExecuteScriptAsync(javaScript);
        }

        public void GoBack()
        {
            ThrowDispose();
            this.CoreWV2?.GoBack();
        }

        public void GoForward()
        {
            ThrowDispose();
            this.CoreWV2?.GoForward();
        }

        #endregion

        //-------------------------------------------//

        #region EVENTS

        #region OnPlayingAudio

        private IJSFunction? OnPlayingAudioFN { get; set; }

        public object? OnPlayingAudio
        {
            get 
            {
                ThrowDispose();
                return OnPlayingAudioFN?.Raw;
            } 
            set
            {
                ThrowDispose();

                if (value == OnPlayingAudioFN?.Raw)
                    return;

                this.OnPlayingAudioFN?.Dispose();
                this.OnPlayingAudioFN = null;

                if (value == null)
                    return;

                this.OnPlayingAudioFN = IJSFunction.Create(value);
            }
        }

        internal void FirePlayingAudioEvent()
        {
            bool isPlayingAudio = this.IsPlayingAudio;
            this.OnPlayingAudioFN?.Execute(isPlayingAudio);
            this.playingAudioEvent?.Invoke(this.WV, isPlayingAudio);
        }

        #endregion

        #region OnMuted

        private IJSFunction? OnMutedFN { get; set; }

        public object? OnMuted
        {
            get 
            {
                ThrowDispose();
                return OnMutedFN?.Raw;
            } 
            set
            {
                ThrowDispose();

                if (value == OnMutedFN?.Raw)
                    return;

                this.OnMutedFN?.Dispose();
                this.OnMutedFN = null;

                if (value == null)
                    return;

                this.OnMutedFN = IJSFunction.Create(value);
            }
        }

        internal void FireMutedEvent()
        {
            bool muted = this.Muted;
            this.OnMutedFN?.Execute(muted);
            this.mutedEvent?.Invoke(this.WV, muted);
        }

        #endregion

        #region OnZoomFactorChanged

        private IJSFunction? OnZoomFactorChangedFN { get; set; }

        public object? OnZoomFactorChanged
        {
            get 
            {
                ThrowDispose();
                return OnZoomFactorChangedFN?.Raw;
            } 
            set
            {
                ThrowDispose();

                if (value == OnZoomFactorChangedFN?.Raw)
                    return;

                this.OnZoomFactorChangedFN?.Dispose();
                this.OnZoomFactorChangedFN = null;

                if (value == null)
                    return;

                this.OnZoomFactorChangedFN = IJSFunction.Create(value);
            }
        }

        internal void FireZoomFactorChangedEvent()
        {
            double factor = this.ZoomFactor;

            // Para cuando se recargue el WV se mantenga el ultimo ZoomFactor
            if(this.WV.WVController != null)
                this.WV.WVController.ZoomFactor = factor;

            this.OnZoomFactorChangedFN?.Execute(factor);
            this.zoomFactorChangedEvent?.Invoke(this.WV, factor);
        }

        #endregion

        #region OnStatusBarTextChanged

        private IJSFunction? OnStatusBarTextChangedFN { get; set; }

        public object? OnStatusBarTextChanged
        {
            get
            {
                ThrowDispose();
                return OnStatusBarTextChangedFN?.Raw;
            }
            set
            {
                ThrowDispose();

                if (value == OnStatusBarTextChangedFN?.Raw)
                    return;

                this.OnStatusBarTextChangedFN?.Dispose();
                this.OnStatusBarTextChangedFN = null;

                if (value == null)
                    return;

                this.OnStatusBarTextChangedFN = IJSFunction.Create(value);
            }
        }

        internal void FireStatusBarTextChangedEvent()
        {
            string text = this.StatusBarText;

            this.OnStatusBarTextChangedFN?.Execute(text);
            this.statusBarTextChangedEvent?.Invoke(this.WV, text);
        }

        #endregion

        #endregion

        private void ThrowDispose()
        {
            Plugin.ThrowDispose(this.WV);
        }

        private void Aux_Reload(bool ignoreCache)
        {
            this.CoreWV2?.CallDevToolsProtocolMethodAsync("Page.reload", @"{""ignoreCache"":" + ignoreCache.ToString().ToLower() + "}");
        }

        internal void ClearEvents()
        {
            this.CleanJSEvents();

            this.playingAudioEvent = null;
            this.mutedEvent = null;
            this.zoomFactorChangedEvent = null;
            this.statusBarTextChangedEvent = null;

            this.OnPlayingAudioFN = null;
            this.OnMutedFN = null;
            this.OnZoomFactorChanged = null;
            this.OnStatusBarTextChanged = null;
        }

        protected override void Dispose(bool disposing)
        {
            //throw new NotImplementedException();
        }

        public override void Dispose()
        {
            //base.Dispose();
        }

        internal void ToDefault()
        {
            //browser.StatusBar = true;
            this.AcceleratorKeys = true;
            this.SwipeNavigation = false;
            this.HotReload = false;
            this.ResetWebViewOnReload = false;
            this.Muted = false;
            //browser.PinchZoom = false;
            //browser.ZoomControl = false;
            this.ZoomFactor = 1;
            //browser.ContextMenu = false;

            this.InternalContextMenu.ToDefault();
        }

    }
}