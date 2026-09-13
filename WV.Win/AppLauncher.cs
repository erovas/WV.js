using WV.Logger;
using WV.Win.Imp;
using WV.Interfaces;
using WV.Win.Scripts;

namespace WV.Win
{
    internal class AppLauncher
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Cargar JScript principal
            Helpers.JScripts.Add(PackJSScript(ScriptResources.MainScript.Replace("-HostObjectName-", Helpers.HostObjectName)));
            
            string? uri = null;
            string? lang = null;
            IContext ctx = CreateDefaultContext();

            if (args.Length > 0)
                uri = args[0];

            if (args.Length > 1)
                lang = args[1];

            if (string.IsNullOrWhiteSpace(uri) || uri.ToLower() == "null")
                uri = null;
            
            // Se inicia una Instancia de WebView (Inicia el programa)
            _ = new WebView(ctx, uri, lang);

        }

        private static string PackJSScript(string? script)
        {
            return "(_=>{ /**/ " + script + " /**/ })();";
        }

        private static ILogger CreateDefaultLogger()
        {
            // "C:\\Users\\....\\AppData\\Local\\WV.js\\logs"
            //var logDir = Path.Combine(
            //    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            //    "WV.js", "logs");

            var logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");

            var logger = new Logger.Logger("Main-WebView")
                .AddSink(new ConsoleSink())
                .AddSink(new FileSink(Path.Combine(logDir, "wv.log")));

            logger.MinimumLevel = LogLevel.Debug;
            return logger;
        }

        private static IContext CreateDefaultContext()
        {
            ILogger logger = CreateDefaultLogger();
            string name = typeof(WebView).Name;
            return Helpers.CreateContext(null, logger, name);
        }
    }
}