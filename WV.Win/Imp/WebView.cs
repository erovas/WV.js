
using System.Collections.Concurrent;
using Microsoft.Web.WebView2.Core;
using WV.Win.Win32.Structs;
using WV.Win.Win32.Enums;
using System.Diagnostics;
using System.Reflection;
using WV.Win.Classes;
using WV.Win.Scripts;
using System.Drawing;
using WV.Attributes;
using WV.Interfaces;
using WV.Win.Win32;

namespace WV.Win.Imp
{
    public sealed class WebView : Plugin, IWebView
    {

        //-------------------------------------------------------//

        #region INTERNAL

        internal CoreWebView2Controller? WVController { get; private set; }

        internal UIThreadSyncCtx? WVUIContext { get; private set; }

        internal List<WebView> Children { get; } = new List<WebView>();

        internal Dictionary<string, PluginLoader> ImportedPlugins { get; } = new();

        internal Dictionary<string, object> PluginInstances { get; } = new();

        private ConcurrentDictionary<Type, object> SingletonInstances { get; } = new();

        // Cache para almacenar archivos: <ruta, (bytes, tamaño, lastModified, eTag)>
        private ConcurrentDictionary<string, BytesCache> FileCache { get; } = new();

        internal Window InternalWindow { get; }

        internal Browser InternalBrowser { get; }

        internal PrintManager InternalPrintManager { get; }

        internal IntPtr Handle { get; }

        internal string RootPath { get; private set; } = string.Empty;

        #endregion

        //-------------------------------------------------------//

        #region PUBLIC PROPS

        //public string UID { get; }

        public IWindow Window
        {
            get
            {
                Plugin.ThrowDispose(this);
                return this.InternalWindow;
            }
        }

        public IBrowser Browser
        {
            get
            {
                Plugin.ThrowDispose(this);
                return this.InternalBrowser;
            }
        }

        public IPrintManager PrintManager
        {
            get
            {
                Plugin.ThrowDispose(this);
                return this.InternalPrintManager;
            }
        }

        public bool IsMain { get; }
        
        public string[] PluginsName
        {
            get
            {
                Plugin.ThrowDispose(this);
                var list = this.ImportedPlugins.Keys.ToList();
                list.Insert(0, this.Name);
                return list.ToArray();
            }
        }
        

        #endregion

        //-------------------------------------------------------//

        #region CONSTRUCTORS

#pragma warning disable CS8604 // Posible argumento de referencia nulo
        public WebView(IWebView? webView) : this(webView, null)
#pragma warning restore CS8604 // Posible argumento de referencia nulo
        {
        }

#pragma warning disable CS8604 // Posible argumento de referencia nulo
        public WebView(IWebView? webView, string? url) : this(webView, url, null)
#pragma warning restore CS8604 // Posible argumento de referencia nulo
        {
        }

#pragma warning disable CS8604 // Posible argumento de referencia nulo
        public WebView(IWebView? webView, string? url, string? language) : base(webView)
#pragma warning restore CS8604 // Posible argumento de referencia nulo
        {
            // Para evitar que al compilar se quite el metodo por "no uso"
            this.PluginDisposed("______");

            // Se ha construido mal la instancia.
            // WebView == null es solo para la ventana principal
            if (Helpers.WinInstances.Count > 0 && this.WebView == null)
                throw new Exception("Instance created incorrectly");

            // Para saber si es el WebView principal
            this.IsMain = Helpers.WinInstances.Count == 0;

            language = this.IsMain ? language : (string.IsNullOrWhiteSpace(language) ? this.WebView.Browser.Language : language);

            this.InternalWindow = new Window(this);
            this.InternalBrowser = new Browser(this, language);
            this.InternalPrintManager = new PrintManager(this);

            // Esta instancia de WebView es hija de otro WebView, se guarda esta instancia como su hijo
            if (!this.IsMain)
                ((WebView)this.WebView).Children.Add(this);

            // Establecer WebView del plugin de la ventana principal, es padre de si misma
            if (this.IsMain)
            {
                PropertyInfo? prop = typeof(Plugin).GetProperty("WebView", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                MethodInfo? setter = prop?.GetSetMethod(nonPublic: true);
                setter?.Invoke(this, new object[] { this });
            }

            //---------------------------------------//

            this.Handle = Helpers.CreateMainWindows(this);
            IntPtr MainhWnd = this.Handle;

            // Ubicar la pantalla al inicio del monitor
            this.InternalWindow.Rect.SetPosition(0, 0);

            // Obteniendo el hilo del contexto del UI
            this.WVUIContext = new UIThreadSyncCtx(MainhWnd);
            SynchronizationContext.SetSynchronizationContext(this.WVUIContext);

            // Start initializing WebView2 in a fire-and-forget manner. Errors will be handled in the initialization function
            _ = CreateCoreWebView2Async(MainhWnd, url);

            //---------------------------------------//

            // Bucle de mensajes, que hacen funcionar la ventana
            Utils32.RunMessageLoop();
        }

        #endregion

        #region WndProc

        // Controla los mensajes de la ventana principal (padre)
        internal IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if(this.Disposed)
                return IntPtr.Zero;

            return this.InternalWindow.WndProc(hWnd, msg, wParam, lParam);
        }

