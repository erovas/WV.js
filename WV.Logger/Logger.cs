using WV.Interfaces;

namespace WV.Logger
{
    public sealed class Logger : ILogger, IDisposable
    {
        public static string InitMessage { get; set; } = " ####||================ LOG INITIALIZED ================||#### ";

        internal readonly List<LogSink> _sinks = new();
        internal readonly object _sinksLock = new();
        private readonly string _source;
        internal bool _disposed;
        private volatile bool _enabled = true;

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public LogLevel MinimumLevel { get; set; } = LogLevel.Info;
        public string Source { get { return _source; } }

        public Logger(string source)
        {
            _source = source;
        }

        public Logger AddSink(LogSink sink)
        {
            if (sink == null) 
                return this;
            
            lock (_sinksLock)  
                _sinks.Add(sink);

            return this;
        }

        public void RemoveSink(LogSink sink)
        {
            lock (_sinksLock) 
                _sinks.Remove(sink);
        }

        // --- Atajos ---
        public void Trace(string message) => Log(LogLevel.Trace, message);

        public void Debug(string message) => Log(LogLevel.Debug, message);

        public void Info(string message) => Log(LogLevel.Info, message);

        public void Warning(string message) => Log(LogLevel.Warning, message);

        public void Error(string message, Exception? exception = null) => Log(LogLevel.Error, message, exception);

        public void Critical(string message, Exception? exception = null) => Log(LogLevel.Critical, message, exception);

        // --- Núcleo ---
        public void Log(LogLevel level, string message, Exception? exception = null)
        {
            if (_disposed) 
                return;

            if (!_enabled) 
                return;

            if (level < MinimumLevel) 
                return;

            var entry = new LogEntry(level, message, exception, _source);

            // Copiamos la lista para no bloquear durante escritura
            LogSink[] snapshot;
            
            lock (_sinksLock) 
                snapshot = _sinks.ToArray();

            foreach (var sink in snapshot)
                try { sink.Write(entry); }
                catch { /* aislar sinks entre sí */ }
            
        }

        /// <summary>
        /// Crea un logger hijo que añade un prefijo de origen al mensaje.
        /// Útil para plugins o componentes que quieren identificar su origen.
        /// </summary>
        public ILogger ForSource(string source)
        {
            // Un logger "hijo" comparte los sinks del padre pero añade su propio source.
            return new ScopedLogger(this, source);
        }

        public void Dispose()
        {
            if (_disposed) 
                return;

            LogSink[] snapshot;
            lock (_sinksLock)
            {
                snapshot = _sinks.ToArray();
                _sinks.Clear();
                _disposed = true;
            }

            foreach (var sink in snapshot)
            {
                try { sink.Dispose(); }
                catch { /* nada */ }
            }
        }

    }
}