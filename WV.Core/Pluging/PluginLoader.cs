using System.Reflection;
using WV.Attributes;
using WV.Interfaces;

namespace WV.Core.Pluging
{
    internal sealed class PluginLoader
    {
        private PluginLoadContext? Context { get; set; }

        public IPluginMetadata Metadata { get; private set; }

        public Type Type { get; private set; }
        

        public PluginLoader(string pluginPath)
        {
            (Metadata, Context, Type) = Load(pluginPath);
        }

        private static (IPluginMetadata md, PluginLoadContext ctx, Type tp) Load(string pluginPath)
        {
            FileStream streamDll = new FileStream(pluginPath, FileMode.Open, FileAccess.Read);
            FileStream? streamPdb = null;

            // Si se esta en un entorno de DEBUG, intentar cargar el PDB.
            if (App.IsDebugging)
                try { streamPdb = new FileStream(Path.ChangeExtension(pluginPath, ".pdb"), FileMode.Open, FileAccess.Read); }
                catch (Exception) { }

            var ctx = new PluginLoadContext(pluginPath);

            try
            {
                var asm = ctx.LoadFromStream(streamDll, streamPdb);
                var metadata = FromAssembly(asm, pluginPath);
                var type = ResolveType(asm, metadata);
                return (metadata, ctx, type);
            }
            catch (Exception)
            {
                // Descargar el Contexto
                ctx.Unload();
                ctx = null;
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
#pragma warning disable CS8625 
            this.Type = null;
#pragma warning restore CS8625 
            this.Context?.Unload();
            this.Context = null;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }


        public static PluginMetadata FromAssembly(Assembly assembly, string pluginPath)
        {
            var attr = assembly.GetCustomAttribute<PluginAttribute>() ?? throw new Exception("The DLL is not a plugin.");

            return new PluginMetadata
            {
                Path = pluginPath,
                Name = attr.Name,
                Type = attr.Type,
                Version = attr.Version,
                Singleton = attr.Singleton,
                Author = attr.Author,
                Description = attr.Description
            };
        }

        private static Type ResolveType(Assembly assembly, PluginMetadata metadata)
        {
            var type = assembly.GetType(metadata.Type, throwOnError: false);
            if (type != null && typeof(Plugin).IsAssignableFrom(type) && !type.IsAbstract)
                return type;

            throw new Exception("There are no plugin defined in the assembly");
        }

    }
}