using WV;
using WV.WebView;
using WV.WebView.Enums;

namespace Essential
{
    public class Window : Plugin, IPlugin
    {
        public static string JScript => Resources.WindowScript;

        private IWebView WVC { get; set; }
        private TaskBar WVTaskBar { get; set; }

        public Window(IWebView webView) : base(webView)
        {
            this.WVC = this.WebView;
            this.WVTaskBar = new TaskBar(this.WVC.TaskBar);
        }

        public Window(IWebView webView, string jsonParameters) : base(webView)
        {
            this.WVC = IWebView.CreateInstance(jsonParameters);
            this.WVTaskBar = new TaskBar(this.WVC.TaskBar);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.Disposed)
                return;

            if (disposing)
            { }

            // Se está recargando el WebView
            if (this.WebView.IsLoadingPage && this.WVC != this.WebView)
                this.WVC.Close(true);

            this.WVC = null;
            this.WVTaskBar = null;
        }

        //-------------------------------------------//
        //-------------------------------------------//

        public string State
        {
            get => WVC.State.ToString();
            set
            {
                if (Enum.TryParse(value, out WVState myStatus))
                    WVC.State = myStatus;
            }
        }
        
        public IScreen Screen => WVC.Screen;

        public TaskBar TaskBar => this.WVTaskBar;

        public IFrame[] Frames => WVC.Frames.ToArray();

        //-------------------------------------------//
        //-------------------------------------------//

        public string? Uri => this.WVC.Uri;
        public bool IsActive => WVC.IsActive;
        public bool IsFocused => WVC.IsFocused;
        public bool IsFullScreen => WVC.IsFullScreen;
        public bool IsLoadingPage => WVC.IsLoadingPage;
        public string? Language => WVC.Language;
        public bool LocalServer => WVC.LocalServer;
        public string Domain => WVC.Domain;
        public string? IndexPage => WVC.IndexPage;
        public string? ErrorPage => WVC.ErrorPage;

        //-------------------------------------------//
        //-------------------------------------------//

        public string StartupLocation
        {
            get => WVC.StartupLocation.ToString();
            set
            {
                if (Enum.TryParse(value, out WVStartupLocation myStatus))
                    WVC.StartupLocation = myStatus;
            }
        }
        public string Title
        {
            get => WVC.Title;
            set => WVC.Title = value;
        }
        public string? Icon
        {
            get => WVC.Icon;
            set => WVC.Icon = value;
        }
        public bool Topmost
        {
            get => WVC.Topmost;
            set => WVC.Topmost = value;
        }
        public bool ClickThrough
        {
            get => WVC.ClickThrough;
            set => WVC.ClickThrough = value;
        }
        public bool Enabled
        {
            get => WVC.Enabled;
            set => WVC.Enabled = value;
        }
        public bool Visible
        {
            get => WVC.Visible;
            set => WVC.Visible = value;
        }
        public bool PreventClose
        {
            get => WVC.PreventClose;
            set => WVC.PreventClose = value;
        }
        public bool AllowDrag
        {
            get => WVC.AllowDrag;
            set => WVC.AllowDrag = value;
        }
        public bool AllowSnap
        {
            get => WVC.AllowSnap;
            set => WVC.AllowSnap = value;
        }
        public bool AllowSysMenu
        {
            get => WVC.AllowSysMenu;
            set => WVC.AllowSysMenu = value;
        }
        public bool HotReload
        {
            get => WVC.HotReload;
            set => WVC.HotReload = value;
        }

        public bool CanGoBack => WVC.CanGoBack;

        public bool CanGoForward => WVC.CanGoForward;

        public bool Muted
        {
            get => WVC.Muted;
            set => WVC.Muted = value;
        }

        public bool IsPlayingAudio => WVC.IsPlayingAudio;

        //-------------------------------------------//
        //-------------------------------------------//

        public bool StatusBar
        {
            get => WVC.StatusBar;
            set => WVC.StatusBar = value;
        }

        public bool GeneralAutofill
        {
            get => WVC.GeneralAutofill;
            set => WVC.GeneralAutofill = value;
        }

        public bool PasswordAutosave
        {
            get => WVC.PasswordAutosave;
            set => WVC.PasswordAutosave = value;
        }

