using WV.Enums;
using WV.Interfaces;
using static WV.AppManager;
using Microsoft.Web.WebView2.Core;

namespace WV.Win.Imp
{
    public class Browser : Plugin, IBrowser
    {
        private WebView WV => (WebView)this.WebView;
        private CoreWebView2Controller WVController => this.WV.WVController!;
        private CoreWebView2 CoreWV2 => this.WVController.CoreWebView2;

        internal ContextMenu InternalContextMenu { get; }

        //=======================================//

        #region Events

        public event WVEventHandler<bool>? PlayingAudio;
        public event WVEventHandler<bool>? MutedEvent;
        public event WVEventHandler<double>? ZoomFactorChanged;
        public event WVEventHandler<string>? StatusBarTextChanged;

        #endregion

        //=======================================//

        #region Fields

        private double _MinZoomFactor = 0;
        private double _MaxZoomFactor = 0;
        private bool _ResetWebViewOnReload;

        // Hot Reload
        private Timer? _debounceTimer;
        private object _lock = new object();
        private FileSystemWatcher? _watcher;
        public bool _HotReload;

        #endregion

        public Browser(IContext ctx, string? language) : base(ctx)
        {
            this.Language =  Helpers.GetLanguage(language);
            this.InternalContextMenu = new ContextMenu(this.WV);
        }

        #region Properties

        public string Uri 
        { 
            get
            {
                ThrowIfDisposed();
                return this.CoreWV2.Source;
            }
            
        }

        public bool CanGoBack
        {
            get
            {
                ThrowIfDisposed();
                return this.CoreWV2.CanGoBack;
            }
        }  

        public bool CanGoForward
        {
            get
            {
                ThrowIfDisposed();
                return this.CoreWV2.CanGoForward;
            }
        } 

        public bool IsPlayingAudio
        {
            get
            {
                ThrowIfDisposed();
                return this.CoreWV2.IsDocumentPlayingAudio;
            }
        } 

        public bool StatusBar
        {
            get
            {
                ThrowIfDisposed();
                return this.CoreWV2.Settings.IsStatusBarEnabled;
            }
            set
            {
                ThrowIfDisposed();
                this.CoreWV2.Settings.IsStatusBarEnabled = value;
            }
        }

        public bool AcceleratorKeys
        {
            get 
            {
                ThrowIfDisposed();
                return this.CoreWV2.Settings.AreBrowserAcceleratorKeysEnabled;
            } 

            set
            {
                ThrowIfDisposed();
                this.CoreWV2.Settings.AreBrowserAcceleratorKeysEnabled = value;
            }
        }

        public bool SwipeNavigation
        {
            get 
            {
                ThrowIfDisposed();
                return this.CoreWV2.Settings.IsSwipeNavigationEnabled;
            } 

            set
            {
                ThrowIfDisposed();
                this.CoreWV2.Settings.IsSwipeNavigationEnabled = value;
            }
        }

