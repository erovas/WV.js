using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WV.JavaScript.Enums;
using WV.WebView;
using WV.WebView.Entities;
using WV.WebView.Enums;
using WV.Windows.Constants;
using WV.Windows.HostObject;
using WV.Windows.Utils;
using static System.Windows.Forms.AxHost;
using Microsoft.VisualBasic.Logging;
using System.Security.Policy;

namespace WV.Windows
{
    /// <summary>
    /// Lógica de interacción para WebView.xaml
    /// </summary>
    public partial class WebView : Window
    {
        private List<IFrame> InnerFrames { get; }
        private WindowInteropHelper InnerWinInterop { get; }
        private IntPtr InnerHandle => this.InnerWinInterop.Handle;
        private bool IsWV2Ready => this.WV2 != null && this.WV2.CoreWebView2 != null;

        private double BackupMaxWidth { get; set; }
        private double BackupMaxHeight { get; set; }
        private double BackupMinWidth { get; set; } 
        private double BackupMinHeight { get; set; }

        /// <summary>
        /// Guarda el State de la ventana antes de ser Minimizada o FullScreen.
        /// (Solo guarda Maximized || Normalized)
        /// </summary>
        private WVState LastState { get; set; }

        /// <summary>
        /// Guarda el estado REAL de la ventana WebView
        /// </summary>
        private WVState InternalState { get; set; }

        /// <summary>
        /// Flag para evitar lanzar evento JS de State
        /// </summary>
        private bool NoFireStateEvent { get; set; }

        /// <summary>
        /// Sirve para comparar con this.X y saber si hubo o no un cambio para disparar evento JS de Position
        /// </summary>
        private int LastX { get; set; }

        /// <summary>
        /// Sirve para comparar con this.Y y saber si hubo o no un cambio para disparar evento JS de Position
        /// </summary>
        private int LastY { get; set; }

        private double LastWidth { get; set; }
        private double LastHeight { get; set; }
        private double LastLeft { get; set; }
        private double LastTop { get; set; }

        /// <summary>
        /// Flag para indicar si el cursor está encerrado o no dentro de la ventana
        /// </summary>
        private bool IsClippedCursor { get; set; }

        private HOWindows InnerHOWindows { get; }

        private Parameters InnerParameters { get; }

        private bool InnerFirstLoad { get; set; } = true;

        private IScreen InnerScreen { get; set; }

        private WebView(string[] args)
        {
            InitializeComponent();

            this.InnerFrames = new List<IFrame>();
            this.BackupMaxWidth = double.PositiveInfinity;
            this.BackupMaxHeight = double.PositiveInfinity;
            this.BackupMinWidth = 1;
            this.BackupMinHeight = 1;

            this.InnerHOWindows = new HOWindows(this, args);

            // Seteo eventos nativos disparados en JS
            this.LocationChanged += WPF_LocationChanged;
            this.StateChanged += WPF_StateChanged;

            this.Activated += WPF_Activated;
            this.Deactivated += WPF_Activated;
            this.IsEnabledChanged += WPF_IsEnabledChanged;
            this.Closing += WPF_Closing;
            this.Closed += WPF_Closed;

            //
            this.InnerWinInterop = new WindowInteropHelper(this);
            this.InternalState = WVState.Restore;
            
            this.LastX = this.X;
            this.LastY = this.Y;

            //this.InnerScreen = this.Screen;
        }

        protected async override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Solución bug de animación de minimizar/maximizar
            AUX_FixAnimationWindow(this.InnerHandle);

            //"Evento" para permitir o evitar Snapping
            AUX_SetSnappingEvent();

            this.WV2.CoreWebView2InitializationCompleted += WV2_InitializationCompleted;

            if (AppManager.Environment == null)
            {
                //Para quitar los 3 puntos de menu contextual cuando se selecciona un texto
                CoreWebView2EnvironmentOptions threeDotsOff = new("--disable-features=msWebOOUI,msPdfOOUI");
                //Poner el menú contextual en un idioma
                threeDotsOff.Language = AppManager.Language;
                threeDotsOff.AdditionalBrowserArguments = "--disable-web-security --allow-file-access-from-files --allow-file-access";
                AppManager.Environment = await CoreWebView2Environment.CreateAsync(null, AppManager.UserDataPath, threeDotsOff);
                //AppManager.Environment = await CoreWebView2Environment.CreateAsync(null, AppManager.UserDataPath);
            }

