using System.Collections.Immutable;
using System.Windows.Media;
using WV.WebView;
using WV.WebView.Entities;
using WV.WebView.Enums;
using System.Windows;
using WV.Windows.Constants;
using WV.Windows.Utils;
using System.IO;
using System.Reflection.Metadata;

namespace WV.Windows
{
    public partial class WebView : IWebView
    {
        #region EVENTS

        public event EventHandler<IWebMessage> WebMessageReceived;

        #endregion

        #region IFRAME

        public event EventHandler<IFrame> FrameCreated;

        public event EventHandler<IFrame> FrameDestroyed;

        public ImmutableArray<IFrame> Frames => ImmutableArray.Create<IFrame>(InnerFrames.ToArray());

        #endregion

        #region READONLY PUBLIC PROPERTIES

        public string UID { get; }

        public string? Uri => this.IsWV2Ready ? this.WV2.CoreWebView2.Source : null;

        public new bool IsActive => base.IsActive;

        public new bool IsFocused => this.IsWV2Ready ? this.WV2.IsFocused : false;

        public bool IsFullScreen { get; private set; }

        public bool IsLoadingPage { get; private set; }

        public bool CanGoBack
        {
            get
            {
                if (this.IsWV2Ready)
                    return this.WV2.CoreWebView2.CanGoBack;

                return false;
            }
        }

        public bool CanGoForward
        {
            get
            {
                if (this.IsWV2Ready)
                    return this.WV2.CoreWebView2.CanGoForward;

                return false;
            }
        }

        public bool IsPlayingAudio
        {
            get
            {
                if (this.IsWV2Ready)
                    return this.WV2.CoreWebView2.IsDocumentPlayingAudio;

                return false;
            }
        }

        #endregion

        #region PUBLIC PROPERTIES

        public new string? Language { get; }

        public bool LocalServer { get; }

        public string Domain { get; }
        
        public string? IndexPage { get; }

        public string? ErrorPage { get; }

        public bool EnableFramePlugins { get; set; }

        public bool EnablePlugins 
        { 
            get => this.InnerHOWindows.InnerEnablePlugins;
            set => this.InnerHOWindows.InnerEnablePlugins = value;
        }

        public WVState State 
        { 
            get => this.InternalState; 
            set
            {
                switch (value)
                {
                    case WVState.Minimized:
                        this.Minimize();
                        break;

                    case WVState.Maximized:
                        this.Maximize();
                        break;

                    case WVState.FullScreen:
                        this.OpenFullScreen();
                        break;

                    default:
                        this.Restore();
                        break;
                }
            }
        }

        public IScreen Screen => new Webview.Screen(System.Windows.Forms.Screen.FromHandle(this.InnerHandle));

        public ITaskBar TaskBar { get; }

        private WVStartupLocation _StartupLocation;
        public WVStartupLocation StartupLocation 
        { 
            get => _StartupLocation;
            set
            {
                _StartupLocation = value;

                switch (value)
                {
                    case WVStartupLocation.CenterScreen:
                        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        break;
                    default:
                        this.WindowStartupLocation = WindowStartupLocation.Manual;
                        break;
                }
            }
        }

        public new string Title
        {
            get => base.Title;
            set => base.Title = string.IsNullOrEmpty(value) ? base.Title : value;
        }

        private string? _Icon;
        public new string? Icon 
        { 
            get => _Icon; 
            set
            {
                try
                {
                    base.Icon = System.Windows.Media.Imaging.BitmapFrame.Create(new Uri(value, UriKind.RelativeOrAbsolute));
                    _Icon = value;
                }
                catch (Exception) { }
            }
        }

        public new bool Topmost 
        { 
            get => base.Topmost; 
            set => base.Topmost = value; 
        }

        public bool Muted
        {
            get
            {
                if (this.IsWV2Ready)
                    return this.WV2.CoreWebView2.IsMuted;

                return false;
            }
            set
            {
                if (this.IsWV2Ready)
                    this.WV2.CoreWebView2.IsMuted = value;
            }
        }

