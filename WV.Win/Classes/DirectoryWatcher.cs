namespace WV.Win.Classes
{
    internal sealed class DirectoryWatcher : IDisposable
    {
        private FileSystemWatcher? _watcher;
        private Timer? _debounceTimer;
        private object _lock = new();
        private bool _disposed;
        private string _Directory = string.Empty;

        private Action<WatcherEventArgs>? _onWatch;
        private Action<Exception>? _onError;
        private WatcherEventArgs? _watcherArgs;

        #region Properties

        public string Directory
        {
            get => _Directory;
            set
            {
                if (_disposed)
                    return;

                var started = IsStarted;
                Stop();
                _watcher = CreateWatcher(value);
                _Directory = value;
                if (started)
                    Start();
            }
        }

        public bool IsStarted { get; private set; }

        public Action<WatcherEventArgs>? OnWatch
        {
            get { return _onWatch; }
            set {
                if (_disposed)
                    return;
                
                _onWatch = value; 
            }
        }

        public Action<Exception>? OnError
        {
            get { return _onError; }
            set
            {
                if (_disposed)
                    return;

                _onError = value;
            }
        }

        #endregion

        public DirectoryWatcher(string directory)
        {
            Directory = directory;
        }

        public DirectoryWatcher() { }

        public void Start()
        {
            if(_disposed && IsStarted)
                return;

            _debounceTimer = new Timer(DebouncedEvent, null, Timeout.Infinite, Timeout.Infinite);
            _watcher ??= CreateWatcher(_Directory);

            _watcher.Changed += HotReload_Refresh;
            _watcher.Deleted += HotReload_Refresh;
            _watcher.Renamed += HotReload_Refresh;
            _watcher.Error += HotReload_Error;

            IsStarted = true;
        }

        public void Stop()
        {
            if (_disposed)
                return;

            if (_watcher == null)
                return;

            _debounceTimer?.Dispose();
            _debounceTimer = null;

            _watcher.Changed -= HotReload_Refresh;
            _watcher.Deleted -= HotReload_Refresh;
            _watcher.Renamed -= HotReload_Refresh;
            _watcher.Error -= HotReload_Error;
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            _watcher = null;

            IsStarted = false;
        }

        #region Helpers

        private static FileSystemWatcher CreateWatcher(string directory)
        {
            return new FileSystemWatcher(directory)
            {

                NotifyFilter = NotifyFilters.DirectoryName |
                                            NotifyFilters.FileName |
                                            NotifyFilters.LastWrite,
                //Filter = "*.*",
                Filter = "",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };
        }

        #endregion

        private void DebouncedEvent(object? state)
        {
            if(_watcherArgs != null)
                _onWatch?.Invoke(_watcherArgs);
        }

        private void HotReload_Error(object sender, ErrorEventArgs e)
        {
            _onError?.Invoke(e.GetException());
        }

        private void HotReload_Refresh(object sender, FileSystemEventArgs e)
        {
            _watcherArgs = new WatcherEventArgs(e);
            
            lock (_lock)
            {
                // Cancelar timer existente
                _debounceTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                // Disparar con 100 ms de delay
                _debounceTimer?.Change(100, Timeout.Infinite);
            }
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DirectoryWatcher()
        { 
            Dispose(false); 
        }

        private void Dispose(bool disposing) 
        {
            if(disposing)
                Stop();

            _onWatch = null;
            _onError = null;
            _watcherArgs = null;
            _disposed = true;
        }

        #endregion

    }

    #region Attributes

    [Flags]
    internal enum WatcherType
    {
        /// <summary>
        /// The creation of a file or folder.
        /// </summary>
        Created = 1,

        /// <summary>
        /// The deletion of a file or folder.
        /// </summary>
        Deleted = 2,

        /// <summary>
        /// The change of a file or folder.
        /// </summary>
        Changed = 4,

        /// <summary>
        /// The renaming of a file or folder.
        /// </summary>
        Renamed = 8,

        /// <summary>
        /// all of the above
        /// </summary>
        All = Created | Deleted | Changed | Renamed
    }

    internal sealed class WatcherEventArgs
    {
        /// <summary>
        /// Gets one of the <see cref='WatcherType'/> values.
        /// </summary>
        public WatcherType ChangeType
        {
            get;
        }

        /// <summary>
        /// Gets the fully qualified path of the affected file or directory.
        /// </summary>
        public string FullPath
        {
            get;
        }


        /// <summary>
        /// Gets the name of the affected file or directory.
        /// </summary>
        public string? Name
        {
            get;
        }

        /// <summary>
        /// Gets the previous fully qualified path of the affected file or directory.
        /// </summary>
        public string OldFullPath
        {
            get;
        }

        /// <summary>
        /// Gets the old name of the affected file or directory.
        /// </summary>
        public string? OldName
        {
            get;
        }

        public WatcherEventArgs(FileSystemEventArgs e)
        {
            ChangeType = (WatcherType)e.ChangeType; 
            FullPath = e.FullPath; 
            Name = e.Name;
            OldFullPath = string.Empty;

            if (ChangeType.HasFlag(WatcherType.Renamed))
            {
                var ex = (RenamedEventArgs)e;
                OldFullPath = ex.FullPath;
                OldName = ex.OldName;
            }
        }
    }

    #endregion
}