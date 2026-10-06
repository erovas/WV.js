using WV.Enums;
using WV.Interfaces;

namespace WV.Core.Logging
{
    public abstract class LogSink : IDisposable
    {
        private bool _initialized = false;

        protected abstract void WriteCore(LogEntry entry);

        public void Write(LogEntry entry)
        {
            EnsureInitialized();
            WriteCore(entry);
        }

        private void EnsureInitialized()
        {
            if (_initialized) 
                return;

            _initialized = true;
            WriteCore(new LogEntry(LogLevel.None, Logger.InitMessage));
        }

        public virtual void Dispose()
        {
            //Do nothing
        }
    }
}