        #endregion

        // Crear el WebView2 y agregarlo a la ventana, configurarlo
        private async Task CreateCoreWebView2Async(IntPtr hwnd, string? url)
        {
            //if (string.IsNullOrWhiteSpace(url))
            //    url = "index.html";

            //if(!url.StartsWith("http"))
            //    url = Helpers.URL + url;

            try
            {
                url = url?.Replace('\\', '/');

                // Por defecto
                if (string.IsNullOrWhiteSpace(url))
                    url = AppManager.SrcPath + "/index.html";

                // Puede ser un ruta a un archivo html local
                else if (Helpers.IsLocalPath(url) && !File.Exists(url))
                    throw new ArgumentException($"File not exists '{url}'");

                // Puede ser una ruta relativa en el directorio en donde esta WV.js
                else if (!Helpers.IsUri(url) && !File.Exists(url = AppManager.SrcPath + "/" + url))
                    throw new ArgumentException($"File not exists '{url}'");

                // Es una url a una pagina "https://www.MyPage.com"
                //else if(...)

                //url = Helpers.URL + "index.html"; //Para pruebas desde una pagina de "internet"

                // Si la url es una pagina, la ruta de ejecución será la del .../src
                if (Helpers.IsUri(url))
                    this.RootPath = AppManager.SrcPath;
                
                // Si la url es un archivo, la ruta de ejecución será en donde se encuentra el archivo html
                else
                    this.RootPath = Path.GetDirectoryName(url) + "";

                CoreWebView2Environment? Environment = null;
                string Language = this.InternalBrowser.Language;

                if (!Helpers.LangEnvironments.TryGetValue(Language, out Environment))
                {
                    string UserDataPath = AppManager.UserDataPath + (this.IsMain ? "" : "/" + Language);

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

                    CoreWebView2EnvironmentOptions envOptions = new(args, Language);
                    Environment = await CoreWebView2Environment.CreateAsync(null, UserDataPath, envOptions);
                    Helpers.LangEnvironments[Language] = Environment;
                }

                // Agregar control WebView a la ventana
                this.WVController = await Environment.CreateCoreWebView2ControllerAsync(hwnd);

                //this.InternalPrintManager.PrintSettings = Environment.CreatePrintSettings();
                this.InternalPrintManager.ToDefault();

                //Evitar parpadeo del WebView cuando se renderiza por primera vez
                this.WVController.DefaultBackgroundColor = Color.Transparent;

                // Hacer que el WebView tenga el mismo tamaño de la ventana
                User32.GetWindowRect(hwnd, out RECT rect);
                this.WVController.Bounds = new Rectangle(0, 0, rect.Width, rect.Height);
                this.WVController.IsVisible = true;

                CoreWebView2 CoreWV2 = this.WVController.CoreWebView2;

                //Ejecuta script principal justo antes de parsear el HTML
                foreach (string item in Helpers.JScripts)
                    await CoreWV2.AddScriptToExecuteOnDocumentCreatedAsync(item);

                CoreWV2.Settings.AreDefaultScriptDialogsEnabled = true;
                CoreWV2.Settings.IsWebMessageEnabled = true;
                CoreWV2.Settings.AreHostObjectsAllowed = true;

                //Se carga el WebView como HostObject que va a manejar todo lo relacionado con la ventana del WebView
                CoreWV2.AddHostObjectToScript(Helpers.HostObjectName, this);

                //Crear Servidor local tipo "https://WV.js"
                //if (Directory.Exists(AppManager.SrcPath))
                //    CoreWV2.SetVirtualHostNameToFolderMapping(AppManager.Domain, AppManager.SrcPath, CoreWebView2HostResourceAccessKind.Allow);

                //=====================================================//

                // Que NO aparezca la opción de abrir la dev tools desde el menu contextual o atajo de teclado
                CoreWV2.Settings.AreDevToolsEnabled = false;

                // Controlarlo con JS
                // Quitar el Zoom con gesture (touchpad | touchscreen)
                //CoreWV2.Settings.IsPinchZoomEnabled = false;

                // Navegación en touch con gesto
                CoreWV2.Settings.IsSwipeNavigationEnabled = false;

                // Controlarlo con JS
                //Que NO aparezca el menu click derecho. ¡¡¡Ya se hace de otra manera!!!
                //CoreWV2.Settings.AreDefaultContextMenusEnabled = false;

                // Quitar F5, y demas teclas especiales
                //this.InternalBrowser.AcceleratorKeys = false;

                // Controlarlo con JS
                // Quitar el Zoom con CTRL + +, Ctrl + scroll
                //CoreWV2.Settings.IsZoomControlEnabled = false;

                // Eliminar la statusbar (esquina inferior izquierda)
                CoreWV2.Settings.IsStatusBarEnabled = false;

                // Evento para manejar OnZoomFactoChanged de JS
                this.WVController.ZoomFactorChanged += WV_ZoomFactorChanged;

                // Se hace disparar el evento, para obtener el ZoomFactor Maximo
                this.WVController.ZoomFactor = double.MaxValue;

                //=====================================================//

                //window.chrome.webview.hostObjects.Window.getHostProperty("posicion").then(x => console.log(JSON.parse(x)))

                CoreWV2.ContextMenuRequested += WV2_ContextMenuRequested;
                CoreWV2.IsMutedChanged += WV2_IsMutedChanged;
                CoreWV2.IsDocumentPlayingAudioChanged += WV2_IsDocumentPlayingAudioChanged;
                CoreWV2.StatusBarTextChanged += CoreWV2_StatusBarTextChanged;
                CoreWV2.NavigationStarting += WV2_NavigationStarting;   //Evento "reload" para cuando se pulsa F5
                // "*" for all requests
                CoreWV2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All, CoreWebView2WebResourceRequestSourceKinds.All);
                //CoreWV2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);   // Deprecated
                CoreWV2.WebResourceRequested += WV2_ResourceRequested;

                //----------------------------//

                CoreWV2.NewWindowRequested += CoreWV2_NewWindowRequested;
                //CoreWV2.ProcessFailed += WV2_ProcessFailed;
                //CoreWV2.WebMessageReceived += WV2_WebMessageReceived;
                //CoreWV2.FrameCreated += WV2_FrameCreated;
                CoreWV2.PermissionRequested += WV2_PermissionRequested;
                
                //CoreWV2.ContentLoading += WV2_ContentLoading;
                //CoreWV2.DOMContentLoaded += WV2_DOMContentLoaded;
                CoreWV2.NavigationCompleted += WV2_NavigationCompleted;

                CoreWV2.ScreenCaptureStarting += WV2_ScreenCaptureStarting;

                //----------------------------//
                if (AppManager.IsDebugging)
                    CoreWV2.OpenDevToolsWindow();

                //Navegar a la página
                CoreWV2.Navigate(url);

                
            }
            catch (WebView2RuntimeNotFoundException)
            {
                var result = User32.MessageBox(hwnd, "WebView2 runtime not installed. Want to install it now?", AppManager.Domain, (uint)(MsgBoxStyle.MB_YESNO | MsgBoxStyle.MB_ICONINFORMATION));

                // Puede ser Cancel (cerrado con X)
                if (result != (int)MsgBoxResult.IDYES)
                    System.Environment.Exit(1);

                try
                {
                    string installerPath = await Helpers.DownloadWebView2Bootstrapper();

                    User32.MessageBox(hwnd, "The WebView2 installer will open. Complete the steps to continue.", AppManager.Domain, (uint)(MsgBoxStyle.MB_OK | MsgBoxStyle.MB_ICONINFORMATION));

                    bool success = Helpers.InstallWebView2Runtime(installerPath);

                    if (success)
                        await CreateCoreWebView2Async(hwnd, url);
                    else
                        throw new Exception("WebView2 installation not completed");
                }
                catch (Exception exc)
                {
                    User32.MessageBox(hwnd, exc.Message, "Error", (uint)(MsgBoxStyle.MB_OK | MsgBoxStyle.MB_ICONERROR));
                    System.Environment.Exit(1);
                }
            }
            catch (ArgumentException ex)
            {
                User32.MessageBox(hwnd, $"Failed to initialize WV.js: {System.Environment.NewLine}{ex.Message}", "Error", (uint)(MsgBoxStyle.MB_OK | MsgBoxStyle.MB_ICONERROR));
                System.Environment.Exit(1);
            }
            catch (Exception ex)
            {
                User32.MessageBox(hwnd, $"Failed to initialize WV.js: {System.Environment.NewLine}{ex}", "Error", (uint)(MsgBoxStyle.MB_OK | MsgBoxStyle.MB_ICONERROR));
                System.Environment.Exit(1);
            }
        }

        

