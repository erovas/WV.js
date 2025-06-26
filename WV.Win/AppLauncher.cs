
using WV.Win.Imp;
using WV.Win.Scripts;

namespace WV.Win
{
    internal class AppLauncher
    {
        [STAThread]     //!!!Obligatorio ponerlo, para que funcione WebView2
        static void Main(string[] args)
        {
            // Cargar JScript principal
            Helpers.JScripts.Add(PackJSScript(ScriptResources.MainScript.Replace("-HostObjectName-", Helpers.HostObjectName)));

            string? uri = null;
            string? lang = null;

            if (args.Length > 0)
                uri = args[0];

            if (args.Length > 1)
                lang = args[1];

            if (string.IsNullOrWhiteSpace(uri) || uri.ToLower() == "null")
                uri = null;

            // Se inicia una Instancia de WebView (Inicia el programa)
            new WebView(null, uri, lang);
        }

        private static string PackJSScript(string? script)
        {
            return "(_=>{ /**/ " + script + " /**/ })();";
        }

    }
}