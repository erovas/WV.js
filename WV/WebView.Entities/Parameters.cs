using WV.WebView.Enums;

namespace WV.WebView.Entities
{
    public class Parameters
    {
        private static bool IsValidUri(string? uriString)
        {
            if (!Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri))
                return false;

            return uri.Scheme == Uri.UriSchemeFile || uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }

        #region DEFAULT VALUES

        private string _Domain = App.Domain;
        private string? _IndexPage;
        private string? _ErrorPage;
        private TaskBar _TaskBar = new();
        private string _Title = "App - " + App.Domain;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool LocalServer { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public string Domain 
        { 
            get => _Domain; 
            set => _Domain = Uri.TryCreate(value, UriKind.Relative, out _) ? value : _Domain; 
        }

        /// <summary>
        /// 
        /// </summary>
        public string? IndexPage 
        { 
            get => _IndexPage; 
            set => _IndexPage = IsValidUri(value) ? value : _IndexPage;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? ErrorPage
        {
            get => _ErrorPage;
            set => _ErrorPage = IsValidUri(value) ? value : _ErrorPage;
        }

        /// <summary>
        /// Enable/disable plugin access in iframe
        /// </summary>
        public bool EnableFramePlugins { get; set; }

        /// <summary>
        /// Enable/disable plugin access in iframe
        /// </summary>
        public bool EnablePlugins { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public WVState State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TaskBar TaskBar
        {
            get => _TaskBar;
            set => _TaskBar = value ?? _TaskBar;
        }

        /// <summary>
        /// 
        /// </summary>
        public WVStartupLocation StartupLocation { get; set; } = WVStartupLocation.Manual;

        /// <summary>
        /// 
        /// </summary>
        public string Title 
        { 
            get => _Title; 
            set => _Title = string.IsNullOrEmpty(value)? _Title : value; 
        }

        /// <summary>
        /// Gets or sets a value that is a file path to .ico file (32x32) px.
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether this WebView appears in the topmost z-order.
        /// <para>
        /// true if the window is topmost; otherwise, false.
        /// </para>
        /// </summary>
        public bool Topmost { get; set; }

        /// <summary>
        /// Indicates whether all audio output from this WebView is muted or not. Set 
        /// to true will mute this WebView, and set to false will unmute this WebView.
        /// true if audio is muted.
        /// </summary>
        public bool Muted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ClickThrough { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this WebView is enabled in the user interface (UI)
        /// <para>
        /// true if the WebView is enabled; otherwise, false. The default value is true.
        /// </para>
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value that indicates whether this WebView window is visible.
        /// <para>
        /// true if the WebView is visible; otherwise, false. The default value is true.
        /// </para>
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public bool PreventClose { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool AllowDrag { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public bool AllowSnap { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public bool AllowSysMenu { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public bool HotReload { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Width { get; set; } = 800;

        /// <summary>
        /// 
        /// </summary>
        public int Height { get; set; } = 600;

        /// <summary>
        /// 
        /// </summary>
        public int MaxWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxHeight { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MinWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MinHeight { get; set; }

        /// <summary>
        /// Determines whether the status bar is displayed.
        /// 
        /// <para>
        /// Require execute Reload() method if WebView is already showed
        /// </para>
        /// 
        /// <para>
        /// The status bar is usually displayed in the lower left of the WebView and shows
        /// things such as the URI of a link when the user hovers over it and other information.
        /// The default value is true. The status bar UI can be altered by web content and
        /// should not be considered secure.
        /// </para>
        /// </summary>
        public bool StatusBar { get; set; } = true;

        /// <summary>
        /// Determines whether general form information will be saved and autofilled.
        /// 
        /// <para>
        /// Require execute Reload() method if WebView is already showed
        /// </para>
        /// 
        /// <para>
        /// General autofill information includes information like names, street and email
        /// addresses, phone numbers, and arbitrary input. This excludes password information.
        /// When disabled, no suggestions appear, and no new information is saved. When enabled,
        /// information is saved, suggestions appear, and clicking on one will populate the
        /// form fields. The default value is true. It will apply immediately after setting.
        /// </para>
        /// </summary>
        public bool GeneralAutofill { get; set; } = true;

        /// <summary>
        /// Determines whether password information will be autosaved.
        /// 
        /// <para>
        /// When disabled, no new password data is saved and no Save/Update Password prompts
        /// are displayed. However, if there was password data already saved before disabling
        /// this setting, then that password information is auto-populated, suggestions are
        /// shown and clicking on one will populate the fields. When enabled, password information
        /// is auto-populated, suggestions are shown and clicking on one will populate the
        /// fields, new data is saved, and a Save/Update Password prompt is displayed. The
        /// default value is false. It will apply immediately after setting.
        /// </para>
        /// </summary>
        public bool PasswordAutosave { get; set; }

        /// <summary>
        /// Determines the ability of the end users to use pinching motions on touch input
        /// enabled devices to scale the web content in the WebView.
        /// 
        /// <para>
        /// Require execute Reload() method if WebView is already showed
        /// </para>
        /// 
        /// <para>
        /// When disabled, the end users cannot use pinching motions on touch input enabled
        /// devices to scale the web content in the WebView. The default value is true.
        /// Pinch-zoom, referred to as "Page Scale" zoom, is performed as a post-rendering
        /// step, it changes the page scale factor property and scales the surface the web
        /// page is rendered onto when user performs a pinch zooming action. It does not
        /// change the layout but rather changes the viewport and clips the web content,
        /// the content outside of the viewport isn't visible onscreen and users can't reach
        /// this content using mouse. This API only affects the Page Scale zoom and has no
        /// effect on the existing browser zoom properties (ZoomControl and ZoomFactor) or
        /// other end user mechanisms for zooming.
        /// </para>
        /// </summary>
        public bool PinchZoom { get; set; }

        /// <summary>
        /// Determines whether the user is able to impact the zoom of the WebView.
        /// 
        /// <para>
        /// Require execute Reload() method if WebView is already showed
        /// </para>
        /// 
        /// <para>
        /// When disabled, the user is not able to zoom using Ctrl + +, Ctr + -, or Ctrl + mouse wheel,
        /// but the zoom is set using ZoomFactor property. The default value is true.
        /// </para>
        /// </summary>
        public bool ZoomControl { get; set; } = true;

        /// <summary>
        /// The zoom factor for the WebView. This property directly exposes Microsoft.Web.WebView2.Core.CoreWebView2Controller.ZoomFactor,
        /// see its documentation for more info. Getting this property before the Microsoft.Web.WebView2.Core.CoreWebView2
        /// has been initialized will retrieve the last value which was set to it, or 1.0
        /// (the default) if none has been. The most recent value set to this property before
        /// the CoreWebView2 has been initialized will be set on it after initialization.
        /// </summary>
        public double ZoomFactor { get; set; } = 1.0;

        /// <summary>
        /// Determines whether WebView renders the default JavaScript dialog box.
        /// 
        /// <para>
        /// This is used when loading a new HTML document. If set to false, WebView does
        /// not render the default JavaScript dialog box (specifically those displayed by
        /// the JavaScript alert, confirm, prompt functions and beforeunload event). Instead,
        /// WebView raises ScriptDialogs event that contains all of the information for the 
        /// dialog and allow the host app to show a custom UI. The default value is true.
        /// </para>
        /// </summary>
        public bool DefaultScriptDialogs { get; set; } = true;

        /// <summary>
        /// Determines whether browser-specific accelerator keys are enabled.
        /// 
        /// <para>
        /// When this setting is set to false, it disables all accelerator keys that access
        /// features specific to a web browser, including but not limited to:
        /// • Ctrl+F and F3 for Find on Page
        /// • Ctrl+P for Print
        /// • Ctrl+R and F5 for Reload
        /// • Ctrl+Plus and Ctrl+Minus for zooming
        /// • Ctrl+Shift-C and F12 for DevTools
        /// • Special keys for browser functions, such as Back, Forward, and Search
        /// It does not disable accelerator keys related to movement and text editing, such
        /// as:
        /// • Home, End, Page Up, and Page Down
        /// • Ctrl+X, Ctrl+C, Ctrl+V
        /// • Ctrl+A for Select All
        /// • Ctrl+Z for Undo
        /// Those accelerator keys will always be enabled unless they are handled in the
        /// Microsoft.Web.WebView2.Core.CoreWebView2Controller.AcceleratorKeyPressed event.
        /// This setting has no effect on the Microsoft.Web.WebView2.Core.CoreWebView2Controller.AcceleratorKeyPressed
        /// event. The event will be fired for all accelerator keys, whether they are enabled
        /// or not. The default value of AreBrowserAcceleratorKeysEnabled is true.
        /// </para>
        /// </summary>
        public bool BrowserAcceleratorKeys { get; set; } = true;

        /// <summary>
        /// Determines whether the default context menus are shown to the user in WebView.
        /// 
        /// <para>
        /// The default value is true.
        /// </para>
        /// </summary>
        public bool DefaultContextMenus { get; set; } = true;
    }
}
