using Microsoft.Web.WebView2.Core;
using System.Collections.Concurrent;
using System.Drawing;
using WV.Configs;
using WV.Core.Browsering;
using WV.Enums;
using WV.Interfaces;
using WV.Win.Classes;
using WV.Win.Scripts;
using WV.Win.Win32;
using WV.Win.Win32.Enums;
using WV.Win.Win32.Structs;

namespace WV.Win.Imp
{
    public class Browser : BrowserCore, IBrowser
    {
        private CoreWebView2Controller? _WVController;

        private WebView WV => (WebView)this.WebView;
        private Window Window => WV.InternalWindow;
        private PrintManager PrintManager => WV.InternalPrintManager;
        private Plugins Plugins => WV.InternalPlugins;

        internal CoreWebView2Controller WVController => _WVController!;
        private CoreWebView2 CoreWV2 => WVController.CoreWebView2;
        private CoreWebView2Settings WVSettings => CoreWV2.Settings;
        internal ContextMenu InternalContextMenu => (ContextMenu)_ContextMenu!;

        // Cache para almacenar archivos: <ruta, (bytes, tamaño, lastModified, eTag)>
        private ConcurrentDictionary<string, BytesCache> FileCache { get; } = new();

        //=======================================//

        #region Fields

        private string _Language = string.Empty;
        private readonly DirectoryWatcher _Watcher;
        private BrowserConfig _Config;
        private readonly bool _IsMain;

        #endregion

        public Browser(IPluginContext context, BrowserConfig browserConfig) : base(context, browserConfig)
        {
            _Watcher = new DirectoryWatcher(Directory);
            _Config = browserConfig;
            _IsMain = WV.InternalIsMain;
        }

        protected override void Initialize(IPluginContext context, BrowserConfig browserConfig)
        {
            try
            {
                _Language = Utils.GetLanguage(browserConfig.Language);
                var url = ResolveUrl(browserConfig.Uri, this);
                _ = CreateCoreWebView2Async(browserConfig, url);
            }
            catch (Exception ex)
            {
                var msg = "Failed to initialize WV.js";

                if (_IsMain)
                {
                    Utils32.MsgBoxError(Window.Handle, msg, ex);
                    Utils.ExitApp();
                }

                throw;
            }
            
        }

        protected override IContextMenu CreateContextMenu(IPluginContext context)
        {
            return new ContextMenu(Utils.CreateContext(WebView, Logger, nameof(ContextMenu), Logger.Source), Directory);
        }

        #region Properties

        protected override string UriCore => this.CoreWV2.Source;

        protected override bool CanGoBackCore => this.CoreWV2.CanGoBack;

        protected override bool CanGoForwardCore => this.CoreWV2.CanGoForward;

        protected override bool IsPlayingAudioCore => this.CoreWV2.IsDocumentPlayingAudio;

        protected override bool StatusBarEnabledCore
        {
            get => this.WVSettings.IsStatusBarEnabled;
            set => this.WVSettings.IsStatusBarEnabled = value;
        }

        protected override bool AcceleratorKeysCore
        {
            get => this.WVSettings.AreBrowserAcceleratorKeysEnabled;
            set => this.WVSettings.AreBrowserAcceleratorKeysEnabled = value;
        }

        protected override bool SwipeNavigationCore
        {
            get => this.WVSettings.IsSwipeNavigationEnabled;
            set => this.WVSettings.IsSwipeNavigationEnabled = value;
        }

        protected override bool HotReloadCore
        {
            get 
            {
                return _Watcher.IsStarted;
            }
            set
            {
                if(value)
                    _Watcher.Start();
                else
                    _Watcher.Stop();
            }
        }

        //protected override bool ResetWebViewOnReloadCore
        //{ 
        //    get => _ResetWebViewOnReload;
        //    set => this._ResetWebViewOnReload = value;
        //}

        protected override bool MutedCore
        {
            get => this.CoreWV2.IsMuted;
            set => this.CoreWV2.IsMuted = value;
        }


        protected override double MaxZoomFactorCore { get; } = App.Browser.MaxZoomFactor; //=> _MaxZoomFactor;

