using WV.Interfaces;

namespace WV.Logger
{
    public sealed class ConsoleSink : LogSink
    {
        private readonly object _lock = new();

        public override void Write(LogEntry entry)
        {
            lock (_lock)
            {
                Init();
                var original = Console.ForegroundColor;
                Console.ForegroundColor = GetColor(entry.Level);
                Console.WriteLine(entry.ToString());
                Console.ForegroundColor = original;
            }
        }

        private static ConsoleColor GetColor(LogLevel level) => level switch
        {
            LogLevel.Trace => ConsoleColor.DarkGray,
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.Magenta,
            _ => ConsoleColor.White
        };

    }
}