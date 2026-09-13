namespace WV.Logger
{
    public sealed class CallbackSink : LogSink
    {
        private readonly Action<LogEntry> _callback;

        public CallbackSink(Action<LogEntry> callback)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public override void Write(LogEntry entry)
        {
            try { _callback(entry); }
            catch { /* un sink no debe tumbar a los demás */ }
        }

    }
}