        protected override double MinZoomFactorCore { get; } = App.Browser.MinZoomFactor; //=> _MinZoomFactor;

        protected override double ZoomFactorCore
        {
            get 
            {
                return this.WVController.ZoomFactor;
            } 
            set
            {

                if (value < this.MinZoomFactorCore)
                    value = this.MinZoomFactorCore;

                else if(value > this.MaxZoomFactorCore)
                    value = this.MaxZoomFactorCore;

                if (this.WVController.ZoomFactor == value)
                    return;

                this.WVController.ZoomFactor = value;

                // El evento Nativo NO se dispara cuando el ZoomFactor seteado está dentro del rango maximo y minimo
                // Lo disparamos cuando eso suceda
                if(value >= this.MinZoomFactorCore && value <= this.MaxZoomFactorCore)
                    this.FireZoomFactorChangedEvent();
            }
        }

        protected override string StatusBarTextCore => this.CoreWV2.StatusBarText;

        protected override string LanguageCore => _Language!;

        protected override BrowserColorScheme ColorSchemeCore
        { 
            get => (BrowserColorScheme)this.WVController.CoreWebView2.Profile.PreferredColorScheme;
            set => this.WVController.CoreWebView2.Profile.PreferredColorScheme = (CoreWebView2PreferredColorScheme)value;
        }

        protected override string ColorSchemeTextCore
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

        protected override void OpenDevToolsCore()
        {
            this.CoreWV2.OpenDevToolsWindow();
        }

        protected override async Task<string> CallDevToolsProtocolAsyncCore(string method, string? parametersAsJson = null)
        {
            if (parametersAsJson == null)
                parametersAsJson = "{}";
            
            return await this.CoreWV2.CallDevToolsProtocolMethodAsync(method, parametersAsJson);
        }

        protected override void NavigateCore(string uri)
        {
            this.CoreWV2.Navigate(uri);
        }

        protected override void ReloadCore()
        {
            Aux_Reload(false);
        }

        protected override void HardReloadCore()
        {
            //this.WV.CleanFileCache();
            Aux_Reload(true);
        }

        protected override Task<string>? ExecuteScriptAsyncCore(string javaScript)
        {
            return this.CoreWV2.ExecuteScriptAsync(javaScript);
        }

        protected override void GoBackCore()
        {
            this.CoreWV2.GoBack();
        }

        protected override void GoForwardCore()
        {
            this.CoreWV2.GoForward();
        }

        #endregion


        //=======================================//

        #region Internal Methods


        private void ToDefault()
        {
            ClearListeners();
            ClearEvents();

            var config = _Config;

            HotReloadCore = config.HotReload;
            AcceleratorKeysCore = config.AcceleratorKeys;
            SwipeNavigationCore = config.SwipeNavigation;
            //this.ResetWebViewOnReload = false;
            MutedCore = config.Muted;
            //browser.PinchZoom = false;
            //browser.ZoomControl = false;
            ZoomFactorCore = config.ZoomFactor;
            StatusBarEnabledCore = config.StatusBarEnabled;
            ColorSchemeCore = config.ColorScheme;

            this.InternalContextMenu.ToDefault();
        }

        private void FireZoomFactorChangedEvent()
        {
            if (this.Disposed)
                return;

            double factor = ZoomFactorCore;

            // Para cuando se recargue el WV se mantenga el ultimo ZoomFactor
            WVController?.ZoomFactor = factor;

            FireZoomFactorChangedEvent(factor);
        }

        #endregion

