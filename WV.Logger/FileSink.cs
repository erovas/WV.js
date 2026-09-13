using System.Text;

namespace WV.Logger
{
    public sealed class FileSink : LogSink
    {
        private readonly string _path;
        private readonly long _maxBytes;
        private readonly object _lock = new();
        private bool _disposed;

        public FileSink(string path, long maxBytes = 5 * 1024 * 1024) // 5 MB por defecto
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
            _maxBytes = maxBytes;

            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
        }

        public override void Write(LogEntry entry)
        {
            lock (_lock)
            {
                if (_disposed) 
                    return;

                try
                {
                    RotateIfNeeded();
                    Init();
                    File.AppendAllText(_path, entry.ToString() + Environment.NewLine, Encoding.UTF8);
                }
                catch
                {
                    // Un logger nunca debe tumbar la app por un error al escribir.
                    // Como fallback, escribimos a Debug.
                    System.Diagnostics.Debug.WriteLine($"[FileSink error] {entry}");
                }
            }
        }

        private void RotateIfNeeded()
        {
            if (!File.Exists(_path)) 
                return;

            var info = new FileInfo(_path);

            if (info.Length < _maxBytes) 
                return;

            var backup = _path + ".old";
            
            if (File.Exists(backup)) 
                File.Delete(backup);

            File.Move(_path, backup);
        }

        public override void Dispose()
        {
            lock (_lock) 
                _disposed = true;
        }
    }
}