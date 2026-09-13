namespace WV.Interfaces
{
    public interface IContext
    {
        /// <summary>
        /// WebView que aloja al plugin. Puede ser null tras llamar a <see cref="Release"/>.
        /// </summary>
        IWebView? WebView { get; }

        /// <summary>
        /// Logger hijo con origen del plugin. Es infraestructura compartida:
        /// no se libera ni se suelta en <see cref="Release"/>.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Identidad única del plugin. Es un valor inmutable.
        /// </summary>
        public string UID { get; }

        /// <summary>
        /// Nombre del plugin. Es un valor inmutable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Callback que el webview registró para enterarse de la liberación del plugin.
        /// Puede ser null tras llamar a <see cref="Release"/>.
        /// </summary>
        public Action<string>? OnDisposed { get; }

        /// <summary>
        /// Indica si el contexto ya fue liberado con <see cref="Release"/>.
        /// </summary>
        public bool IsReleased { get; }

        /// <summary>
        /// Suelta las referencias que apuntan al host (Host y OnDisposed) para romper
        /// el ciclo WebView ↔ Plugin ↔ PluginContext.
        ///
        /// No es un Dispose: no hay recursos del sistema que liberar. Solo se sueltan
        /// referencias administradas para que el GC pueda recoger al host cuando ya
        /// no sea usado por nadie más.
        ///
        /// Es idempotente y thread-safe.
        /// </summary>
        void Release();
    }
}