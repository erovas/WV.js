using System.Reflection;
using System.Runtime.Loader;

namespace WV.Win.Classes
{
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver Resolver { get; }
        private List<string> TempFiles { get; }

        public PluginLoadContext(string pluginPath) : base(isCollectible: true)
        {
            this.Resolver = new AssemblyDependencyResolver(pluginPath);
            this.TempFiles = new List<string>();

            // Suscribirse al evento de descarga
            this.Unloading += ctx => { 
                this.DeleteTempFiles(); 
            };
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            // Primero intenta cargar desde el contexto predeterminado (para WV.dll)
            if (assemblyName.Name == "WV")
                return null; // Lo carga el contexto padre

            string? assemblyPath = Resolver.ResolveAssemblyToPath(assemblyName);

            if (assemblyPath != null)
                using (var stream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read))
                    return this.LoadFromStream(stream);
                
            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string? libraryPath = Resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if(libraryPath == null)
                return IntPtr.Zero;

            string tempPath = this.CreateTempCopy(libraryPath);
            return this.LoadUnmanagedDllFromPath(tempPath);
        }

        private string CreateTempCopy(string originalPath)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "WVUnmanagedDllDependencies");
            Directory.CreateDirectory(tempDir);

            string tempFileName = $"{Guid.NewGuid()}_{Path.GetFileName(originalPath)}";
            string tempPath = Path.Combine(tempDir, tempFileName);

            File.Copy(originalPath, tempPath, overwrite: true);
            TempFiles.Add(tempPath); // Registrar para eliminar después

            return tempPath;
        }

        private void DeleteTempFiles()
        {
            foreach (string file in TempFiles)
                try { File.Delete(file); }
                catch { /* Ignorar errores si el archivo está en uso */ }
            
            TempFiles.Clear();
        }
    }
}