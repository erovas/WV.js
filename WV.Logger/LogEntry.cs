using WV.Interfaces;

namespace WV.Logger
{
    public sealed class LogEntry
    {
        public DateTime Timestamp { get; }
        public LogLevel Level { get; }
        public string Message { get; }
        public Exception? Exception { get; }
        public string? Source { get; }

        public LogEntry(LogLevel level, string message, Exception? exception = null, string? source = null)
        {
            Timestamp = DateTime.Now;
            Level = level;
            Message = message ?? string.Empty;
            Exception = exception;
            Source = source;
        }

        public override string ToString()
        {
            var ex = Exception != null ? Environment.NewLine + Exception : string.Empty;
            var src = string.IsNullOrEmpty(Source) ? string.Empty : $" [{Source}]";
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level.ToString().ToUpperInvariant()}]{src} {Message}{ex}";
        }
    }

}