        public bool HotReload
        {
            get 
            {
                ThrowIfDisposed();
                return _HotReload;
            }
            set
            {
                ThrowIfDisposed();

                _HotReload = value;

                if (value)
                {
                    if (_watcher != null)
                        return;

                    string StaticPath = AppManager.SrcPath;

                    try
                    {
                        // En el constructor o método de inicialización:
                        _debounceTimer = new Timer(HotReload_DelayedEvent, null, Timeout.Infinite, Timeout.Infinite);

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

                        
                        _watcher.Changed += HotReload_Refresh;
                        _watcher.Deleted += HotReload_Refresh;
                        _watcher.Renamed += HotReload_Refresh;
                        _watcher.Error += HotReload_Error;
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

                _watcher.Changed -= HotReload_Refresh;
                _watcher.Deleted -= HotReload_Refresh;
                _watcher.Error -= HotReload_Error;
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        public bool ResetWebViewOnReload 
        { 
            get
            {
                ThrowIfDisposed();
                return _ResetWebViewOnReload;
            }
            set
            {
                ThrowIfDisposed();
                this._ResetWebViewOnReload = value;
            }
        }

        public bool Muted
        {
            get 
            {
                ThrowIfDisposed();
                return this.CoreWV2.IsMuted;
            } 
            set
            {
                ThrowIfDisposed();
                this.CoreWV2.IsMuted = value;
            }
        }

        public IContextMenu ContextMenu
        {
            get
            {
                ThrowIfDisposed();
                return this.InternalContextMenu;
            }
        }

        public double MaxZoomFactor
        {
            get
            {
                ThrowIfDisposed();
                return _MaxZoomFactor;
            }
            internal set
            {
                ThrowIfDisposed();
                _MaxZoomFactor = value;
            }
        }

        public double MinZoomFactor 
        { 
            get
            {
                ThrowIfDisposed();
                return _MinZoomFactor;
            }
            internal set
            {
                ThrowIfDisposed();
                _MinZoomFactor = value;
            }
        }

        public double ZoomFactor
        {
            get 
            {
                ThrowIfDisposed();
                return this.WVController.ZoomFactor;
            } 
            set
            {
                ThrowIfDisposed();

                if (value < this.MinZoomFactor)
                    value = this.MinZoomFactor;

                else if(value > this.MaxZoomFactor)
                    value = this.MaxZoomFactor;

                if (this.WVController.ZoomFactor == value)
                    return;

                this.WVController.ZoomFactor = value;

                // El evento Nativo NO se dispara cuando el ZoomFactor seteado está dentro del rango maximo y minimo
                // Lo disparamos cuando eso suceda
                if(value >= this.MinZoomFactor && value <= this.MaxZoomFactor)
                    this.FireZoomFactorChangedEvent();
            }
        }

        public string StatusBarText
        {
            get
            {
                ThrowIfDisposed();
                return this.CoreWV2.StatusBarText;
            }
        }

        public string Language { get; }

        public BrowserColorScheme ColorScheme 
        { 
            get
            {
                ThrowIfDisposed();
                return (BrowserColorScheme)this.WVController.CoreWebView2.Profile.PreferredColorScheme;
            }
            set
            {
                ThrowIfDisposed();
                this.WVController.CoreWebView2.Profile.PreferredColorScheme = (CoreWebView2PreferredColorScheme)value;
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

        //=======================================//

        #region Methods

        public void OpenDevTools()
        {
            ThrowIfDisposed();
            this.CoreWV2.OpenDevToolsWindow();
        }

        public async Task<string> CallDevToolsProtocolAsync(string method, string? parametersAsJson = null)
        {
            ThrowIfDisposed();

            if (parametersAsJson == null)
                parametersAsJson = "{}";
            
            return await this.CoreWV2.CallDevToolsProtocolMethodAsync(method, parametersAsJson);
        }

        public void Navigate(string uri)
        {
            ThrowIfDisposed();
            this.CoreWV2.Navigate(uri);
        }

        public void Reload()
        {
            ThrowIfDisposed();
            Aux_Reload(false);
        }

        public void HardReload()
        {
            ThrowIfDisposed();
            //this.WV.CleanFileCache();
            Aux_Reload(true);
        }

        public Task<string>? ExecuteScriptAsync(string javaScript)
        {
            ThrowIfDisposed();
            return this.CoreWV2.ExecuteScriptAsync(javaScript);
        }

        public void GoBack()
        {
            ThrowIfDisposed();
            this.CoreWV2.GoBack();
        }

        public void GoForward()
        {
            ThrowIfDisposed();
            this.CoreWV2.GoForward();
        }

        #endregion

        //=======================================//

        #region Protected Methods

        protected override void ThrowIfDisposed()
        {
            base.ThrowIfDisposed();
            Plugin.ThrowIfDisposed(this.WV);
        }

        #endregion

        //=======================================//

        #region Internal Methods

        internal void ClearAllEvents()
        {
            this.ClearListeners();
            this.ClearEvents();
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

        internal void FireMutedEvent()
        {
            if (this.Disposed)
                return;

            bool muted = this.Muted;
            this.MutedEvent?.Invoke(this.WV, muted);
        }

        internal void FirePlayingAudioEvent()
        {
            if (this.Disposed)
                return;

            bool isPlayingAudio = this.IsPlayingAudio;
            this.PlayingAudio?.Invoke(this.WV, isPlayingAudio);
        }

        internal void FireStatusBarTextChangedEvent()
        {
            if (this.Disposed)
                return;

            string text = this.StatusBarText;
            this.StatusBarTextChanged?.Invoke(this.WV, text);
        }

        internal void FireZoomFactorChangedEvent()
        {
            if (this.Disposed)
                return;

            double factor = this.ZoomFactor;

            // Para cuando se recargue el WV se mantenga el ultimo ZoomFactor
            if (this.WV.WVController != null)
                this.WV.WVController.ZoomFactor = factor;

            this.ZoomFactorChanged?.Invoke(this.WV, factor);
        }

        #endregion

        //=======================================//

        #region Private Methods

        private void Aux_Reload(bool ignoreCache)
        {
            this.CoreWV2.CallDevToolsProtocolMethodAsync("Page.reload", @"{""ignoreCache"":" + ignoreCache.ToString().ToLower() + "}");
        }

        private void HotReload_Refresh(object sender, FileSystemEventArgs e)
        {
            lock (_lock)
            {
                _debounceTimer?.Change(Timeout.Infinite, Timeout.Infinite); // Cancelar timer existente
                _debounceTimer?.Change(100, Timeout.Infinite); // Reiniciar con 100 ms de delay
            }
        }

        private void HotReload_DelayedEvent(object? state)
        {
            this.WV.WVUIContext?.Post(x => this.HardReload(), null);
        }

        private void HotReload_Error(object sender, ErrorEventArgs e)
        {
            this.WV.WVUIContext?.Post(x =>
                this.ExecuteScriptAsync("alert(`" + e.ToString() + "`);")
            , null);
        }

        #endregion

    }
}