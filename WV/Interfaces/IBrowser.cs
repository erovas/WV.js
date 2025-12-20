using WV.Enums;
using static WV.AppManager;

namespace WV.Interfaces
{
    public interface IBrowser
    {
        /// <summary>
        /// PlayingAudio Event C#
        /// </summary>
        event WVEventHandler<bool> PlayingAudio;

        /// <summary>
        /// Muted Event C#
        /// </summary>
        event WVEventHandler<bool> MutedEvent;

        /// <summary>
        /// ZoomFactorChanged Event C#
        /// </summary>
        event WVEventHandler<double> ZoomFactorChanged;

        /// <summary>
        /// StatusBarTextChanged Event C#
        /// </summary>
        event WVEventHandler<string> StatusBarTextChanged;

        #region PROPS

        /// <summary>
        /// Gets the URI of the current top level document.
        /// </summary>
        string Uri { get; }

        /// <summary>
        /// Enable or disable automatic browser reloading, use it during development. Default value is false.
        /// </summary>
        bool HotReload { get; set; }

        /// <summary>
        /// Gets a value that determines wheter this WebView is loading a page.
        /// </summary>
        //bool IsLoadingPage { get; }

        /// <summary>
        /// true if the WebView is able to navigate to a previous page in the navigation history.
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// true if the WebView is able to navigate to a next page in the navigation history.
        /// </summary>
        bool CanGoForward { get; }

        /// <summary>
        /// Indicates whether any audio output from this CoreWebView2 is playing. 
        /// true if audio is playing even if IBrowser.Muted is true.
        /// </summary>
        bool IsPlayingAudio { get; }

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
        /// It does not disable accelerator keys related to movement and text editing, such as:
        /// • Home, End, Page Up, and Page Down
        /// • Ctrl+X, Ctrl+C, Ctrl+V
        /// • Ctrl+A for Select All
        /// • Ctrl+Z for Undo
        /// </para>
        /// </summary>
        bool AcceleratorKeys { get; set; }

        /// <summary>
        /// Determines whether the end user to use swiping gesture on touch input enabled devices to navigate in WebView2.
        /// <para>
        /// Swiping gesture navigation on touch screen includes:
        /// • Swipe left/right (swipe horizontally) to navigate to previous/next page in navigation history.
        /// </para>
        /// </summary>
        bool SwipeNavigation { get; set; }

        /// <summary>
        /// Gets or sets the zoom factor for the WebView.
        /// 
        /// <para>
        /// Note that changing zoom factor may cause window.innerWidth or window.innerHeight
        /// and page layout to change.A zoom factor that is applied by the host by setting
        /// this ZoomFactor property becomes the new default zoom for the WebView.This zoom
        /// factor applies across navigations and is the zoom factor WebView is returned
        /// to when the user presses Ctrl+0. Specifying a ZoomFactor less than
        /// or equal to 0 is not allowed. WebView also has an internal supported zoom factor
        /// range.When a specified zoom factor is out of that range, it is normalized to
        /// be within the range event is raised for the real applied zoom factor. 
        /// </para>
        /// </summary>
        double ZoomFactor { get; set; }

        /// <summary>
        /// Gets maximum ZommFactor.
        /// </summary>
        public double MaxZoomFactor { get; }

        /// <summary>
        /// Gets minimum ZoomFactor.
        /// </summary>
        public double MinZoomFactor { get; }

        /// <summary>
        /// When WebView is reload, reset all settings to default. Default value is false.
        /// </summary>
        bool ResetWebViewOnReload { get; set; }

        /// <summary>
        /// Determines whether the default context menus are shown to the user in WebView.
        /// </summary>
        IContextMenu ContextMenu { get; }

        /// <summary>
        /// Indicates whether all audio output from this WebView2 is muted or not. Set
        /// to true will mute this CoreWebView2, and set to false will unmute this WebView2.
        /// true if audio is muted.
        /// </summary>
        bool Muted { get; set; }

        /// <summary>
        /// Get last status bar text.
        /// </summary>
        string StatusBarText { get; }

        /// <summary>
        /// Get current language.
        /// </summary>
        string Language { get; }

        /// <summary>
        /// Get or set browser color scheme.
        /// </summary>
        BrowserColorScheme ColorScheme { get; set; }

        /// <summary>
        /// Gets or sets the browser's color scheme by text.
        /// </summary>
        string ColorSchemeText { get; set; }

        #endregion

        //-------------------------------------------//

        #region METHODS

        /// <summary>
        /// Open developer tools [DevTools].
        /// </summary>
        void OpenDevTools();

        /// <summary>
        /// Navigate to a specific URI.
        /// </summary>
        void Navigate(string uri);

        /// <summary>
        /// Reload the page.
        /// </summary>
        void Reload();

        /// <summary>
        /// Reload the page avoiding the cache.
        /// </summary>
        void HardReload();

        /// <summary>
        /// Runs JavaScript code from the javaScript parameter in the current WebView.
        /// </summary>
        /// <param name="javaScript"></param>
        Task<string>? ExecuteScriptAsync(string javaScript);

        /// <summary>
        /// Navigates the WebView to the previous page in the navigation history.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Navigates the WebView to the next page in the navigation history.
        /// </summary>
        void GoForward();

        #endregion

        //-------------------------------------------//

        #region EVENTS

        /// <summary>
        /// Playing Audio event JS.
        /// </summary>
        //object? OnPlayingAudio { get; set; }

        /// <summary>
        /// Muted Event JS.
        /// </summary>
        //object? OnMuted {  get; set; }

        /// <summary>
        /// ZoomFactor changed event JS.
        /// </summary>
        //object? OnZoomFactorChanged { get; set; }

        /// <summary>
        /// StatusBarTextChanged event JS 
        /// </summary>
        //object? OnStatusBarTextChanged { get; set; }

        /// <summary>
        /// Appends an event listener for events whose type attribute value is type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        //void AddEventListener(string type, object listener);

        /// <summary>
        /// Removes the event listener in target's event listener list with the same type and callback.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        //void RemoveEventListener(string type, object listener);

        #endregion

    }
}