        protected override void DisposeCore(bool disposing)
        {
            if (!disposing)
                return;

            _Watcher.Dispose();

            if (_WVController is null)
                return;

            _WVController.ZoomFactorChanged -= WV_ZoomFactorChanged;

            var coreWV2 = this.CoreWV2;
            //----------------------------//

            coreWV2.ContextMenuRequested -= WV2_ContextMenuRequested;
            coreWV2.IsMutedChanged -= WV2_IsMutedChanged;
            coreWV2.IsDocumentPlayingAudioChanged -= WV2_IsDocumentPlayingAudioChanged;
            coreWV2.StatusBarTextChanged -= CoreWV2_StatusBarTextChanged;
            coreWV2.NavigationStarting -= WV2_NavigationStarting;   //Evento "reload" para cuando se pulsa F5
            coreWV2.WebResourceRequested -= WV2_ResourceRequested;

            //----------------------------//

            coreWV2.NewWindowRequested -= CoreWV2_NewWindowRequested;
            //coreWV2.ProcessFailed -= WV2_ProcessFailed;
            //coreWV2.WebMessageReceived -= WV2_WebMessageReceived;
            //coreWV2.FrameCreated -= WV2_FrameCreated;
            coreWV2.PermissionRequested -= WV2_PermissionRequested;

            //coreWV2.ContentLoading -= WV2_ContentLoading;
            //coreWV2.DOMContentLoaded -= WV2_DOMContentLoaded;
            coreWV2.NavigationCompleted -= WV2_NavigationCompleted;

            coreWV2.ScreenCaptureStarting -= WV2_ScreenCaptureStarting;

            _WVController.Close();
            _WVController = null;
        }

        #region Private Methods

        private void Aux_Reload(bool ignoreCache)
        {
            this.CoreWV2.CallDevToolsProtocolMethodAsync("Page.reload", @"{""ignoreCache"":" + ignoreCache.ToString().ToLower() + "}");
        }

        private async Task CreateCoreWebView2Async(BrowserConfig config, string url)
        {
            IntPtr hwnd = Window.Handle;

            try
            {
                CoreWebView2Environment Environment = await GetOrCreateEnvironmentAsync(_IsMain, _Language);

                // Agregar control WebView a la ventana
                var wvController = _WVController = await Environment.CreateCoreWebView2ControllerAsync(hwnd);

                ConfigureController(wvController, Window.Handle);

                CoreWebView2 coreWV2 = wvController.CoreWebView2;

                await ConfigureWebView(coreWV2, WebView);

                RegisterWebViewEvents(wvController, coreWV2);

                this.ToDefault();

                //Navegar a la página
                coreWV2.Navigate(url);

                _Watcher.OnWatch = e => 
                {
                    Window.WVUIContext?.Post(x => this.HardReload(), null);
                };

                _Watcher.OnError = e =>
                {
                    Window.WVUIContext?.Post(x => this.ExecuteScriptAsync($"alert(`{e}`);"), null);
                };
            }
            catch (WebView2RuntimeNotFoundException)
            {
                var yes = Utils32.MsgBoxYesNo(hwnd, "WebView2 runtime not installed. Want to install it now?", App.Window.Title);

                if (!yes)
                    Utils.ExitApp();

                try
                {
                    string installerPath = await Utils.DownloadWebView2Bootstrapper();

                    Utils32.MsgBoxInfo(hwnd, "The WebView2 installer will open. Complete the steps to continue.", App.Window.Title);

                    bool success = Utils.InstallWebView2Runtime(installerPath);

                    if (success)
                        await CreateCoreWebView2Async(config, url);
                    else
                    {
                        Utils32.MsgBoxError(hwnd, "WebView2 installation not completed");
                        Utils.ExitApp();
                    }
                        
                }
                catch (Exception exc)
                {
                    this.Logger.Critical("Failed to install WV.js", exc);
                    Utils32.MsgBoxError(hwnd, exc.Message);
                    Utils.ExitApp();
                }
            }
            catch (ArgumentException ex)
            {
                this.Logger.Critical("Failed to initialize WV.js", ex);

                if (_IsMain)
                {
                    Utils32.MsgBoxError(hwnd, "Failed to initialize WV.js:", ex);
                    Utils.ExitApp();
                }

                this.Dispose();
                throw;
                
            }
            catch (Exception ex)
            {
                this.Logger.Critical("Failed to initialize WV.js", ex);
                if (_IsMain)
                {
                    Utils32.MsgBoxError(hwnd, "Failed to initialize WV.js:", ex);
                    Utils.ExitApp();
                }
                this.Dispose();
                throw;
            }
        }

