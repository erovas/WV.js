using WV.Enums;

namespace WV.Interfaces
{
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