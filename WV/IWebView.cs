using System.Collections.Immutable;
using System.Text.Json;
using WV.WebView;
using WV.WebView.Entities;
using WV.WebView.Enums;

namespace WV
{
    public interface IWebView
    {
        private static Type? _WVType;

        public static IWebView CreateInstance(Parameters? parameters, string[]? args = null)
        {
            parameters ??= new Parameters();
            args ??= Array.Empty<string>();

            if(_WVType == null)
            {
                Type itype = typeof(IWebView);
                List<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(s => s.GetTypes())
                                    .Where(p => itype.IsAssignableFrom(p) && p != itype).ToList();

                _WVType = types.FirstOrDefault();
            }

            if (_WVType == null)
                throw new Exception("IWebView is not implemented");

            object? value = Activator.CreateInstance(_WVType, new object[] { parameters, args });

            if (value == null)
                throw new Exception("Impossible create instance");

            return (IWebView)value;
        }

        public static IWebView CreateInstance()
        {
            return CreateInstance(new Parameters());
        }

        public static IWebView CreateInstance(string jsonParameters, string[]? args = null)
        {
            Parameters? parameters = null;

            try
            {
                parameters = JsonSerializer.Deserialize<Parameters>(jsonParameters);
            }
            catch (Exception) { }

            return CreateInstance(parameters, args);
        }

        #region EVENTS

        /// <summary>
        /// Event that is fired when ...
        /// </summary>
        event EventHandler<IWebMessage> WebMessageReceived;

        #endregion

        #region IFRAME

        /// <summary>
        /// Event that is fired when an iframe is added to the DOM
        /// </summary>
        event EventHandler<IFrame> FrameCreated;

        /// <summary>
        /// Event that is fired when an iframe is removed from the DOM
        /// </summary>
        event EventHandler<IFrame> FrameDestroyed;

        /// <summary>
        /// iframes that are in the DOM
        /// </summary>
        ImmutableArray<IFrame> Frames { get; }

        #endregion

        #region READONLY PROPERTIES

        /// <summary>
        /// Gets a value that identify this WebView
        /// </summary>
        string UID { get; }

        /// <summary>
        /// Gets the URI of the current top level document.
        /// </summary>
        string? Uri { get; }

        /// <summary>
        /// Gets a value that indicates wheter this WebView is the main.
        /// <para>
        /// true if the Webview is the main Window
        /// </para>
        /// </summary>
        bool IsMain { get; }

        /// <summary>
        /// Gets a value that indicates whether this WebView is active.
        /// <para>
        /// true if the Webview is active; otherwise, false. The default is false.
        /// </para>
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Gets a value that determines whether this WebView has logical focus.
        /// <para>
        /// true if this element has logical focus; otherwise, false.
        /// </para>
        /// </summary>
        bool IsFocused { get; }

        /// <summary>
        /// Gets a value that determines whether this WebView is FullScreen.
        /// </summary>
        bool IsFullScreen { get; }

        /// <summary>
        /// Gets a value that determines wheter this WebView is loading a page
        /// </summary>
        bool IsLoadingPage { get; }

        /// <summary>
        /// true if the WebView is able to navigate to a previous page in the navigation history
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// true if the WebView is able to navigate to a next page in the navigation history.
        /// </summary>
        bool CanGoForward { get; }

        /// <summary>
        /// Indicates whether any audio output from this CoreWebView2 is playing. 
        /// true if audio is playing even if WebView.Muted is true.
        /// </summary>
        bool IsPlayingAudio { get; }

        #endregion

        #region WebView.Entities.Parameters PROPERTIES

        /// <summary>
        /// 
        /// </summary>
        //string? Language { get; }

        /// <summary>
        /// 
        /// </summary>
        bool LocalServer { get; }

        /// <summary>
        /// 
        /// </summary>
        string Domain { get; }

        /// <summary>
        /// 
        /// </summary>
        string? IndexPage { get; }

        /// <summary>
        /// 
        /// </summary>
        string? ErrorPage { get; }

        /// <summary>
        /// Enable/disable plugin access in iframe
        /// </summary>
        bool EnableFramePlugins { get; set; }

        /// <summary>
        /// Enable/disable plugin access in WebView
        /// </summary>
        bool EnablePlugins { get; set; }

        /// <summary>
        /// 
        /// </summary>
        WVState State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IScreen Screen { get; }

        /// <summary>
        /// 
        /// </summary>
        ITaskBar TaskBar { get; }

        /// <summary>
        /// 
        /// </summary>
        WVStartupLocation StartupLocation { get; set; }