        #region METHODS

        #region NewPluginInstance

        public object NewPluginInstance(string pluginName, params object[] args)
        {
            Plugin.ThrowDispose(this);

            PluginLoader? ctxPlugin = null;

            if (pluginName != this.Name && !this.ImportedPlugins.TryGetValue(pluginName, out ctxPlugin))
                throw new Exception("Plugin [" + pluginName + "] no exists");

            Type pluginType = ctxPlugin != null ? ctxPlugin.Type : typeof(WebView);

            List<object> Args = args.ToList();
            Args.Insert(0, this); // Constructor(WebView, arg1, arg2, ...)

            // Verifica si el plugin está marcado como singleton. Usa GetOrAdd para asegurar atomicidad y threadsafety
            if (pluginType.GetCustomAttribute<SingletonAttribute>() != null)
                return this.SingletonInstances.GetOrAdd(pluginType, type => CreateInstance(type, pluginName, Args.ToArray()));

            return CreateInstance(pluginType, pluginName, Args.ToArray());
        }

        private object CreateInstance(Type type, string pluginName, object[] args)
        {
            object? pluginInstance = Activator.CreateInstance(type, args);

            if (pluginInstance == null)
                throw new Exception("Impossible to build plugin instance [" + pluginName + "]");

            this.PluginInstances.Add(((Plugin)pluginInstance).UID, pluginInstance);

