namespace WV.Interfaces
{
    public interface IWebView : IDisposable
    {

        #region Properties

        /// <summary>
        /// Gets a value that identify this WebView
        /// </summary>
        string UID { get; }

        /// <summary>
        /// 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        IWindow Window { get; }

        /// <summary>
        /// 
        /// </summary>
        IBrowser Browser { get; }

        /// <summary>
        /// 
        /// </summary>
        IPlugins Plugins { get; }

        /// <summary>
        /// 
        /// </summary>
        IPrintManager PrintManager { get; }

        /// <summary>
        /// Gets a value that indicates wheter this WebView is the main.
        /// <para>
        /// true if the Webview is the main Window
        /// </para>
        /// </summary>
        bool IsMain { get; }

        /// <summary>
        /// 
        /// </summary>
        bool Disposed { get; }

        #endregion

        //-------------------------------------------//

        #region Methods

        /// <summary>
        /// Restart the application
        /// </summary>
        void Restart();

        #endregion

    }
}