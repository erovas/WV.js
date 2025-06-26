using System.Reflection;

namespace WV.Win.Classes
{
    internal sealed class PluginLoader
    {
        #region HELPERS

        private static Type RawPluginType { get; } = typeof(Plugin);

        #endregion

        private PluginLoadContext? Context { get; set; }

        public bool IsLoaded { get; private set; }

        private Type? _Type;
        public Type Type 
        { 
            get {
                if (this._Type == null)
                    throw new Exception("Plugin not loaded");

                return _Type; 
            }
        }

        public string Path { get; }

        public PluginLoader(string pluginPath, string root)
        {
            pluginPath = pluginPath.Replace('\\', '/');

            if(!System.IO.Path.IsPathFullyQualified(pluginPath))
                pluginPath = root + "/" + pluginPath;

            string ext = System.IO.Path.GetExtension(pluginPath);

            if (string.IsNullOrEmpty(ext) || ext.ToUpper() != ".DLL")
                throw new Exception($"The file [{pluginPath}] is not a Dll");

            if (!File.Exists(pluginPath))
                throw new Exception($"File not exists [{pluginPath}]");

            this.Path = pluginPath;
        }

        public Type Load()
        {
            if(this.IsLoaded)
                return this.Type;

            string pluginPath = this.Path;
            FileStream streamDll = new FileStream(pluginPath, FileMode.Open, FileAccess.Read);
            FileStream? streamPdb = null;

            // Si se esta en un entorno de DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
                try { streamPdb = new FileStream(System.IO.Path.ChangeExtension(pluginPath, ".pdb"), FileMode.Open, FileAccess.Read); }
                catch (Exception) { }

            this.Context = new PluginLoadContext(pluginPath);

            try
            {
                Assembly asm = this.Context.LoadFromStream(streamDll, streamPdb);

                List<Type> TypeList = asm.GetTypes().Where(t => RawPluginType.IsAssignableFrom(t) && RawPluginType.Name != t.Name).ToList();

                if (TypeList.Count == 0)
                    throw new Exception("There are no plugins defined in the assembly");

                if (TypeList.Count > 1)
                    throw new Exception("There is more than one plugin defined in the assembly");

                this._Type = TypeList.First();
                this.IsLoaded = true;
                return this.Type;
            }
            catch (Exception)
            {
                // Descargar el Contexto
                this.Context.Unload();
                this.Context = null;
                // Ayuda a liberar recursos
                GC.Collect();
                throw;
            }
            finally
            {
                streamDll.Dispose();
                streamPdb?.Dispose();
            }
        }

        public void Unload()
        {
            if(!this.IsLoaded)
                return;

            this.Context?.Unload();
            this.Context = null;
            this._Type = null;
            this.IsLoaded = false;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
    }
}