            return pluginInstance;
        }

        #endregion

        public object GetPluginInstance(string UID)
        {
            Plugin.ThrowDispose(this);
            return this.PluginInstances[UID];
        }

        public void Restart()
        {
            if (!this.IsMain)
                throw new Exception("This is not a main WebView");

            // Obtener la ruta real del proceso.
            string? executablePath = Helpers.GetRealExecutablePath();

            // Obtener argumentos originales (omitiendo el primer elemento que es la ruta del ejecutable)
            string[] arguments = Environment.GetCommandLineArgs().Skip(1).ToArray();

            if (arguments.Length > 0 && string.IsNullOrWhiteSpace(arguments[0]))
                arguments[0] = "null";

            // Configurar el nuevo proceso
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = string.Join(" ", arguments.Select(arg => arg.Contains(" ") ? $"\"{arg}\"" : arg)),
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory
            };

            // Manejo especial para .dll en desarrollo
            if (executablePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                startInfo.FileName = "dotnet";
                startInfo.ArgumentList.Insert(0, executablePath);
            }

            // Iniciar nuevo proceso
            Process.Start(startInfo);

            // Finalizar proceso actual
            Environment.Exit(0);
        }

        public string[] LoadPluginsFromFolder(string foldePath = "")
        {
            Plugin.ThrowDispose(this);

            if(string.IsNullOrWhiteSpace(foldePath))
                foldePath = AppManager.PluginsPath;

            if (!Directory.Exists(foldePath))
                throw new Exception($"The directory [{foldePath}] does not exist");

            List<string> dllError = new List<string>();

            foreach (string assemblyFile in Directory.GetFiles(foldePath, "*.dll", SearchOption.AllDirectories))
                try { this.LoadPlugin(assemblyFile); }
                catch (Exception ex) { dllError.Add(ex.Message); }
            
            return dllError.ToArray();
        }

        public string LoadPlugin(string pluginPath)
        {
            Plugin.ThrowDispose(this);
            
            try
            {
                var ctx = new PluginLoader(pluginPath, this.RootPath);
                var type = ctx.Load();

                if (this.ImportedPlugins.ContainsKey(type.Name))
                {
                    ctx.Unload();
                    throw new Exception($"The '{type.Name}' plugin has already been loaded");
                }
                    
                this.ImportedPlugins.Add(type.Name, ctx);
                return type.Name;
            }
            catch (Exception ex)
            {
                throw new Exception($"DLL: '{pluginPath}' - ERROR: {ex.Message}");
            }
        }

        public void UnloadPlugin(string pluginName)
        {
            Plugin.ThrowDispose(this);

            if (!this.ImportedPlugins.ContainsKey(pluginName))
                throw new Exception($"[{pluginName}] Plugin not found.");

            var ctx = this.ImportedPlugins[pluginName];
            
            var instances = this.PluginInstances.Where(i => ctx.Type == i.Value.GetType()).ToList();

            // Verificar si existe alguna instancia sin hacer Dispose()
            foreach (Plugin item in this.PluginInstances.Values)
                if(!item.Disposed)
                    throw new Exception($"The [{pluginName}] plugin cannot be unloaded while instances remain undisposed. {Environment.NewLine} Instance UID = {item.UID}");

            foreach (var instance in instances)
                this.PluginInstances.Remove(instance.Key);
            
            // Puede que sea un plugin Singleton
            this.SingletonInstances.TryRemove(ctx.Type, out object? _);
                
            ctx.Unload();
            this.ImportedPlugins.Remove(pluginName);
        }

        /*
        public object Entrada(object obj)
        {
            //Si le pasamos  { a:123, b:321 }, obtenemos '123', funciona con SYNC y ASYNC en JS
            //NO sirve con objetos planos que tengan getter y setters
            //object asd_get = Invoke.Helper.PropertyGet(obj, "a");

            //No se puede setear propiedades simples de objetos planos, ni con setters
            //Invoke.Helper.PropertySet(obj, "a", 3210123);

            //No da error, pero no funciona
            //Invoke.Helper.PropertySetRef(obj, "a", 3210123);

            //asd_get = Invoke.Helper.PropertyGet(obj, "a");

            
            // Funciona si se ejecuta desde ASYNC en JS
            object salida = Invoke.Helper.ExecuteMethod(obj, "", "Hola 123");

            try
            {
                // Funciona tanto si se ejecuta desde SYNC y ASYNC en JS
                Task.Run(() => { Invoke.Helper.ExecuteMethod(obj, "", "Hola 123"); });
            }
            catch (Exception)
            {
                throw;
            }
            

            JSFunction func = new JSFunction(obj);

            func.Execute("Hola 123");

            return obj;
        }
        */

        #endregion

        // Se lanza cuando se recarga el WebView
        internal void CleanMyself()
        {
            // Destruir/vaciar todos los eventos de IWindow
            this.InternalWindow.ClearEvents();

            // Destruir/vaciar todos los eventos de IBrowser
            this.InternalBrowser.ClearEvents();

            // Destruir/vaciar todos los eventos de IContextMenu
            this.InternalBrowser.InternalContextMenu.ClearEvents();

            // Destruir/vaciar todos los eventos de IPrintManager
            this.InternalPrintManager.ClearEvents();

            //-------------------------//

            //Hacer dispose de todas las instancias de WebView hijas
            foreach (WebView child in this.Children)
                try
                {
                    child.Dispose();
                }
                catch (Exception){ }

            // Limpiar la lista de instancias de WebView hijas
            this.Children.Clear();
                
            //Hacer dispose de todas las instancias de plugins
            foreach (Plugin item in this.PluginInstances.Values)
                try
                {
                    item.Dispose();
                }
                catch (Exception) { }

            // Limpiar la lista de instancias de plugins
            this.PluginInstances.Clear();
            this.SingletonInstances.Clear();

            // Unload de los Contextos de los plugins
            foreach (var ctx in this.ImportedPlugins.Values)
                ctx.Unload();
            
            this.ImportedPlugins.Clear();
            
        }

        internal void CleanFileCache()
        {
            this.FileCache.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            if (this.Disposed)
                return;

            this.CleanMyself();

            // Eliminar instancia del diccionario
            Helpers.WinInstances.Remove(this.Handle);

            this.WVController?.Close();
            this.WVController = null;

            //Cerrar la ventana del WebView en cuestion
            this.InternalWindow.Close();
        }

        private void PluginDisposed(string UID)
        {
            if (!this.PluginInstances.TryGetValue(UID, out object? instance))
                return;

            this.PluginInstances.Remove(UID);
            this.SingletonInstances.TryRemove(instance.GetType(), out object? _);
        }

        //==========================================//

        #region WebView Events

        #region ResourceRequested WV Event

        // Método para obtener/cargar archivos en cache
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
            if(sender == null) 
                return;

            CoreWebView2 CoreWV2 = (CoreWebView2)sender;

            // Se está navegando como File, evitar procesar
            if(CoreWV2.Source.StartsWith("file:///"))
                return;

            //-------------------------------------------------//

            // Es una petición http normal, evitar procesar
            if (e.Request.Uri.StartsWith("http"))
                return;

            //================================================//

            // Los espacios y caracteres especiales vienen con simbolos raros, quitarlos y normalizar la Uri
            string filePath = Uri.UnescapeDataString(e.Request.Uri);

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
                    $"Content-Type: {Helpers.GetMimeType(filePath)}\n" +
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
                $"Content-Type: {Helpers.GetMimeType(filePath)}\n" +
                $"Cache-Control: public, max-age=3600\n" +
                $"ETag: {cachedFile.ETag}\n" +
                $"Last-Modified: {cachedFile.LastModified.ToString("R")}\n" +
                $"Accept-Ranges: bytes\n" +
                $"Content-Length: {fileLength}"
            );
            
        }

        #endregion

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

        private void WV2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            this.CleanFileCache();
            Helpers.WebViewDefault(this);
        }

        private void WV2_PermissionRequested(object? sender, CoreWebView2PermissionRequestedEventArgs e)
        {
            // Permitir todo
            e.State = CoreWebView2PermissionState.Allow;
        }

        private void WV2_IsMutedChanged(object? sender, object e)
        {
            this.InternalBrowser.FireMutedEvent();
        }

        private void WV2_IsDocumentPlayingAudioChanged(object? sender, object e)
        {
            this.InternalBrowser.FirePlayingAudioEvent();
        }

        private void WV2_ContextMenuRequested(object? sender, CoreWebView2ContextMenuRequestedEventArgs e)
        {
            if (sender == null)
                return;

            this.InternalBrowser.InternalContextMenu.ContextMenuHandler((CoreWebView2)sender, e);
        }

        #region ZoomFactorChanged WV Event

        // Se dispara cuando se hace Ctrl + Scroll; Ctrl + ...,
        // o cuando se setea ZoomFactor por encima o por debajo de sus limites
        private void WV_ZoomFactorChanged(object? sender, object e)
        {
            if (this.WVController == null)
                return;

            var wvcontroller = this.WVController;
            Browser browser = this.InternalBrowser;

            // Obtener el ZoomFactor maximo
            if (browser.MaxZoomFactor == 0)
            {
                // Normalmente es 4.999999...
                browser.MaxZoomFactor = wvcontroller.ZoomFactor;
                // Hacemos disparar el evento para obtener el ZoomFactor minimo
                wvcontroller.ZoomFactor = double.Epsilon;
                return;
            }

            if (browser.MinZoomFactor == 0)
            {
                // Normalmente es 0.25
                browser.MinZoomFactor = wvcontroller.ZoomFactor;
                // Hacemos volver a la normalidad
                wvcontroller.ZoomFactor = 1;
                return;
            }

            // Disparar evento JS
            browser.FireZoomFactorChangedEvent();
            
        }

        #endregion

        private void CoreWV2_StatusBarTextChanged(object? sender, object e)
        {
            this.InternalBrowser.FireStatusBarTextChangedEvent();
        }

        private void CoreWV2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            // Evitar asi que se habrán otras ventanas NO deseadas explicitamente. con el New Window de JS
            e.Handled = true;
        }

        #endregion
        
    }
}