        public bool PinchZoom
        {
            get => WVC.PinchZoom; 
            set => WVC.PinchZoom = value;
        }

        public bool ZoomControl
        {
            get => WVC.ZoomControl; 
            set => WVC.ZoomControl = value;
        }

        public double ZoomFactor
        {
            get => WVC.ZoomFactor; 
            set => WVC.ZoomFactor = value;
        }

        public bool DefaultScriptDialogs
        {
            get => WVC.DefaultScriptDialogs;
            set => WVC.DefaultScriptDialogs = value;
        }

        public bool BrowserAcceleratorKeys
        {
            get => WVC.BrowserAcceleratorKeys; 
            set => WVC.BrowserAcceleratorKeys = value;
        }

        public bool DefaultContextMenus
        {
            get => WVC.DefaultContextMenus; 
            set => WVC.DefaultContextMenus = value;
        }

        //-------------------------------------------//
        //-------------------------------------------//

        #region SIZE

        public int X
        {
            get => WVC.X;
            set => WVC.X = value;
        }
        public int Y
        {
            get => WVC.Y;
            set => WVC.Y = value;
        }
        public int Width
        {
            get => WVC.Width;
            set => WVC.Width = value;
        }
        public int Height
        {
            get => WVC.Height;
            set => WVC.Height = value;
        }
        public int MaxWidth
        {
            get => WVC.MaxWidth;
            set => WVC.MaxWidth = value;
        }
        public int MaxHeight
        {
            get => WVC.MaxHeight;
            set => WVC.MaxHeight = value;
        }
        public int MinWidth
        {
            get => WVC.MinWidth;
            set => WVC.MinWidth = value;
        }
        public int MinHeight
        {
            get => WVC.MinWidth;
            set => WVC.MinHeight = value;
        }

        #endregion

        //-------------------------------------------//
        //-------------------------------------------//

        public void Show()
        {
            WVC.Show();
        }

        public void ShowDialog()
        {
            WVC.ShowDialog();
        }

        public void Close(bool force = false)
        {
            WVC.Close(force);
        }

        //-------------------------------------------//

        public void OpenDevTools()
        {
            WVC.OpenDevTools();
        }

        public void OpenTaskManager()
        {
            WVC.OpenTaskManager();
        }

        public void Reload(bool ignoreCache = false)
        {
            WVC.Reload(ignoreCache);
        }

        public Task<string> ExecuteScript(string script)
        {
            return WVC.ExecuteScriptAsync(script);
        }

        public void SendMessageAsJson(string messageAsJson)
        {
            this.WVC.SendMessageAsJson(messageAsJson);
        }

        public void SendMessageAsString(string messageAsString)
        {
            this.WVC.SendMessageAsString(messageAsString);
        }

        public void GoBack()
        {
            WVC.GoBack();
        }

        public void GoForward()
        {
            WVC.GoForward();
        }

        //-------------------------------------------//
        //-------------------------------------------//

        #region EVENTS

        //void AddReloadEvent(Action method);

        //bool RemoveReloadEvent(Action method);

        #endregion

        //-------------------------------------------//
        //-------------------------------------------//

        #region RESIZE

        /// <summary>
        /// 
        /// </summary>
        public void Drag()
        {
            WVC.Drag();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResizeTopLeft()
        {
            WVC.ResizeTopLeft();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResizeTopRight()
        {
            WVC.ResizeTopRight();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResizeBottomLeft()
        {
            WVC.ResizeBottomLeft();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResizeBottomRight()
        {
            WVC.ResizeBottomRight();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResizeLeft()
        {
            WVC.ResizeLeft();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResizeRight()
        {
            WVC.ResizeRight();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResizeTop()
        {
            WVC.ResizeTop();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResizeBottom()
        {
            WVC.ResizeBottom();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Minimize()
        {
            WVC.Minimize();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Restore()
        {
            WVC.Restore();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Maximize()
        {
            WVC.Maximize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clip"></param>
        public void OpenFullScreen(bool clip = true)
        {
            WVC.OpenFullScreen(clip);
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseFullScreen()
        {
            WVC.CloseFullScreen();
        }

        #endregion


    }
}