using WV.Interfaces;

namespace WV.Logger
{
    public sealed class ScopedLogger : ILogger
    {
        private readonly Logger _parent;
        private readonly string _source;
        private volatile bool _enabled;

        public ScopedLogger(Logger parent, string source)
        {
            _parent = parent;
            _source = source;
            _enabled = _parent.Enabled;
        }

        public LogLevel MinimumLevel
        {
            get => _parent.MinimumLevel;
            set => _parent.MinimumLevel = value;
        }

        public string Source { get { return _source; } }

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public void Trace(string message) => Log(LogLevel.Trace, message);
        public void Debug(string message) => Log(LogLevel.Debug, message);
        public void Info(string message) => Log(LogLevel.Info, message);
        public void Warning(string message) => Log(LogLevel.Warning, message);
        public void Error(string message, Exception? exception = null) => Log(LogLevel.Error, message, exception);
        public void Critical(string message, Exception? exception = null) => Log(LogLevel.Critical, message, exception);

        public void Log(LogLevel level, string message, Exception? exception = null)
        {
            if (_parent._disposed)
                return;

            if (!_enabled)
                return;

            if (level < _parent.MinimumLevel) 
                return;
            
            var entry = new LogEntry(level, message, exception, _source);

            LogSink[] snapshot;

            lock (_parent._sinksLock)
                snapshot = _parent._sinks.ToArray();

            foreach (var sink in snapshot)
                try { sink.Write(entry); }
                catch { }
        }

        public ILogger ForSource(string source)
        {
            return _parent.ForSource(source);
        }
    }
}