        private string ResolveUrl(string url, Browser bro)
        {
            // Puede ser un ruta a un archivo html local
            if (Utils.IsLocalPath(url) && !File.Exists(url))
                throw new ArgumentException($"File not exists '{url}'");

            // Puede ser una ruta relativa en el directorio en donde esta WV.js
            else if (!Utils.IsUri(url) && !File.Exists(url = Path.Combine(bro.Directory, url)))
                throw new ArgumentException($"File not exists '{url}'");

            // Es una url a una pagina "https://www.MyPage.com"
            //else if(...)

            //url = Helpers.URL + "index.html"; //Para pruebas desde una pagina de "internet"

            return url;
        }

        private static async Task<CoreWebView2Environment> GetOrCreateEnvironmentAsync(bool isMain, string language)
        {
            CoreWebView2Environment? environment = null;
            string lang = language;

            if (!Utils.LangEnvironments.TryGetValue(lang, out environment))
            {
                string userDataPath = (isMain ? Utils.UserDataFolder : Path.Combine(Utils.UserDataFolder, "lang"));

                //Quitar restricciones que tiene el WebView
                string args = string.Empty;
                //args += "--enable-features=EnableHostObjectJsonConversion,WebAssembly ";

                args += "--enable-features=WebRtcHybridAgc,WebRtcAllowScreenCaptureUnprompted ";
                args += "--enable-automation ";
                args += "--no-first-run ";
                args += "--disable-popup-blocking ";
                args += "--force-screen-capture ";
                args += "--force-display-capture ";
                args += "--auto-select-desktop-capture-source=\"Entire Screen\" ";
                args += "--enable-usermedia-screen-capturing ";

                args += "--disable-features=msWebOOUI,msPdfOOUI ";      //Quitar 3 puntos de menu contextual cuando se selecciona un texto
                args += "--disable-web-security ";                      //Deshabilita la política de mismo origen (Same-Origin Policy), permitiendo solicitudes cruzadas entre dominios
                args += "--allow-file-access-from-files ";
                args += "--allow-file-access ";
                //args += "--enable-features=WebAssembly ";
                args += "--auto-accept-camera-and-microphone-capture ";
                args += "--disable-features=PermissionsPolicy ";
                //args += "--auto-select-desktop-capture-source ";
                args += "--autoplay-policy=no-user-gesture-required ";  //Permitir auto reproduccion audio/video
                args += "--enable-gpu-benchmarking ";                 //Habilita chrome.gpuBenchmarking TODO: Mirar
                args += "--enable-precise-memory-info ";              //Valores mas precisos con performance.memory, 

                // expose-gc [Expone funcion gc() - Garbage Collector]
                // trace-gc [logs detallados del GC en la consola]
                args += "--js-flags=--expose-gc,--trace-gc ";

                var envOptions = new CoreWebView2EnvironmentOptions(args, lang);
                environment = await CoreWebView2Environment.CreateAsync(null, userDataPath, envOptions);
                Utils.LangEnvironments[lang] = environment;
            }

            return environment;
        }

        private static void ConfigureController(CoreWebView2Controller wv, IntPtr handle)
        {
            //Evitar parpadeo del WebView cuando se renderiza por primera vez
            wv.DefaultBackgroundColor = Color.Transparent;

            // Hacer que el WebView tenga el mismo tamaño de la ventana
            User32.GetWindowRect(handle, out RECT rect);
            wv.Bounds = new Rectangle(0, 0, rect.Width, rect.Height);
            wv.IsVisible = true;
        }

