namespace WV.Interfaces
{
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
        None = 6
    }

    public interface ILogger
    {
        LogLevel MinimumLevel { get; set; }
        string Source { get; }
        bool Enabled { get; set; }

        void Trace(string message);
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message, Exception? exception = null);
        void Critical(string message, Exception? exception = null);

        // Para mensajes con contexto adicional (ej: nombre del plugin, del WebView, etc.)
        void Log(LogLevel level, string message, Exception? exception = null);

        ILogger ForSource(string source);
    }
}