            //Utilizar el Enviroment original
            await this.WV2.EnsureCoreWebView2Async(AppManager.Environment);

            //Ejecuta script principal justo antes de parsear el HTML
            //string JScript = AppLocal.JScript.Replace("0/*UID*/", this.UID);
            foreach (string item in AppManager.JScripts)
                await this.WV2.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(item);
            
            //Se carga el HostObject que va a manejar todo el tinglado
            this.WV2.CoreWebView2.AddHostObjectToScript(AppManager.HostObjectName, this.InnerHOWindows);

            try
            {
                string DomainName = this.Domain;
                string SRCFolderPath = AppManager.SrcPath;
                
                if (this.LocalServer)
                    this.WV2.CoreWebView2.SetVirtualHostNameToFolderMapping(DomainName, SRCFolderPath, CoreWebView2HostResourceAccessKind.Allow);

                if (this.IndexPage != null)
                    this.WV2.CoreWebView2.Navigate(this.IndexPage);
                else if (this.LocalServer)
                    this.WV2.CoreWebView2.Navigate("https://" + DomainName + "/index.html");
                else
                    throw new Exception("There is no index page or local server");

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK);
                this.Close(true);
            }

            this.WV2.CoreWebView2.ContextMenuRequested += WV2_ContextMenuRequested;
            this.WV2.CoreWebView2.ProcessFailed += WV2_ProcessFailed;
            this.WV2.CoreWebView2.IsDocumentPlayingAudioChanged += WV2_IsDocumentPlayingAudioChanged;
            this.WV2.CoreWebView2.IsMutedChanged += WV2_IsMutedChanged;
            this.WV2.CoreWebView2.WebMessageReceived += WV2_WebMessageReceived;

            this.WV2.CoreWebView2.FrameCreated += WV2_FrameCreated;
            
            // Que no aparezca la opción de abrir la dev tools desde el menu contextual o atajo de teclado
            this.WV2.CoreWebView2.Settings.AreDevToolsEnabled = false;

            //Menu click derecho
            //this.WV2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;  
            this.DefaultContextMenus = this.DefaultContextMenus;

            this.EnablePlugins = this.EnablePlugins;
            this.Visible = this.Visible;

            this.WV2.CoreWebView2.OpenDevToolsWindow();
            //this.WV2.CoreWebView2.OpenTaskManagerWindow();

            //this.WV2.CoreWebView2.OpenDefaultDownloadDialog();

            //para adelante o atras mediante gesture touch (swipe)
            this.WV2.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;  

            //Requiere un reload para ser aplicados desde JS
            this.StatusBar = this.StatusBar; //https://github.com/MicrosoftEdge/WebView2Feedback/issues/583
            this.GeneralAutofill = this.GeneralAutofill;
            this.PasswordAutosave = this.PasswordAutosave;
            this.PinchZoom = this.PinchZoom;
            this.ZoomControl = this.ZoomControl;
            this.DefaultScriptDialogs = this.DefaultScriptDialogs;      //!! Mirar lo de los eventos
            this.BrowserAcceleratorKeys = this.BrowserAcceleratorKeys;  //!! Mirar lo de los eventos

            //window.chrome.webview.hostObjects.Window.getHostProperty("posicion").then(x => console.log(JSON.parse(x)))

            //Evento "reload" para cuando se pulsa F5
            this.WV2.CoreWebView2.NavigationStarting += WV2_NavigationStarting;

            this.WV2.CoreWebView2.NavigationCompleted += WV2_NavigationCompleted;

            this.WV2.CoreWebView2.ContentLoading += WV2_ContentLoading;


            string filter = "*";   // or "*" for all requests
            this.WV2.CoreWebView2.AddWebResourceRequestedFilter(filter, CoreWebView2WebResourceContext.All);
            this.WV2.CoreWebView2.WebResourceRequested += WV2_ResourceRequested;