        private static async Task ConfigureWebView(CoreWebView2 coreWV2, object wv)
        {
            //Ejecuta script principal justo antes de parsear el HTML
            foreach (string item in Utils.JScripts)
                await coreWV2.AddScriptToExecuteOnDocumentCreatedAsync(item);

            coreWV2.Settings.AreDefaultScriptDialogsEnabled = true;
            coreWV2.Settings.IsWebMessageEnabled = true;
            coreWV2.Settings.AreHostObjectsAllowed = true;

            //Se carga el WebView como HostObject que va a manejar todo lo relacionado con la ventana del WebView
            coreWV2.AddHostObjectToScript(Utils.HostObjectName, wv);

            //Crear Servidor local tipo "https://WV.js"
            //if (Directory.Exists(AppManager.SrcPath))
            //    CoreWV2.SetVirtualHostNameToFolderMapping(AppManager.Domain, AppManager.SrcPath, CoreWebView2HostResourceAccessKind.Allow);

            //=====================================================//

            // Que NO aparezca la opción de abrir la dev tools desde el menu contextual o atajo de teclado
            coreWV2.Settings.AreDevToolsEnabled = false;

            // Controlarlo con JS
            // Quitar el Zoom con gesture (touchpad | touchscreen)
            //coreWV2.Settings.IsPinchZoomEnabled = false;

            // Navegación en touch con gesto
            coreWV2.Settings.IsSwipeNavigationEnabled = false;

            // Controlarlo con JS
            //Que NO aparezca el menu click derecho. ¡¡¡Ya se hace de otra manera!!!
            //CoreWV2.Settings.AreDefaultContextMenusEnabled = false;

            // Quitar F5, y demas teclas especiales
            //this.InternalBrowser.AcceleratorKeys = false;

            // Controlarlo con JS
            // Quitar el Zoom con CTRL + +, Ctrl + scroll
            //CoreWV2.Settings.IsZoomControlEnabled = false;

            // Eliminar la statusbar (esquina inferior izquierda)
            coreWV2.Settings.IsStatusBarEnabled = false;
        }

        private void RegisterWebViewEvents(CoreWebView2Controller WVController, CoreWebView2 coreWV2)
        {
            // Evento para manejar OnZoomFactoChanged de JS
            WVController.ZoomFactorChanged += WV_ZoomFactorChanged;

            //----------------------------//

            coreWV2.ContextMenuRequested += WV2_ContextMenuRequested;
            coreWV2.IsMutedChanged += WV2_IsMutedChanged;
            coreWV2.IsDocumentPlayingAudioChanged += WV2_IsDocumentPlayingAudioChanged;
            coreWV2.StatusBarTextChanged += CoreWV2_StatusBarTextChanged;
            coreWV2.NavigationStarting += WV2_NavigationStarting;   //Evento "reload" para cuando se pulsa F5
            // "*" for all requests
            coreWV2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All, CoreWebView2WebResourceRequestSourceKinds.All);
            coreWV2.WebResourceRequested += WV2_ResourceRequested;

            //----------------------------//

            coreWV2.NewWindowRequested += CoreWV2_NewWindowRequested;
            //coreWV2.ProcessFailed += WV2_ProcessFailed;
            //coreWV2.WebMessageReceived += WV2_WebMessageReceived;
            //coreWV2.FrameCreated += WV2_FrameCreated;
            coreWV2.PermissionRequested += WV2_PermissionRequested;

            //coreWV2.ContentLoading += WV2_ContentLoading;
            //coreWV2.DOMContentLoaded += WV2_DOMContentLoaded;
            coreWV2.NavigationCompleted += WV2_NavigationCompleted;

            coreWV2.ScreenCaptureStarting += WV2_ScreenCaptureStarting;

            //----------------------------//
            if (App.IsDebugging)
                coreWV2.OpenDevToolsWindow();
        }

        //------------------------//

        #region WV Events

        private void WV2_ScreenCaptureStarting(object? sender, CoreWebView2ScreenCaptureStartingEventArgs e)
        {
            e.Handled = true;
        }

        private void WV2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (sender == null)
                return;

            if (e.IsSuccess)
                return;

            CoreWebView2 CoreWV2 = (CoreWebView2)sender;

            string reason = string.Empty;

            reason += $"Impossible navigate to ['{CoreWV2.Source}']." + Environment.NewLine;
            reason += "Http status code: " + e.HttpStatusCode + Environment.NewLine;
            reason += "Reason: ";

