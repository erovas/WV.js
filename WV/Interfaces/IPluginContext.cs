namespace WV.Interfaces
{
    public interface IPluginContext
    {
        public static IPluginContext Create(IWebView? webview, ILogger logger, string uid, string name, Action<string>? onDisposed = null)
        {
            return new PluginContext(webview, logger, uid, name, onDisposed);
        }

        /// <summary>
        /// WebView que aloja al plugin. Puede ser null tras llamar a <see cref="Release"/>.
        /// </summary>
        IWebView? WebView { get; }

        /// <summary>
        /// Logger hijo con origen del plugin. Es infraestructura compartida:
        /// no se libera ni se suelta en <see cref="Release"/>.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Identidad única del plugin. Es un valor inmutable.
        /// </summary>
        string UID { get; }

        /// <summary>
        /// Nombre del plugin. Es un valor inmutable.
        /// </summary>
        string Name { get; }
    }
}