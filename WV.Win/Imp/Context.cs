using WV.Interfaces;

namespace WV.Win.Imp
{
    /// <summary>
    /// Contexto de ejecución de un plugin.
    ///
    /// Agrupa el WebView que lo aloja, el logger con su origen, la identidad del plugin
    /// y el callback con el que notifica su liberación.
    ///
    /// No implementa IDisposable porque no posee recursos del sistema: solo
    /// referencias a objetos que pertenecen a otros (host, logger, delegate del host).
    /// Para romper el grafo de referencias cuando el plugin se libera, usa <see cref="Release"/>.
    /// </summary>
    public sealed class Context : IContext
    {
        internal IWebView? _webview;
        private Action<string>? _onDisposed;
        private int _released;

        /// <summary>
        /// WebView que aloja al plugin. Puede ser null tras llamar a <see cref="Release"/>.
        /// </summary>
        public IWebView? WebView => Volatile.Read(ref _webview);

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
        public Action<string>? OnDisposed => Volatile.Read(ref _onDisposed);

        /// <summary>
        /// Indica si el contexto ya fue liberado con <see cref="Release"/>.
        /// </summary>
        public bool IsReleased => Volatile.Read(ref _released) == 1;

        public Context(IWebView? webview, ILogger logger, string uid, string name, Action<string>? onDisposed = null)
        {
            _webview = webview;
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            UID = uid;
            Name = name;
            _onDisposed = onDisposed;
        }

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
        public void Release()
        {
            // Garantiza una sola ejecución aunque varios hilos lo llamen.
            if (Interlocked.Exchange(ref _released, 1) == 1)
                return;

            // Suelta las dos vías hacia el host:
            //  1. La referencia directa (Host).
            //  2. La referencia a través del target del delegate (OnDisposed).
            Volatile.Write(ref _webview, null);
            Volatile.Write(ref _onDisposed, null);
        }
    }
}