        public bool ClickThrough 
        {
            get => this.Background == System.Windows.Media.Brushes.Transparent;
            set
            {
                if (value)
                    this.Background = System.Windows.Media.Brushes.Transparent;
                else
                    this.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#01000000");
            }
        }

        public bool Enabled 
        {
            get => this.IsEnabled;
            set => this.IsEnabled = value;
        }

        private bool _Visible;
        public bool Visible
        {
            get
            {
                if (!this.IsWV2Ready)
                    return _Visible;

                return this.Visibility == System.Windows.Visibility.Visible && _Visible;
            }
            
            set
            {
                _Visible = value;

                if (this.IsWV2Ready)
                    this.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            }
        }
        
        public bool PreventClose { get; set; }
        public bool AllowDrag { get; set; }
        public bool AllowSnap { get; set; }
        public bool AllowSysMenu { get; set; }

        public bool _HotReload;
        private FileSystemWatcher? _watcher;
        public bool HotReload 
        { 
            get => _HotReload;
            set
            {
                _HotReload = value;

                if (value)
                {
                    if (_watcher != null)
                        return;

                    string StaticPath = AppManager.SrcPath;

                    try
                    {
                        _watcher = new FileSystemWatcher(StaticPath);

                        _watcher.NotifyFilter = NotifyFilters.DirectoryName |
                                                NotifyFilters.FileName |
                                                NotifyFilters.LastWrite;

                        _watcher.Changed += Event_HotReloadRefresh;
                        _watcher.Error += Event_HotReloadError;

                        _watcher.Filter = "";
                        _watcher.IncludeSubdirectories = true;
                        _watcher.EnableRaisingEvents = true;
                    }
                    catch (Exception ex)
                    {
                        _watcher = null;
                    }

                    
                    return;
                }

                if (_watcher == null)
                    return;

                _watcher.Changed -= Event_HotReloadRefresh;
                _watcher.Error -= Event_HotReloadError;
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        //-------------------------------------------//
        //-------------------------------------------//

        public int X
        {
            get
            {
                if (!this.IsWV2Ready)
                    return (int)this.Left;

                if (this.WindowState == WindowState.Maximized ||
                    (
                        this.WindowState == WindowState.Minimized
                        &&
                        this.LastState == WVState.Maximized
                    )
                   )
                {
                    IScreen sc = this.Screen;
                    return (int)sc.WorkingArea.Left;
                }

                return (int)this.Left;
            }
            set
            {
                //En fullscreen NO se puede cambiar de posición la ventana
                if (this.IsFullScreen)
                    return;

                this.Left = value;
            }
        }

        public int Y
        {
            get
            {
                if (!this.IsWV2Ready)
                    return (int)this.Top;

                if (this.WindowState == WindowState.Maximized ||
                    (
                        this.WindowState == WindowState.Minimized
                        &&
                        this.LastState == WVState.Maximized
                    )
                   )
                {
                    IScreen sc = this.Screen;
                    return (int)sc.WorkingArea.Top;
                }

                return (int)this.Top;
            }

            set
            {
                //En fullscreen NO se puede cambiar de posición la ventana
                if (this.IsFullScreen)
                    return;

                this.Top = value;
            }
        }

        public new int Width
        {
            get
            {
                if (!this.IsWV2Ready)
                    return (int)base.Width;

                return (int)WV2.ActualWidth;
            }
            set
            {
                //En fullscreen NO se puede modificar el ancho o alto de la ventana
                if (this.IsFullScreen || value < 1)
                    return;

                if (base.MaxWidth < value)
                    value = (int)base.MaxWidth;

                base.Width = value;
            }
        }

        public new int Height
        {
            get
            {
                if (!this.IsWV2Ready)
                    return (int)base.Height;

                return (int)WV2.ActualHeight;
            }
            set
            {
                //En fullscreen NO se puede modificar el ancho o alto de la ventana
                if (this.IsFullScreen || value < 1)
                    return;

                //Limitar el alto
                if (base.MaxHeight < value)
                    value = (int)base.MaxHeight;

                base.Height = value;
            }
        }

        public new int MaxWidth
        {
            get
            {
                return double.IsPositiveInfinity(base.MaxWidth) ? 0 : (int)base.MaxWidth;
            }
            set
            {
                //En fullscreen NO se puede modificar el ancho o alto de la ventana
                if (this.IsFullScreen)
                    return;

                if (value < 1)
                {
                    this.BackupMaxWidth = double.PositiveInfinity;
                    base.MaxWidth = double.PositiveInfinity;
                    return;
                }

                this.BackupMaxWidth = value;
                base.MaxWidth = value;
            }
        }

        public new int MaxHeight
        {
            get
            {
                return double.IsPositiveInfinity(base.MaxHeight) ? 0 : (int)base.MaxHeight;
            }
            set
            {
                //En fullscreen NO se puede modificar el ancho o alto de la ventana
                if (this.IsFullScreen)
                    return;

                if (value < 1)
                {
                    this.BackupMaxHeight = double.PositiveInfinity;
                    base.MaxHeight = double.PositiveInfinity;
                    return;
                }

                this.BackupMaxHeight = value;
                base.MaxHeight = value;
            }
        }

        public new int MinWidth
        {
            get => (int)base.MinWidth;
            set => base.MinWidth = value < 1 ? 1 : value;
        }

        public new int MinHeight
        {
            get => (int)base.MinHeight;
            set => base.MinHeight = value < 1 ? 1 : value;
        }

        //-------------------------------------------//
        //-------------------------------------------//

        private bool _StatusBar;
        public bool StatusBar
        {
            get
            {
                if (!this.IsWV2Ready)
                    return _StatusBar;

                return this.WV2.CoreWebView2.Settings.IsStatusBarEnabled;
            }
            set
            {
                _StatusBar = value;

                if (this.IsWV2Ready)
                    this.WV2.CoreWebView2.Settings.IsStatusBarEnabled = value;
            }
        }

        private bool _GeneralAutofill;
        public bool GeneralAutofill
        {
            get
            {
                if (!this.IsWV2Ready)
                    return _GeneralAutofill;

                return this.WV2.CoreWebView2.Settings.IsGeneralAutofillEnabled;
            }
            set
            {
                _GeneralAutofill = value;

                if (this.IsWV2Ready)
                    this.WV2.CoreWebView2.Settings.IsGeneralAutofillEnabled = value;
            }
        }

        private bool _PasswordAutosave;
        public bool PasswordAutosave
        {
            get
            {
                if (!this.IsWV2Ready)
                    return _PasswordAutosave;

                return this.WV2.CoreWebView2.Settings.IsPasswordAutosaveEnabled;
            }
            set
            {
                _PasswordAutosave = value;

                if (this.IsWV2Ready)
                    this.WV2.CoreWebView2.Settings.IsPasswordAutosaveEnabled = value;
            }
        }

        private bool _PinchZoom;
        public bool PinchZoom
        {
            get
            {
                if (!this.IsWV2Ready)
                    return _PinchZoom;

                return this.WV2.CoreWebView2.Settings.IsPinchZoomEnabled;
            }
            set
            {
                _PinchZoom = value;

                if (this.IsWV2Ready)
                    this.WV2.CoreWebView2.Settings.IsPinchZoomEnabled = value;
            }
        }

        private bool _ZoomControl;
        public bool ZoomControl
        {
            get
            {
                if (!this.IsWV2Ready)
                    return _ZoomControl;

                return this.WV2.CoreWebView2.Settings.IsZoomControlEnabled;
            }
            set
            {
                _ZoomControl = value;

                if (this.IsWV2Ready)
                    this.WV2.CoreWebView2.Settings.IsZoomControlEnabled = value;
            }
        }

        private double _ZoomFactor;
        public double ZoomFactor
        {
            get
            {
                if (!this.IsWV2Ready)
                    return _ZoomFactor;

                return this.WV2.ZoomFactor;
            }
            set
            {
                _ZoomFactor = value;

                if (this.IsWV2Ready)
                    this.WV2.ZoomFactor = value;
            }
        }

        private bool _DefaultScriptDialogs;
        public bool DefaultScriptDialogs
        {
            get
            {
                if (!this.IsWV2Ready)
                    return _DefaultScriptDialogs;

                return this.WV2.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled;
            }
            set
            {
                _DefaultScriptDialogs = value;

                if (this.IsWV2Ready)
                    this.WV2.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = value;
            }
        }

        private bool _BrowserAcceleratorKeys;
        public bool BrowserAcceleratorKeys
        {
            get
            {
                if (!this.IsWV2Ready)
                    return _BrowserAcceleratorKeys;

                return this.WV2.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled;
            }
            set
            {
                _BrowserAcceleratorKeys = value;

                if (this.IsWV2Ready)
                    this.WV2.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = value;
            }
        }

        private bool _DefaultContextMenus;
        public bool DefaultContextMenus
        {
            get
            {
                if (!this.IsWV2Ready)
                    return _DefaultContextMenus;

                return this.WV2.CoreWebView2.Settings.AreDefaultContextMenusEnabled;
            }
            set
            {
                _DefaultContextMenus = value;

                if (this.IsWV2Ready)
                    this.WV2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = value;
            }
        }

        #endregion

        public WebView(Parameters parameters) : this()
        {
            this.InnerParameters = parameters;

            this.UID = Guid.NewGuid().ToString();
            this.TaskBar = new Webview.TaskBar(this);

            this.IndexPage = parameters.IndexPage;
            this.ErrorPage = parameters.ErrorPage;
            this.Domain = parameters.Domain;
            this.Language = parameters.Language;
            this.LocalServer = parameters.LocalServer;

            AUX_SetParameters(parameters);
        }

        #region RESIZE METHODS

        public void Drag()
        {
            if (this.IsFullScreen)
                return;

            if (this.AllowDrag)
            {
                SynchronizationContext.Current.Post(x => {
                    User32.ReleaseCapture();
                    User32.SendMessage(this.InnerHandle, WindowsMessages.WM_NCLBUTTONDOWN, WM_NCHitTest.HTCAPTION, 0);
                }, null);
            }
        }

        public void ResizeTopLeft()
        {
            if (this.IsFullScreen)
                return;

            SynchronizationContext.Current.Post(x => {
                User32.ReleaseCapture();
                User32.SendMessage(this.InnerHandle, WindowsMessages.WM_NCLBUTTONDOWN, WM_NCHitTest.HTTOPLEFT, 0);
            }, null);
        }

        public void ResizeTopRight()
        {
            if (this.IsFullScreen)
                return;

            SynchronizationContext.Current.Post(x => {
                User32.ReleaseCapture();
                User32.SendMessage(this.InnerHandle, WindowsMessages.WM_NCLBUTTONDOWN, WM_NCHitTest.HTTOPRIGHT, 0);
            }, null);
        }

        public void ResizeBottomLeft()
        {
            if (this.IsFullScreen)
                return;

            SynchronizationContext.Current.Post(x => {
                User32.ReleaseCapture();
                User32.SendMessage(this.InnerHandle, WindowsMessages.WM_NCLBUTTONDOWN, WM_NCHitTest.HTBOTTOMLEFT, 0);
            }, null);
        }

        public void ResizeBottomRight()
        {
            if (this.IsFullScreen)
                return;

            SynchronizationContext.Current.Post(x => {
                User32.ReleaseCapture();
                User32.SendMessage(this.InnerHandle, WindowsMessages.WM_NCLBUTTONDOWN, WM_NCHitTest.HTBOTTOMRIGHT, 0);
            }, null);
        }

        public void ResizeLeft()
        {
            if (this.IsFullScreen)
                return;

            SynchronizationContext.Current.Post(x => {
                User32.ReleaseCapture();
                User32.SendMessage(this.InnerHandle, WindowsMessages.WM_NCLBUTTONDOWN, WM_NCHitTest.HTLEFT, 0);
            }, null);
        }

        public void ResizeRight()
        {
            if (this.IsFullScreen)
                return;

            SynchronizationContext.Current.Post(x => {
                User32.ReleaseCapture();
                User32.SendMessage(this.InnerHandle, WindowsMessages.WM_NCLBUTTONDOWN, WM_NCHitTest.HTRIGHT, 0);
            }, null);
        }

        public void ResizeTop()
        {
            if (this.IsFullScreen)
                return;

            SynchronizationContext.Current.Post(x => {
                User32.ReleaseCapture();
                User32.SendMessage(this.InnerHandle, WindowsMessages.WM_NCLBUTTONDOWN, WM_NCHitTest.HTTOP, 0);
            }, null);
        }

        public void ResizeBottom()
        {
            if (this.IsFullScreen)
                return;

            SynchronizationContext.Current.Post(x => {
                User32.ReleaseCapture();
                User32.SendMessage(this.InnerHandle, WindowsMessages.WM_NCLBUTTONDOWN, WM_NCHitTest.HTBOTTOM, 0);
            }, null);
        }

        public void Restore()
        {
            if (this.IsFullScreen)
            {
                if (this.InternalState == WVState.Minimized)
                    this.WindowState = WindowState.Normal;
                return;
            }

            this.NoFireStateEvent = true;
            this.WindowState = WindowState.Normal;
            this.WindowState = WindowState.Normal;
            this.NoFireStateEvent = false;
        }

        public void Maximize()
        {
            if (this.IsFullScreen)
            {
                if (this.InternalState == WVState.Minimized)
                    this.WindowState = WindowState.Normal;

                return;
            }

            this.WindowState = WindowState.Maximized;
        }

        public void Minimize()
        {
            this.WindowState = WindowState.Minimized;
        }

        public void OpenFullScreen(bool clip = true)
        {
            //Ya está en modo FullScreen, no hacer nada
            if (this.IsFullScreen)
                return;

            IScreen sc = this.Screen;

            //Si las dimensiones Maximas definidas por el usuario son menores a la de la pantalla actual
            //Si las dimensiones Minimas definidas por el usuario son mayores a la de la pantalla actual
            //NO dejar poner en FullScreen
            if (this.InternalState != WVState.Maximized &&
                (
                    base.MaxWidth < sc.Bounds.Width
                    ||
                    base.MaxHeight < sc.Bounds.Height
                    ||
                    base.MinWidth > sc.Bounds.Width
                    ||
                    base.MinHeight > sc.Bounds.Height
                )
               )
                return;

            //Si las dimensiones Minimas NO son menores o iguales al Screen actual, NO dejar poner en modo FullScreen
            // o sea, las dimensiones minimas definidas por el usuario son mayores al tamaño de la pantalla
            if (base.MinWidth > sc.Bounds.Width || base.MinHeight > sc.Bounds.Height)
                return;


            if (this.InternalState != WVState.Minimized)
                this.LastState = this.InternalState;

            //Si está Maximizado o Minimizado
            if (this.WindowState == WindowState.Maximized || this.WindowState == WindowState.Minimized)
            {
                //Quita corrección de descuadre del WPF cuando está maximizado
                AUX_RestoreMaxWidthHeigh();

                //Indicamos que NO se quiere disparar evento JS de State
                this.NoFireStateEvent = true;
                this.InternalState = WVState.Restore;

                //Hay que hacerlo 2 veces para asegurar que se "Normaliza" la pantalla
                this.WindowState = WindowState.Normal;
                this.WindowState = WindowState.Normal;

                //Indicamos que ahora SÍ queremos dejar disparar evento JS de State
                this.NoFireStateEvent = false;
            }

            //Se guarda las dimensiones y posición de la ventana
            this.LastWidth = base.Width;
            this.LastHeight = base.Height;
            this.LastLeft = this.Left;
            this.LastTop = this.Top;

            //Hacemos la ventana que tenga las dimensiones de toda la pantalla
            base.Width = sc.Bounds.Width;
            base.Height = sc.Bounds.Height;

            //Ubicamos en las coordenadas 0,0 de la pantalla en la que está
            this.Left = sc.Bounds.Left;
            this.Top = sc.Bounds.Top;

            //Ya indicamos al desarrollador que el State del WebView está en modo FullScreen
            this.InternalState = WVState.FullScreen;
            this.IsFullScreen = true;
            
            //Disparamos los evento JS de State y de Position
            AUX_FireWinStateJsEvent();
            AUX_FirePositionJsEvent();

            //Para que el cursor del mouse no pueda salir de la ventana
            AUX_ClipCursorFullScreen(clip);
        }

        public void CloseFullScreen()
        {
            //NO está en modo FullScreen, no hacer nada
            if (!this.IsFullScreen)
                return;

            //Liberar el cursor del mouse (si lo está)
            AUX_ClipCursorFullScreen(false);

            //Si está minimizado, se normaliza primero
            if (this.WindowState == WindowState.Minimized)
            {
                //Indicamos que NO se quiere disparar evento JS de State
                this.NoFireStateEvent = true;

                //Esto haría saltar el evento JS de State
                this.WindowState = WindowState.Normal;

                //Indicamos que ahora SÍ queremos dejar disparar evento JS de State
                this.NoFireStateEvent = false;
            }

            //Se indica que ya NO está en modo FullScreen
            this.IsFullScreen = false;

            //Se restaura el maximo ancho y alto que habia definido el usuario
            AUX_RestoreMaxWidthHeigh();

            //Se restaura las dimensiones y posición original de la ventana
            base.Width = this.LastWidth;
            base.Height = this.LastHeight;
            this.Left = this.LastLeft;
            this.Top = this.LastTop;

            //Se recupera el State en el que estaba antes de pasar a FullScreen
            switch (this.LastState)
            {
                //Como se ha normalizado la ventana, entonces simulamos el disparo del evento JS de State
                case WVState.Restore:
                    this.InternalState = WVState.Restore;
                    AUX_FireWinStateJsEvent();
                    AUX_FirePositionJsEvent();
                    break;

                //Como primero se ha normalizado, pues hacemos disparar el evento JS de State
                case WVState.Maximized:
                    this.WindowState = WindowState.Maximized;
                    break;
            }
        }

        #endregion

        #region PUBLIC METHODS

        public new void Show()
        {
            SynchronizationContext.Current.Post(x => {
                try
                {
                    base.Show();
                }
                catch (Exception) { }

            }, null);
        }

        public new void ShowDialog()
        {
            SynchronizationContext.Current.Post(x => {
                try
                {
                    base.ShowDialog();
                }
                catch (Exception) { }

            }, null);
        }

        public void Close(bool force = false)
        {
            if(force) 
                this.PreventClose = false;

            base.Close();
        }

        //-------------------------------------------//

        public void OpenDevTools()
        {
            this.WV2?.CoreWebView2?.OpenDevToolsWindow();
        }

        public void OpenTaskManager()
        {
            this.WV2?.CoreWebView2?.OpenTaskManagerWindow();
        }

        public void Reload(bool ignoreCache = false)
        {
            _ = AUX_Reload(ignoreCache);
        }

        public Task<string> ExecuteScriptAsync(string javaScript)
        {
            return this.WV2?.ExecuteScriptAsync(javaScript);
        }

        public void SendMessageAsJson(string messageAsJson)
        {
            this.WV2?.CoreWebView2?.PostWebMessageAsJson(messageAsJson);
        }

        public void SendMessageAsString(string messageAsString)
        {
            this.WV2?.CoreWebView2?.PostWebMessageAsString(messageAsString);
        }

        public void GoBack()
        {
            this.WV2?.CoreWebView2?.GoBack();
        }

        public void GoForward()
        {
            this.WV2?.CoreWebView2?.GoForward();
        }
        
        #endregion

    }
}