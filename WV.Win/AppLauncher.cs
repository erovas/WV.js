using WV.Win.Imp;
using WV.Interfaces;
using WV.Win.Scripts;
using WV.Core.Logging;
using WV.Core.Logging.Sinks;
using WV.Enums;
using WV.Configs;
using WV.Win.Win32;

namespace WV.Win
{
    internal class AppLauncher
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Cargar JScript principal
            Utils.JScripts.Add(PackJScript(ScriptResources.MainScript.Replace("-HostObjectName-", Utils.HostObjectName)));

            try
            {
                WebViewConfig config = new();

                if (args.Length > 0)
                    config = Core.Utils.GetWebViewConfig(args[0]) ?? config;
                else
                {
                    var path = Path.Combine(App.Directory, "WV.json");

                    if (File.Exists(path))
                        config = Core.Utils.GetWebViewConfig(File.ReadAllText(path)) ?? config;
                }

                IPluginContext ctx = CreateDefaultContext(config);

                // Se inicia una Instancia de WebView (Inicia el programa)
                _ = new WebView(ctx, config);
            }
            catch (Exception ex)
            {
                Utils32.MsgBoxError(IntPtr.Zero, "Error", ex.InnerException ?? ex);
                throw;
            }
        }

        private static string PackJScript(string? script)
        {
            return "(_=>{ /**/ " + script + " /**/ })();";
        }

        private static ILogger CreateDefaultLogger(LoggerConfig config)
        {
            var logDir = Core.Utils.GetFullDirectory(App.Directory, config.Directory, false);

            var logger = new Logger(config.Source)
                .AddSink(new ConsoleSink())
                .AddSink(new FileSink(Path.Combine(logDir, "wv.log")));

            logger.Enabled = config.Enabled;
            logger.MinimumLevel = LogLevel.Debug;
            return logger;
        }

        private static IPluginContext CreateDefaultContext(WebViewConfig config)
        {
            ILogger logger = CreateDefaultLogger(config.Logger);
            string name = typeof(WebView).Name;
            return Utils.CreateContext(null, logger, name);
        }
    }
}