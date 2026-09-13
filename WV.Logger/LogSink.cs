using WV.Interfaces;

namespace WV.Logger
{
    public abstract class LogSink : IDisposable
    {
        private bool _initialized = false;

        public abstract void Write(LogEntry entry);

        public void Init()
        {
            if (_initialized)
                return;

            _initialized = true;
            Write(new LogEntry(LogLevel.None, Logger.InitMessage));
        }

        public virtual void Dispose() 
        {
            //Do nothing
        }
    }
}