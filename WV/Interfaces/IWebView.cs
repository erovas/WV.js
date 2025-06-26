namespace WV.Interfaces
{
    public interface IWebView : IDisposable
    {

        #region READONLY PROPERTIES

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
        IPrintManager PrintManager { get; }

        /// <summary>
        /// Gets a value that indicates wheter this WebView is the main.
        /// <para>
        /// true if the Webview is the main Window
        /// </para>
        /// </summary>
        bool IsMain { get; }

        /// <summary>
        /// Gets an array with all the names of the loaded plugins
        /// </summary>
        string[] PluginsName { get; }

        #endregion

        //-------------------------------------------//

        #region METHODS

        /// <summary>
        /// Create an instance of a plugin
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        object NewPluginInstance(string pluginName, params object[] args);

        /// <summary>
        /// Retrieves a plugin instance using its UID
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        object GetPluginInstance(string UID);

        /// <summary>
        /// Restart the application
        /// </summary>
        void Restart();

        /// <summary>
        /// Load plugins from folder
        /// </summary>
        /// <param name="foldePath"></param>
        /// <returns></returns>
        string[] LoadPluginsFromFolder(string foldePath = "");

        /// <summary>
        /// Load plugin from path
        /// </summary>
        /// <param name="pluginPath"></param>
        /// <returns></returns>
        string LoadPlugin(string pluginPath);

        /// <summary>
        /// Unload plugin by name
        /// </summary>
        /// <param name="pluginName"></param>
        void UnloadPlugin(string pluginName);

        #endregion

    }
}