            switch (e.WebErrorStatus)
            {
                case CoreWebView2WebErrorStatus.Unknown:
                    reason += "Unknown error.";
                    break;
                case CoreWebView2WebErrorStatus.CertificateCommonNameIsIncorrect:
                    reason += "SSL certificate common name does not match the web address";
                    break;
                case CoreWebView2WebErrorStatus.CertificateExpired:
                    reason += "SSL certificate has expired.";
                    break;
                case CoreWebView2WebErrorStatus.ClientCertificateContainsErrors:
                    reason += "SSL client certificate contains errors.";
                    break;
                case CoreWebView2WebErrorStatus.CertificateRevoked:
                    reason += "SSL certificate has been revoked.";
                    break;
                case CoreWebView2WebErrorStatus.CertificateIsInvalid:
                    reason += "SSL certificate is not valid.";
                    break;
                case CoreWebView2WebErrorStatus.ServerUnreachable:
                    reason += "Host is unreachable.";
                    break;
                case CoreWebView2WebErrorStatus.Timeout:
                    reason += "Connection has timed out.";
                    break;
                case CoreWebView2WebErrorStatus.ErrorHttpInvalidServerResponse:
                    reason += "Server returned an invalid or unrecognized response.";
                    break;
                case CoreWebView2WebErrorStatus.ConnectionAborted:
                    reason += "Connection was stopped.";
                    break;
                case CoreWebView2WebErrorStatus.ConnectionReset:
                    reason += "Connection was reset.";
                    break;
                case CoreWebView2WebErrorStatus.Disconnected:
                    reason += "Internet connection has been lost.";
                    break;
                case CoreWebView2WebErrorStatus.CannotConnect:
                    reason += "Connection to the destination was not established.";
                    break;
                case CoreWebView2WebErrorStatus.HostNameNotResolved:
                    reason += "Provided host name was not able to be resolved.";
                    break;
                case CoreWebView2WebErrorStatus.OperationCanceled:
                    reason += "Operation was canceled";
                    break;
                case CoreWebView2WebErrorStatus.RedirectFailed:
                    reason += "Request redirect failed.";
                    break;
                case CoreWebView2WebErrorStatus.UnexpectedError:
                    reason += "Unexpected error occurred.";
                    break;
                case CoreWebView2WebErrorStatus.ValidAuthenticationCredentialsRequired:
                    reason += "Valid Authentication Credentials Required";
                    break;
                case CoreWebView2WebErrorStatus.ValidProxyAuthenticationRequired:
                    reason += "Valid Proxy Authentication Required";
                    break;
                default:
                    reason += "¡¿Pero esto qué es?!";
                    break;
            }

