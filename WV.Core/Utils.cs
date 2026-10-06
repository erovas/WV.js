using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using WV.Configs;
using WV.Interfaces;

namespace WV.Core
{
    public static class Utils
    {
        internal static string CallingMsg(string txt, bool isProp = false)
        {
            return $"## Calling '{txt}' {(isProp? "property" : "method")}.";
        }

        internal static string FailMsg(string txt, bool isProp = false)
        {
            return $"@@ Fail calling '{txt}' {(isProp ? "property" : "method")}.";
        }

        private static readonly JsonSerializerOptions JOptions = new()
        {
            PropertyNamingPolicy = null,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(allowIntegerValues: true) }
        };

        private static Type? _WebViewType;

        internal static Type WebViewType
        {
            get 
            {
                if(_WebViewType != null)
                    return _WebViewType;

                Type itype = typeof(IWebView);
                List<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(s => s.GetTypes())
                                    .Where(p => itype.IsAssignableFrom(p) && p != itype && !p.IsAbstract).ToList();

                _WebViewType = types.FirstOrDefault();

                if (_WebViewType == null)
                    throw new Exception("IWebView is not implemented");

                return _WebViewType;
            }
        }

        public static WebViewConfig? GetWebViewConfig(string json)
        {
            return JsonSerializer.Deserialize<WebViewConfig>(json, JOptions);
        }

        internal static ProcessStartInfo BuildRestartStartInfo()
        {
            // 1. Determinar ejecutable real.
            var executablePath = GetRealExecutablePath() ?? throw new InvalidOperationException("Cannot determine the executable path to restart.");

            // 2. Argumentos originales (sin el [0] que es la ruta).
            var originalArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();

            // 3. Construir ProcessStartInfo con ArgumentList (escape correcto por SO).
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = Environment.CurrentDirectory
            };

            if (executablePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                // Corriendo vía `dotnet app.dll`
                startInfo.FileName = "dotnet";
                startInfo.ArgumentList.Add(executablePath);
            }
            else
            {
                // Self-contained / single-file
                startInfo.FileName = executablePath;
            }

            foreach (var arg in originalArgs)
                startInfo.ArgumentList.Add(arg);

            return startInfo;
        }

        private static string? GetRealExecutablePath()
        {
            var mainModule = Environment.ProcessPath; // ?? Process.GetCurrentProcess().MainModule?.FileName;

            if (!string.IsNullOrEmpty(mainModule))
            {
                var fileName = Path.GetFileNameWithoutExtension(mainModule);
                var isDotnetHost = string.Equals(fileName, "dotnet", StringComparison.OrdinalIgnoreCase);

                if (!isDotnetHost && !mainModule.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    return mainModule;
            }

            // Fallback: el primer argumento suele ser el .dll en apps `dotnet app.dll`.
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 0
                && !string.IsNullOrWhiteSpace(args[0])
                && args[0].EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                return Path.GetFullPath(args[0]);
            }

            return null;
        }

        private static string GetUID()
        {
            return Guid.NewGuid().ToString();
        }

        public static IPluginContext CreateContext(IWebView? wv, ILogger log, string name, string? source = null, Action<string>? onDisposed = null)
        {
            ILogger logger = log;

            if (wv != null)
                logger = string.IsNullOrEmpty(source) ? log.ForSource(name) : log.ForSource($"{source}:{name}");

            string uid = GetUID();
            return IPluginContext.Create(wv, logger, uid, name, onDisposed);
        }

        public static string GetFullDirectory(string root, string? path)
        {
            if(string.IsNullOrWhiteSpace(path))
                path = root;

            if (!Path.IsPathFullyQualified(path))
                path = Path.Combine(root, path);

            if (!string.IsNullOrWhiteSpace(Path.GetExtension(path)))
                if(!File.Exists(path))
                    throw new FileNotFoundException($"The file [{path}] does not exist");
                else
                    return path;

            else if (!Directory.Exists(path))
                throw new Exception($"The directory [{path}] does not exist");

            return path;
        }

    }
}