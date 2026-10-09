using System.Collections.Concurrent;
using WV.Configs;
using WV.Interfaces;

namespace WV.Core.Pluging
{
    public abstract class PluginsCore : Plugin, IPlugins
    {
        private readonly Dictionary<string, object> _Instances = new();
        private readonly ConcurrentDictionary<Type, object> _SingletonInstances = new();
        private readonly Dictionary<string, PluginLoader> _Imported = new();
        private readonly List<IPluginMetadata> _LoadedItems = new();
        private readonly IPluginMetadataList _Loaded;

        #region Properties

        public IPluginMetadataList Loaded 
        { 
            get
            {
                var txt = nameof(Loaded);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return _Loaded;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        public string Directory { get; }

        #endregion

        public PluginsCore(IPluginContext context, PluginsConfig pluginsConfig) : base(context)
        {
            var txt = $"{nameof(PluginsCore)} constructor";
            LogCalling(txt);
            try
            {
                _Loaded = new PluginMetadataList(_LoadedItems);
                Directory = Utils.GetFullDirectory(App.Directory, pluginsConfig.Directory);
                Initialize(context, pluginsConfig);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected virtual void Initialize(IPluginContext context, PluginsConfig pluginsConfig)
        {
            foreach (var item in pluginsConfig.Load)
                LoadAux(item);
        }

        #region Methods

        public object GetInstance(string UID)
        {
            var txt = $"{nameof(GetInstance)}(\"{UID}\")";
            LogCalling(txt);
            try
            {
                ThrowIfDisposed();
                return GetInstanceCore(UID);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected virtual object GetInstanceCore(string UID)
        {
            return _Instances[UID];
        }

        public IPluginLoadResult Load(string pluginPath)
        {
            var txt = $"{nameof(Load)}(\"{pluginPath}\")";
            LogCalling(txt);
            try
            {
                ThrowIfDisposed();
                return LoadCore(pluginPath);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected virtual IPluginLoadResult LoadCore(string pluginPath)
        {
            return LoadAux(pluginPath);
        }

        public IPluginLoadResultList LoadFrom(string? folderPath = null)
        {
            var txt = $"{nameof(LoadFrom)}(\"{folderPath}\")";
            LogCalling(txt);
            try
            {
                ThrowIfDisposed();
                return LoadFromCore(folderPath);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected virtual IPluginLoadResultList LoadFromCore(string? folderPath = null)
        {
            folderPath = GetFullDirectory(folderPath);

            List<IPluginLoadResult> results = new();

            foreach (string pluginPath in System.IO.Directory.GetFiles(folderPath, "*.dll", SearchOption.AllDirectories))
                results.Add(LoadAux(pluginPath));

            return new PluginLoadResultList(results);
        }

        public object NewInstance(string pluginName, params object[] args)
        {
            var txt = $"{nameof(NewInstance)}(\"{pluginName}\", ...)";
            LogCalling(txt);
            try
            {
                ThrowIfDisposed();
                return NewInstanceCore(pluginName, args);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw (ex.InnerException ?? ex);
            }
        }

        protected virtual object NewInstanceCore(string pluginName, object[] args)
        {
            PluginLoader? pluginLoader = null;

            if (pluginName != this.WebView.Name && !this._Imported.TryGetValue(pluginName, out pluginLoader))
                throw new Exception($"Plugin [{pluginName}] no exists");

            Type pluginType = pluginLoader != null ? pluginLoader.Type : Utils.WebViewType;
            IPluginContext ctx = Utils.CreateContext(WebView, Logger, pluginName, Logger.Source, PluginDisposedEvent);

            List<object> Args = args.ToList();
            Args.Insert(0, ctx); // Constructor(Context, arg1, arg2, ...)

            // Verifica si el plugin está marcado como singleton.
            if (pluginLoader != null && pluginLoader.Metadata.Singleton)
                return this._SingletonInstances.GetOrAdd(pluginType, type => CreateInstance(type, pluginName, Args.ToArray()));

            return CreateInstance(pluginType, pluginName, Args.ToArray());
        }

        public void Unload(string pluginName)
        {
            var txt = $"{nameof(Unload)}(\"{pluginName}\")";
            LogCalling(txt);
            try
            {
                ThrowIfDisposed();
                UnloadCore(pluginName);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected virtual void UnloadCore(string pluginName)
        {
            if (!this._Imported.ContainsKey(pluginName))
                throw new Exception($"[{pluginName}] Plugin not found.");

            var ctx = this._Imported[pluginName];

            // Obtener todas las instancias del plugin <uid, object>
            var instances = this._Instances.Where(i => ctx.Type == i.Value.GetType()).ToList();

            // Verificar si existe alguna instancia sin hacer Dispose()
            foreach (var instance in instances)
                if (instance.Value is Plugin plugin && !plugin.Disposed)
                    throw new Exception($"The [{pluginName}] plugin cannot be unloaded while instances remain undisposed. {Environment.NewLine} Instance UID = {plugin.UID}");

            foreach (var instance in instances)
                this._Instances.Remove(instance.Key);

            // Puede que sea un plugin Singleton
            this._SingletonInstances.TryRemove(ctx.Type, out object? _);

            ctx.Unload();
            this._Imported.Remove(pluginName);
            var item = _LoadedItems.FirstOrDefault(x => x.Name == pluginName);
            if (item != null)
                _LoadedItems.Remove(item);
        }

        private void PluginDisposedEvent(string UID)
        {
            var txt = $"{nameof(PluginDisposedEvent)}(\"{UID}\")";
            LogCalling(txt);
            try
            {
                if (!this._Instances.TryGetValue(UID, out object? instance))
                    return;

                this._Instances.Remove(UID);
                this._SingletonInstances.TryRemove(instance.GetType(), out object? _);
            }
            catch (Exception ex)
            {
                LogFail(txt , ex);
                throw;
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            _SingletonInstances.Clear();

            foreach (Plugin item in _Instances.Values)
                try
                {
                    item.Dispose();
                }
                catch (Exception) { }

            _Instances.Clear();            

            foreach (var ctx in _Imported.Values)
                ctx.Unload();

            _Imported.Clear();
            _LoadedItems.Clear();
        }

        #region Helpers

        private IPluginLoadResult LoadAux(string pluginPath)
        {
            try
            {
                pluginPath = GetFullDirectory(pluginPath);

                foreach (var md in this._Imported.Values.ToList().Select(x => x.Metadata))
                    if (md.Path == pluginPath)
                        return PluginLoadResult.Ok(md);
                
                var pluginLoader = new PluginLoader(pluginPath);
                var metadata = pluginLoader.Metadata;
                var name = metadata.Name;
                var path = metadata.Path;

                if (this._Imported.ContainsKey(name))
                {
                    pluginLoader.Unload();
                    return PluginLoadResult.Fail(metadata, $"A plugin named [{name}] has already been loaded from the path [{this._Imported[name].Metadata.Path}].");
                }

                this._Imported.Add(name, pluginLoader);
                this._LoadedItems.Add(metadata);
                return PluginLoadResult.Ok(metadata);
            }
            catch (Exception ex)
            {
                return PluginLoadResult.Fail(pluginPath, ex.Message);
            }
        }

        private object CreateInstance(Type type, string pluginName, object[] args)
        {
            object? pluginInstance = Activator.CreateInstance(type, args);

            if (pluginInstance == null)
                throw new Exception($"Impossible to build plugin instance [{pluginName}]");

            this._Instances.Add(((Plugin)pluginInstance).UID, pluginInstance);

            return pluginInstance;
        }

        private string GetFullDirectory(string? path)
        {
            return Utils.GetFullDirectory(Directory, path);
        }

        private void LogCalling(string txt, bool isProp = false)
        {
            Logger.Info(Utils.CallingMsg(txt, isProp));
        }

        private void LogFail(string txt, Exception ex, bool isProp = false) 
        {
            Logger.Error(Utils.FailMsg(txt, isProp), ex);
        }

        #endregion

    }
}