            CoreWV2.NavigateToString(ScriptResources.ErrorPage.Replace("-replace-", reason));
        }

        private void WV2_PermissionRequested(object? sender, CoreWebView2PermissionRequestedEventArgs e)
        {
            // Permitir todo
            e.State = CoreWebView2PermissionState.Allow;
        }

        private void CoreWV2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            // Evitar asi que se habrán otras ventanas NO deseadas explicitamente. con el New Window de JS
            e.Handled = true;
        }

        private BytesCache GetCachedFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            // Si el archivo no existe, retornar vacío
            if (!fileInfo.Exists)
                return new BytesCache();

            // Generar ETag (usando última modificación)
            string eTag = fileInfo.LastWriteTime.Ticks.ToString("x");

            // Cargar o actualizar en cache
            return FileCache.AddOrUpdate(filePath,
                (path) =>
                {
                    byte[] data = File.ReadAllBytes(path);
                    return new BytesCache() { Bytes = data, LastModified = fileInfo.LastWriteTime, ETag = eTag };
                },
                (path, existing) =>
                {
                    // Si el archivo ha cambiado, actualizar cache
                    if (fileInfo.LastWriteTime > existing.LastModified)
                    {
                        byte[] data = File.ReadAllBytes(path);
                        return new BytesCache() { Bytes = data, LastModified = fileInfo.LastWriteTime, ETag = eTag };
                    }
                    return existing;
                });
        }

        private void WV2_ResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            if (sender == null)
                return;

            CoreWebView2 CoreWV2 = (CoreWebView2)sender;

            // Se está navegando como File, evitar procesar
            if (CoreWV2.Source.StartsWith("file:///"))
                return;

            //-------------------------------------------------//

            // Es una petición http normal, evitar procesar
            if (e.Request.Uri.StartsWith("http"))
                return;

            //================================================//

            //Se está navegando como Http, y se quiere cargar archivo locales del equipo.

            // Los espacios y caracteres especiales vienen con simbolos raros, quitarlos y normalizar la Uri
            string filePath = System.Uri.UnescapeDataString(e.Request.Uri);

            if (filePath.StartsWith("file:///"))
                filePath = filePath.Replace("file:///", "");

            BytesCache cachedFile = GetCachedFile(filePath);

            // El archivo NO existe
            if (cachedFile.Bytes == null)
            {
                e.Response = CoreWV2.Environment.CreateWebResourceResponse(null, 404, "Not Found", "");
                return;
            }

            long fileLength = cachedFile.Bytes.Length;
            string? rangeHeader = e.Request.Headers.Contains("Range") ? e.Request.Headers.GetHeader("Range") : null;

            if (!string.IsNullOrEmpty(rangeHeader) && rangeHeader.StartsWith("bytes="))
            {
                // Parsear el rango solicitado (ej: "bytes=0-1023")
                var range = rangeHeader.Replace("bytes=", "").Split('-');
                long start = long.Parse(range[0]);
                long end = (range[1] == "") ? fileLength - 1 : long.Parse(range[1]);

                // Validar rango
                if (start >= fileLength || end >= fileLength || start > end)
                {
                    e.Response = CoreWV2.Environment.CreateWebResourceResponse(
                        null,
                        416,
                        "Range Not Satisfiable",
                        $"Content-Range: bytes */{fileLength}" // Requerido por HTTP/416
                    );
                    return;
                }

                // Asegurar que el end no exceda el tamaño del archivo
                end = Math.Min(end, fileLength - 1);

                // Calcular la longitud del fragmento
                long length = end - start + 1;

                // Crear respuesta 206 Partial Content
                e.Response = CoreWV2.Environment.CreateWebResourceResponse(
                    new MemoryStream(cachedFile.Bytes, (int)start, (int)length),
                    206, // Código HTTP 206
                    "Partial Content",
                    $"Content-Type: {Utils.GetMimeType(filePath)}\n" +
                    $"Content-Range: bytes {start}-{end}/{fileLength}\n" +
                    $"Accept-Ranges: bytes\n" +
                    $"Content-Length: {length}"
                );

                return;
            }

            // Si no hay Range, enviar el archivo completo (200 OK)
            e.Response = CoreWV2.Environment.CreateWebResourceResponse(
                new MemoryStream(cachedFile.Bytes),
                200,
                "OK",
                $"Content-Type: {Utils.GetMimeType(filePath)}\n" +
                $"Cache-Control: public, max-age=3600\n" +
                $"ETag: {cachedFile.ETag}\n" +
                $"Last-Modified: {cachedFile.LastModified.ToString("R")}\n" +
                $"Accept-Ranges: bytes\n" +
                $"Content-Length: {fileLength}"
            );
        }

        private void WV2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            this.FileCache.Clear();

            //if (!_ResetWebViewOnReload)
            //    return;

            // Volver todo a Default
            Plugins.ToDefault();
            PrintManager.ToDefault();
            this.ToDefault();
            Window.ToDefault();
        }

        private void CoreWV2_StatusBarTextChanged(object? sender, object e)
        {
            FireStatusBarTextChangedEvent(StatusBarTextCore);
        }

        private void WV2_IsDocumentPlayingAudioChanged(object? sender, object e)
        {
            FirePlayingAudioEvent(IsPlayingAudioCore);
        }

        private void WV2_IsMutedChanged(object? sender, object e)
        {
            FireMutedEvent(MutedCore);
        }

        private void WV_ZoomFactorChanged(object? sender, object e)
        {
            FireZoomFactorChangedEvent();
        }

        private void WV2_ContextMenuRequested(object? sender, CoreWebView2ContextMenuRequestedEventArgs e)
        {
            if (sender == null)
                return;

            InternalContextMenu.ContextMenuHandler((CoreWebView2)sender, e);
        }

        #endregion

        //------------------------//

        #endregion



    }
}