        /// <summary>
        /// Gets or sets a WebView's title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets a value that is a file path to .ico file (32x32) px.
        /// </summary>
        string? Icon { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether this WebView appears in the topmost z-order.
        /// <para>
        /// true if the window is topmost; otherwise, false.
        /// </para>
        /// </summary>
        bool Topmost { get; set; }

        /// <summary>
        /// Indicates whether all audio output from this WebView is muted or not. Set 
        /// to true will mute this WebView, and set to false will unmute this WebView.
        /// true if audio is muted.
        /// </summary>
        bool Muted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool ClickThrough { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this WebView is enabled in the user interface (UI)
        /// <para>
        /// true if the WebView is enabled; otherwise, false. The default value is true.
        /// </para>
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether this WebView window is visible.
        /// <para>
        /// true if the WebView is visible; otherwise, false. The default value is true.
        /// </para>
        /// </summary>
        bool Visible { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        bool PreventClose { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool AllowDrag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool AllowSnap { get; set; }

        /// <summary>
        /// right-click menú
        /// </summary>
        bool AllowSysMenu { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        bool HotReload { get; set; }

        //-------------------------------------------//
        //-------------------------------------------//

        /// <summary>
        /// 
        /// </summary>
        int X { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int Y { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int Width { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        int Height { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        int MaxWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int MaxHeight { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int MinWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int MinHeight { get; set; }

        //-------------------------------------------//
        //-------------------------------------------//

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
        bool StatusBar { get; set; }

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
        bool GeneralAutofill { get; set; }

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
        bool PasswordAutosave { get; set; }

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
        bool PinchZoom { get; set; }

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
        bool ZoomControl { get; set; }

        /// <summary>
        /// The zoom factor for the WebView. This property directly exposes Microsoft.Web.WebView2.Core.CoreWebView2Controller.ZoomFactor,
        /// see its documentation for more info. Getting this property before the Microsoft.Web.WebView2.Core.CoreWebView2
        /// has been initialized will retrieve the last value which was set to it, or 1.0
        /// (the default) if none has been. The most recent value set to this property before
        /// the CoreWebView2 has been initialized will be set on it after initialization.
        /// </summary>
        double ZoomFactor { get; set; }

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
        bool DefaultScriptDialogs { get; set; }

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
        bool BrowserAcceleratorKeys { get; set; }

        /// <summary>
        /// Determines whether the default context menus are shown to the user in WebView.
        /// 
        /// <para>
        /// The default value is true.
        /// </para>
        /// </summary>
        bool DefaultContextMenus { get; set; }

        #endregion

        #region RESIZE METHODS

        /// <summary>
        /// 
        /// </summary>
        void Drag();

        /// <summary>
        /// 
        /// </summary>
        void ResizeTopLeft();

        /// <summary>
        /// 
        /// </summary>
        void ResizeTopRight();

        /// <summary>
        /// 
        /// </summary>
        void ResizeBottomLeft();

        /// <summary>
        /// 
        /// </summary>
        void ResizeBottomRight();

        /// <summary>
        /// 
        /// </summary>
        void ResizeLeft();

        /// <summary>
        /// 
        /// </summary>
        void ResizeRight();

        /// <summary>
        /// 
        /// </summary>
        void ResizeTop();

        /// <summary>
        /// 
        /// </summary>
        void ResizeBottom();

        /// <summary>
        /// 
        /// </summary>
        void Minimize();

        /// <summary>
        /// 
        /// </summary>
        void Restore();

        /// <summary>
        /// 
        /// </summary>
        void Maximize();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clip"></param>
        void OpenFullScreen(bool clip = true);

        /// <summary>
        /// 
        /// </summary>
        void CloseFullScreen();

        #endregion

        #region METHODS

        /// <summary>
        /// 
        /// </summary>
        void Show();

        /// <summary>
        /// 
        /// </summary>
        void ShowDialog();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        void Close(bool force = false);

        //-------------------------------------------//

        /// <summary>
        /// 
        /// </summary>
        void OpenDevTools();

        /// <summary>
        /// 
        /// </summary>
        void OpenTaskManager();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ignoreCache"></param>
        void Reload(bool ignoreCache = false);

        /// <summary>
        /// Runs JavaScript code from the javaScript parameter in the current WebView
        /// </summary>
        /// <param name="javaScript"></param>
        Task<string> ExecuteScriptAsync(string javaScript);

        /// <summary>
        /// Posts the specified messageAsJson to the current WebView.
        /// </summary>
        /// <param name="messageAsJson"></param>
        void SendMessageAsJson(string messageAsJson);

        /// <summary>
        /// Posts a message that is a simple string rather than a JSON string representation
        /// of a JavaScript object.
        /// </summary>
        /// <param name="messageAsString"></param>
        void SendMessageAsString(string messageAsString);

        /// <summary>
        /// 
        /// </summary>
        void GoBack();

        /// <summary>
        /// 
        /// </summary>
        void GoForward();

        #endregion

    }
}
