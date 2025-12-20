using static WV.AppManager;
using WV.Enums;

namespace WV.Interfaces
{
    public interface IWindow
    {
        /// <summary>
        /// StateChanged event.
        /// </summary>
        event WVEventHandler<WindowState, string> StateChanged;

        /// <summary>
        /// Close event.
        /// </summary>
        event WVEventHandler Closing;

        /// <summary>
        /// PositionChanged event.
        /// </summary>
        event WVEventHandler<int, int> PositionChanged;

        /// <summary>
        /// Activated event.
        /// </summary>
        event WVEventHandler<bool> Activated;

        /// <summary>
        /// Enabled event.
        /// </summary>
        event WVEventHandler<bool> EnabledEvent;

        /// <summary>
        /// Visible event.
        /// </summary>
        event WVEventHandler<bool> Visible;

        /// <summary>
        /// Size Changed event.
        /// </summary>
        event WVEventHandler<int, int> SizeChanged;

        /// <summary>
        /// Raw event for platform specific
        /// </summary>
        event WVSysEventHandler Raw;

        #region PROPS

        /// <summary>
        /// Gets a Rect object.
        /// </summary>
        IRect Rect { get; }

        //-------------------------------------------//

        /// <summary>
        /// Gets or sets windows state.
        /// </summary>
        WindowState State { get; set; }

        /// <summary>
        /// Gets or sets windows state by text.
        /// </summary>
        string StateText { get; set; }

        //-------------------------------------------//

        /// <summary>
        /// Gets or sets a WebView's title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether this WebView appears in the topmost z-order.
        /// <para>
        /// true if the window is topmost; otherwise, false.
        /// </para>
        /// </summary>
        bool TopMost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this WebView is enabled in the user interface (UI)
        /// <para>
        /// true if the WebView is enabled; otherwise, false. The default value is true.
        /// </para>
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets a value that indicates whether this WebView window is visible.
        /// <para>
        /// true if the WebView is visible; otherwise, false. The default value is false.
        /// </para>
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Prevents the window from closing, and fires the OnClose event
        /// </summary>
        bool PreventClose { get; set; }

        /// <summary>
        /// Gets a value that indicates whether this WebView window is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Get or set the snap window function. Default is true.
        /// </summary>
        bool AllowSnap { get; set; }

        /// <summary>
        /// Get or set ClickThrough
        /// </summary>
        bool ClickThrough { get; set; }

        #endregion

        //-------------------------------------------//

        #region METHODS

        /// <summary>
        /// Moves the window to the center of the screen
        /// </summary>
        void ToCenter();

        /// <summary>
        /// Close the window
        /// </summary>
        void Close();

        /// <summary>
        /// Shows the window without activating it
        /// </summary>
        void ShowBehind();

        /// <summary>
        /// Shows the window
        /// </summary>
        void Show();

        /// <summary>
        /// Hide the window
        /// </summary>
        void Hide();

        /// <summary>
        /// Drag the window
        /// </summary>
        void Drag();

        /// <summary>
        /// Resize top-left the window
        /// </summary>
        void ResizeTopLeft();

        /// <summary>
        /// Resize top-right the window
        /// </summary>
        void ResizeTopRight();

        /// <summary>
        /// Resize bottom-left the window
        /// </summary>
        void ResizeBottomLeft();

        /// <summary>
        /// Resize bottom-right the window
        /// </summary>
        void ResizeBottomRight();

        /// <summary>
        /// Resize left the window
        /// </summary>
        void ResizeLeft();

        /// <summary>
        /// Resize right the window
        /// </summary>
        void ResizeRight();

        /// <summary>
        /// Resize top the window
        /// </summary>
        void ResizeTop();

        /// <summary>
        /// Resize bottom the window
        /// </summary>
        void ResizeBottom();

        /// <summary>
        /// Minimize the window
        /// </summary>
        void Minimize();

        /// <summary>
        /// Restore the window
        /// </summary>
        void Restore();

        /// <summary>
        /// Maximize the window
        /// </summary>
        void Maximize();

        /// <summary>
        /// Normalize the window
        /// </summary>
        void Normalize();

        #endregion

        //-------------------------------------------//

        #region EVENTS

        /// <summary>
        /// State changed event JS.
        /// </summary>
        //object? OnStateChanged { get; set; }

        /// <summary>
        /// Close event JS
        /// <para>
        /// fired when the PreventClose property is true.
        /// </para>
        /// </summary>
        //object? OnClosing { get; set; }

        /// <summary>
        /// Position changed event JS.
        /// </summary>
        //object? OnPositionChanged { get; set; }

        /// <summary>
        /// Activated event JS.
        /// </summary>
        //object? OnActivated { get; set; }

        /// <summary>
        /// Enabled event JS.
        /// </summary>
        //public object? OnEnabled { get; set; }

        /// <summary>
        /// Visible event JS.
        /// </summary>
        //object? OnVisible { get; set; }

        /// <summary>
        /// Size Changed event JS.
        /// </summary>
        //public object? OnSizeChanged { get; set; }

        /// <summary>
        /// Appends an event listener for events whose type attribute value is type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        //void AddEventListener(string type, object callback);

        /// <summary>
        /// Removes the event listener in target's event listener list with the same type and callback.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        //void RemoveEventListener(string type, object callback);

        #endregion

    }
}