            this.InnerScreen = this.Screen;
            
        }

        private void WV2_ResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            //e.Request.Method = "GET";
            //e.Request.Content 
            //var header = e.Request.Headers;
            //e.Request.Uri = "";
            /*
            if(e.Response != null)
            {
                Stream? contentResponse = e.Response.Content;
                var headerResponse = e.Response.Headers;
                var ReasonPhrase = e.Response.ReasonPhrase;
                var StatusCode = e.Response.StatusCode;

                var adsd = "";
            }

            


            var headers = e.Request.Headers;
            var Method = e.Request.Method;
            string postData = null;
            var content = e.Request.Content;

            // get content from stream
            if (content != null)
            {
                using (var ms = new MemoryStream())
                {
                    content.CopyTo(ms);
                    ms.Position = 0;
                    postData = Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            var url = e.Request.Uri.ToString();

            // collect the headers from the collection into a string buffer
            StringBuilder sb = new StringBuilder();
            foreach (var header in headers)
            {
                sb.AppendLine($"{header.Key}: {header.Value}");
            }

            // for demo write out captured string vals
            Debug.WriteLine($"{url}\n{sb.ToString()}\n{postData}\n---");
            */
            //e.Response.StatusCode

            //var response = new HttpResponseMessage();
            //var response = new CoreWebView2WebResourceResponse();
            //response.StatusCode = HttpStatusCode.NotFound;
            //CoreWebView2WebResourceResponse response = this.WV2.CoreWebView2.Environment.CreateWebResourceResponse(fs, 200, "OK", "Content-Type: image/jpeg");

            //Uri uriasd = new Uri(e.Request.Uri);


            //string[] segmentos = uriasd.Segments;

            //string urls = File.ReadAllText("D:/Documents/asd.txt");
            //arrUrls = urls.Split(Environment.NewLine);

            //if (arrUrls.Contains(segmentos[segmentos.Length - 1])) 
            //if (arrUrls.Contains(uriasd.PathAndQuery))
            //if(isAd(e.Request.Uri))
            //{
            //    CoreWebView2WebResourceResponse response = this.WV2.CoreWebView2.Environment.CreateWebResourceResponse(null, (int)HttpStatusCode.Forbidden, "Blocked", string.Empty);
            //    e.Response = response;
            //}

            //CoreWebView2WebResourceResponse response = this.WV2.CoreWebView2.Environment.CreateWebResourceResponse(null, (int)HttpStatusCode.Forbidden, "Blocked", string.Empty);
            //e.Response = response;
        }

        //private string[] arrUrls;

        //public string getHost(string url)
        //{
        //    return new Uri(url).Host;
        //}

        //public bool isAd(string url)
        //{
        //    try
        //    {
        //        return isAdHost(getHost(url)) || arrUrls.Contains(new Uri(url).AbsolutePath);
        //        //return arrUrls.Contains(new Uri(url).AbsolutePath);
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }

        //}

        //private bool isAdHost(string host)
        //{
        //    if (string.IsNullOrEmpty(host))
        //    {
        //        return false;
        //    }
        //    int index = host.IndexOf(".");

        //    return index >= 0 && (arrUrls.Contains(host) || index + 1 < host.Length && isAdHost(host.Substring(index + 1)));
        //}

        #region WEBVIEW EVENTS

        private void WV2_ContentLoading(object? sender, CoreWebView2ContentLoadingEventArgs e)
        {
            this.IsLoadingPage = true;

            //if (this.InnerFirstLoad)
            //    this.InnerFirstLoad = false;
            //else
            //    AUX_SetParameters(this.InnerParameters);

            if (!e.IsErrorPage)
                return;

            if (this.ErrorPage == null)
                return;

            this.WV2.CoreWebView2.Navigate(this.ErrorPage);
        }

        private void WV2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            //this.WV2.CoreWebView2.Source
            this.IsLoadingPage = false;

        }

        private void WV2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            //throw new NotImplementedException();
            
        }

        private void WV2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string Source = e.Source;
            string? MessageAsString = null;
            string MessageAsJson = e.WebMessageAsJson;

            try
            {
                MessageAsString = e.TryGetWebMessageAsString();
            }
            catch (Exception) { }

            WebMessageReceived?.Invoke(this, new WebMessage(Source, MessageAsString, MessageAsJson));
        }

        /// <summary>
        /// Controla la creación de los IFrames
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WV2_FrameCreated(object? sender, CoreWebView2FrameCreatedEventArgs e)
        {
            CoreWebView2Frame rawFrame = e.Frame;

            // Para controlar la destrucción de los IFrame
            rawFrame.Destroyed += WV2_FrameDestroyed;

            Frame frame = new Frame(rawFrame, this) 
            { 
                EnablePlugins = this.EnableFramePlugins 
            };

            // Agregar el Iframe
            this.InnerFrames.Add(frame);

            // Disparar evento de la interfaz
            FrameCreated?.Invoke(this, frame);            
        }

        /// <summary>
        /// Controla la destrucción de los IFrames
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WV2_FrameDestroyed(object? sender, object e)
        {
            CoreWebView2Frame rawFrame = (CoreWebView2Frame)sender;

            foreach (Frame item in InnerFrames)
            {
                if (item.InnerIFrame != rawFrame)
                    continue;

                // Quitar el IFrame
                InnerFrames.Remove(item);

                // Disparar evento de la interfaz
                FrameDestroyed?.Invoke(this, item);

                // Ya el IFrame está destruido, quitar el control
                rawFrame.Destroyed -= WV2_FrameDestroyed;
                return;
            }
        }

        private void WV2_IsMutedChanged(object? sender, object e)
        {
            AUX_FireJsEvent(JSEvent.muted, "Muted", this.WV2.CoreWebView2.IsMuted);
        }

        private void WV2_IsDocumentPlayingAudioChanged(object? sender, object e)
        {
            AUX_FireJsEvent(JSEvent.playingaudio, "IsPlayingAudio", this.WV2.CoreWebView2.IsDocumentPlayingAudio);
        }

        private void WV2_ProcessFailed(object? sender, CoreWebView2ProcessFailedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void WV2_ContextMenuRequested(object? sender, CoreWebView2ContextMenuRequestedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void WV2_InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            this.WV2.CoreWebView2.NewWindowRequested += WV2_NewWindowRequested;
        }

        private void WV2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            //No hacer nada
            e.Handled = true;
        }

        #endregion

        #region WPF EVENTS

        private void WPF_Closed(object? sender, EventArgs e)
        {
            //Matar el webview2
            this.WV2?.Dispose();
            this.WV2 = null;
        }

        private void WPF_Closing(object? sender, CancelEventArgs e)
        {
            e.Cancel = this.PreventClose;

            if (e.Cancel)
                AUX_FireJsEvent(JSEvent.closing, "PreventClose", this.PreventClose);
            else
                this.InnerHOWindows.Dispose();
        }

        private void WPF_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AUX_FireJsEvent(JSEvent.enabled, "Enabled", this.Enabled);
        }

        private void WPF_Activated(object? sender, EventArgs e)
        {
            AUX_FireJsEvent(JSEvent.activated, "IsActive", this.IsActive);
        }

        private void WPF_StateChanged(object? sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Normal:

                    if (this.InternalState == WVState.Restore)
                        return;

                    //El WebView está en modo FullScreen y está minimizado
                    if (this.IsFullScreen && this.InternalState == WVState.Minimized)
                    {
                        //Por si el cursor estabá clipeado antes de minimizarlo, cliplear para "restaurarlo"
                        if (this.IsClippedCursor)
                            AUX_ClipCursor(this.InnerHandle, true);

                        //Indicar que ya está en State FullScreen
                        this.InternalState = WVState.FullScreen;

                        //Disparar evento JS de State
                        AUX_FireWinStateJsEvent();

                        return;
                    }

                    //Cuando se pasa de Maximizado a Normalizado, se quita corrección de descuadre por el Maximizado
                    if (this.InternalState == WVState.Maximized)
                        AUX_UndoSkewCorrectionMaximized();

                    //Indicar que el WebView está Normalizado
                    this.InternalState = WVState.Restore;

                    //Disparar eventos JS de State y Position
                    AUX_FireWinStateJsEvent();
                    AUX_FirePositionJsEvent();

                    break;

                case WindowState.Minimized:

                    //Si NO está en modo FullScreen, guardar el ultimo State [Normalized || Maximized]
                    if (!this.IsFullScreen)
                        this.LastState = this.InternalState;

                    //Si SÍ está en modo FullScreen, quitar el clipping cuando se minimiza
                    if (this.IsClippedCursor)
                        AUX_ClipCursor(this.InnerHandle, false);

                    //Indicar que ya está en State Minimizado
                    this.InternalState = WVState.Minimized;

                    //Disparar eventos JS de State y Position
                    AUX_FireWinStateJsEvent();
                    AUX_FirePositionJsEvent();
                    break;

                case WindowState.Maximized:

                    if (this.NoFireStateEvent)
                        return;

                    if (this.InternalState == WVState.Maximized)
                        return;

                    //Corrección por descuadre del WPF cuando se maximiza
                    AUX_SkewCorrectionMaximized();

                    //Indicar que el WebView está Minimizado
                    this.InternalState = WVState.Maximized;

                    //Disparar eventos JS de State y Position
                    AUX_FireWinStateJsEvent();
                    AUX_FirePositionJsEvent();

                    break;
            }
        }

        private void WPF_LocationChanged(object? sender, EventArgs e)
        {
            AUX_FirePositionJsEvent();
        }

        #endregion

        #region HELPERS

        private Task<string> AUX_Reload(bool ignoreCache)
        {
            return this.WV2?.CoreWebView2?.CallDevToolsProtocolMethodAsync("Page.reload", @"{""ignoreCache"":" + ignoreCache.ToString().ToLower() + "}");
        }

        private void AUX_FireJsEvent(JSEvent nameEvent, params object[] valuesAsJson)
        {
            if (!this.InnerHOWindows.JSEvents.ContainsKey(nameEvent))
                return;

            List<object> keys = new()
            {
                "JSEvent",
                nameEvent.ToString()
            };

            for (int i = 0; i < valuesAsJson.Length; i++)
                keys.Add(valuesAsJson[i]);
            
            string json = Misc.ArrayToJson(keys.ToArray());
            this.InnerHOWindows.JSEvents[nameEvent].Execute(json);
        }

        /// <summary>
        /// Lanza evento en JS de position
        /// </summary>
        private void AUX_FirePositionJsEvent()
        {
            int x = this.X;
            int y = this.Y;

            if (x != this.LastX || y != this.LastY)
            {
                this.LastX = x;
                this.LastY = y;
                AUX_FireJsEvent(JSEvent.position, "X", x, "Y", y);
            }
        }

        private void AUX_FireWinStateJsEvent()
        {
            AUX_FireJsEvent(JSEvent.state, "State", this.InternalState.ToString());
        }

        /// <summary>
        /// Cuando se maximiza la ventana del WPF se descuadra y el contenido se sale de la pantalla
        /// </summary>
        private void AUX_SkewCorrectionMaximized()
        {
            IScreen sc = this.Screen;
            IArea work = sc.WorkingArea;
            //iArea bounds = sc.Bounds;

            //Guardar valores originales de dimesiones maximas y minimas
            AUX_BackupMinMaxSizes();

            base.MinWidth = 1;
            base.MinHeight = 1;

            base.MaxHeight = work.Height + 7;   //Magic number
            base.MaxWidth = work.Width + 13;    //Magic number

            try
            {
                //Magic numbers
                this.WV2.Margin = new Thickness(7, 7, 6, 0);
            }
            catch (Exception) { }
        }

        private void AUX_UndoSkewCorrectionMaximized()
        {
            //Restauras valores maximos y minimos
            AUX_RestoreMinMaxSizes();

            try
            {
                this.WV2.Margin = new Thickness(0, 0, 0, 0);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clip"></param>
        private static void AUX_ClipCursor(IntPtr Handle, bool clip)
        {
            if (!clip)
            {
                User32.ClipCursor(IntPtr.Zero);
                return;
            }

            Screen sc = System.Windows.Forms.Screen.FromHandle(Handle);
            //iScreen sc = this.Screen;

            System.Drawing.Point position;
            _ = User32.GetCursorPos(out position);

            int x = position.X;
            int y = position.Y;

            int left = sc.Bounds.Left;
            int top = sc.Bounds.Top;
            int right = sc.Bounds.Right;
            int bottom = sc.Bounds.Bottom;

            //Para que el cursor del mouse no pueda salir
            System.Drawing.Rectangle r = new(left, top, right, bottom);
            User32.ClipCursor(ref r);

            //Si el cursor está por fuera del WPF, dejarlo en el centro
            if (x < left || x > right || y < top || y > bottom)
                _ = User32.SetCursorPos((int)((right - (left < 0 ? -left : left)) / 2), (int)((bottom - (top < 0 ? -top : top)) / 2));
        }

        private void AUX_BackupMinMaxSizes()
        {
            this.BackupMinWidth = base.MinWidth;
            this.BackupMinHeight = base.MinHeight;
            this.BackupMaxWidth = base.MaxWidth;
            this.BackupMaxHeight = base.MaxHeight;
        }

        private void AUX_RestoreMinMaxSizes()
        {
            base.MinWidth = this.BackupMinWidth;
            base.MinHeight = this.BackupMinHeight;
            base.MaxWidth = this.BackupMaxWidth;
            base.MaxHeight = this.BackupMaxHeight;
        }

        /// <summary>
        /// Restaura el maximo de pantalla
        /// </summary>
        private void AUX_RestoreMaxWidthHeigh()
        {
            base.MaxWidth = this.BackupMaxWidth;
            base.MaxHeight = this.BackupMaxHeight;

            try
            {
                this.WV2.Margin = new Thickness(0, 0, 0, 0);
            }
            catch (Exception) { }
        }

        private void AUX_ClipCursorFullScreen(bool clip)
        {
            //Si NO esta en modo FullScreen NO dejar clippear el cursor
            if (!this.IsFullScreen)
                return;

            this.IsClippedCursor = clip;
            AUX_ClipCursor(this.InnerHandle, clip);
        }

        /// <summary>
        /// Solución bug de animación de minimizar/maximizar
        /// </summary>
        private void AUX_FixAnimationWindow(IntPtr Handle)
        {
            IntPtr myStyle = new IntPtr(WindowStyles.WS_CAPTION | WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_MINIMIZEBOX | WindowStyles.WS_MAXIMIZEBOX | WindowStyles.WS_SYSMENU | WindowStyles.WS_SIZEBOX);
            User32.SetWindowLongPtr(new HandleRef(null, Handle), GetWindowLong.GWL_STYLE, myStyle);
        }

        private void AUX_FireSysMenuJsEvent()
        {
            AUX_FireJsEvent(JSEvent.sysmenu, "X", System.Windows.Forms.Cursor.Position.X , "Y", + System.Windows.Forms.Cursor.Position.Y);
        }

        /// <summary>
        /// WebView snapping behavior handler
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        protected virtual IntPtr AUX_WindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //int WMTaskbarRClick = 0x0313
            //Click derecho sobre icono de barra de tareas
            if (msg == WindowsMessages.WM_SYSMENU)
            {
                if (!this.AllowSysMenu)
                    handled = true;
                
                AUX_FireSysMenuJsEvent();
                return IntPtr.Zero;
            }

            // Soluciona excepción cuando se despliega el sysmenu y se desenfoca
            if (msg != WindowsMessages.WM_SYSCOMMAND)
                return IntPtr.Zero;

            //Click izquierdo sobre alguna opción del SysMenu
            if (msg == WindowsMessages.WM_SYSCOMMAND)
            {
                IntPtr wParamm = wParam;
                int asd = wParam.ToInt32();
                //var item = Items.FirstOrDefault(d => d.ID == wParam);
                //if (item != null)
                //{
                //    item.OnClick();
                //    handled = true;
                //    return IntPtr.Zero;
                //}

                if (wParam == (IntPtr)WM_SystemCommand.SC_MAXIMIZE)
                {
                    this.Maximize();
                    handled = true;
                }

                else if (wParam == (IntPtr)WM_SystemCommand.SC_MINIMIZE)
                {
                    this.Minimize();
                    handled = true;
                }

                else if (wParam == (IntPtr)WM_SystemCommand.SC_RESTORE)
                {
                    this.Restore();
                    handled = true;
                }
            }

            if ((wParam.ToInt32() & ~0x0F) == 0xF010)
            {
                //
                if (this.AllowSnap && this.ResizeMode == ResizeMode.CanMinimize)
                    this.ResizeMode = ResizeMode.CanResize;

                else if (!this.AllowSnap && this.ResizeMode == ResizeMode.CanResize)
                    this.ResizeMode = ResizeMode.CanMinimize;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// "Evento" para permitir o evitar Snapping
        /// </summary>
        private void AUX_SetSnappingEvent()
        {
            HwndSource.FromHwnd(this.InnerHandle).AddHook(AUX_WindowProc);
        }

        private void AUX_SetParameters(Parameters parameters)
        {
            //this.IndexPage = parameters.IndexPage;
            //this.ErrorPage = parameters.ErrorPage;
            //this.Domain = parameters.Domain;
            this.EnableFramePlugins = parameters.EnableFramePlugins;
            this.EnablePlugins = parameters.EnablePlugins;

            this.StartupLocation = parameters.StartupLocation;

            //if (!this.InnerFirstLoad && this.StartupLocation == WVStartupLocation.CenterScreen)
            //{
            //    IScreen screen = this.InnerScreen;
            //    IArea area = screen.WorkingArea;
            //    this.X = (area.Width / 2) - (this.Width / 2) + area.Left;
            //    this.Y = (area.Height / 2) - (this.Height / 2) + area.Top;
            //}
            //else
            //{
            //    this.X = parameters.X;
            //    this.Y = parameters.Y;
            //}

            //this.Language = parameters.Language;
            //this.LocalServer = parameters.LocalServer;

            this.Width = parameters.Width;
            this.Height = parameters.Height;

            this.X = parameters.X;
            this.Y = parameters.Y;

            this.StatusBar = parameters.StatusBar;
            this.GeneralAutofill = parameters.GeneralAutofill;
            this.PasswordAutosave = parameters.PasswordAutosave;
            this.PinchZoom = parameters.PinchZoom;
            this.ZoomControl = parameters.ZoomControl;
            this.DefaultScriptDialogs = parameters.DefaultScriptDialogs;
            this.BrowserAcceleratorKeys = parameters.BrowserAcceleratorKeys;


            this.AllowDrag = parameters.AllowDrag;
            this.AllowSnap = parameters.AllowSnap;
            this.AllowSysMenu = parameters.AllowSysMenu;

            this.Visible = parameters.Visible;
            this.DefaultContextMenus = parameters.DefaultContextMenus;
            this.ZoomFactor = parameters.ZoomFactor;

            this.MaxWidth = parameters.MaxWidth;
            this.MaxHeight = parameters.MaxHeight;

            this.MinWidth = parameters.MinWidth;
            this.MinHeight = parameters.MinHeight;

            this.Topmost = parameters.Topmost;
            this.ClickThrough = parameters.ClickThrough;

            this.Enabled = parameters.Enabled;
            this.PreventClose = parameters.PreventClose;
            this.HotReload = parameters.HotReload;

            this.Icon = parameters.Icon;
            this.Title = parameters.Title;

            this.State = parameters.State;

            TaskBar TB = parameters.TaskBar;
            this.TaskBar.Progress = TB.Progress;
            this.TaskBar.Status = TB.Status;
            this.TaskBar.Visible = TB.Visible;
        }

        #endregion

        //============================================================//
        //============================================================//

        #region HOTRELOAD EVENTS

        // Apaño
        private bool _hotReloaded = false;

        private void Event_HotReloadRefresh(object sender, FileSystemEventArgs e)
        {
            if (_hotReloaded)
            {
                _hotReloaded = false;
                return;
            }

            _hotReloaded = true;

            this.Dispatcher.BeginInvoke(new Action(async () => {
                await this.AUX_Reload(true);
            }));
        }

        private void Event_HotReloadError(object sender, ErrorEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(async () => {
                await this.ExecuteScriptAsync("alert(`" + e.ToString() + "`);");
            }));
        }

        #endregion

    }
}
