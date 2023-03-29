using System.IO;
using System.Reflection;
using System.Text.Json;
using WV.JavaScript.Enums;
using WV.WebView;
using WV.WebView.Entities;
using WV.Windows.HostObject;
using WV.Windows.Utils;

namespace WV.Windows
{
    public class AppLauncher : System.Windows.Application
    {
        public WebView? WVjs { get; private set; }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.3.0")]
        public void InitializeComponent()
        {
            using (AppConfig appConfig = GetInitializedAppConfig())
            {
                // Obtener las rutas del App.config
                AppManager.SrcPath = appConfig.SrcPath;
                AppManager.PluginsPath = appConfig.PluginPath;
                AppManager.UserDataPath = appConfig.UserDataPath;

                Config config = appConfig.Config;
                Parameters parameters = config.Parameters;
                ImportPlugins(config);
                this.WVjs = new WebView(parameters);
            }
        }

        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.3.0")]
        public static int Main(string[] args)
        {
            AppLauncher app = new AppLauncher();
            app.InitializeComponent();
            return app.Run(app.WVjs);
        }

        #region PRIVATE

        private static void ImportPlugins(Config config)
        {
            List<string> pluginsList = config.Plugins.ToList();
            string JScript;

            //==============================================================//
            // JSScript principal que expone la API de WV.js para los Plugins

            JScript = Misc.StringReplace(HOResources.HOScript1, "-",
                                    nameof(AppManager.HostObjectName), AppManager.HostObjectName,
                                    nameof(App.Name), App.Name);

            JScript = Misc.StringReplace(JScript, "'", nameof(JSEvent), JsonSerializer.Serialize(Enum.GetNames(typeof(JSEvent))));

            JScript = "(_=>{ " + JScript + " })();";
            AppManager.JScripts.Add(JScript);

            JScript = Misc.StringReplace(HOResources.HOScript2, "-", nameof(App.Name), App.Name);
            
            JScript = AUX_PackJScriptPlugin(JScript);

            AppManager.JScripts.Add(JScript);

            //==============================================================//

            foreach (string file in Directory.GetFiles(AppManager.PluginsPath, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(file);
                    Type typePlugin = typeof(Plugin);
                    Type typeIPlugin = typeof(IPlugin);

                    foreach (Type type in asm.GetTypes())
                    {
                        // Verificar que el Type Sí es un Plugin
                        if (!typePlugin.IsAssignableFrom(type))
                            continue;

                        // Obtener nombre del Plugin
                        string name = type.Name;

                        //Verificar que el Type si está listado
                        if (!pluginsList.Contains(name))
                            continue;

                        //Se elimina el nombre de la lista para evitar "importar" otro plugin con el mismo nombre
                        pluginsList.Remove(name);

                        //Guardar el Type en el diccionario global
                        AppManager.PluginTypes.Add(name, type);

                        //Verificar si el Plugin implementa IPlugin, para ejecutar script en JS
                        if (!type.GetInterfaces().Contains(typeIPlugin))
                            continue;

                        JScript = AUX_PackJScriptPlugin(type.GetProperty("JScript", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetValue(null) + "");

                        //Agregar script a la cola para ejecutar
                        AppManager.JScripts.Add(JScript);
                    }
                }
                catch (Exception) { }
            }
        }

        private static AppConfig GetInitializedAppConfig()
        {
            AppConfig appConfig = new AppConfig();

            //El App.config indica que es programa final
            if (appConfig.Released)
            {
                // Está released, pero NO está protegido
                if (!appConfig.Protected)
                {
                    try
                    {
                        string configString = File.ReadAllText(App.ConfigFilePath);
                        appConfig.Config = JsonSerializer.Deserialize<Config>(configString) ?? appConfig.Config;
                    }
                    catch (Exception ex)
                    {
                        appConfig.Config = appConfig.Config;
                    }

                    // Proteger el App.config
                    appConfig.Protected = true;

                    try
                    {
                        // Eliminar el config.json
                        File.Delete(App.ConfigFilePath);
                    }
                    catch (Exception) { }

                }
            }
            else
            {
                try
                {
                    string configString = File.ReadAllText(App.ConfigFilePath);
                    appConfig.Config = JsonSerializer.Deserialize<Config>(configString) ?? appConfig.Config;
                }
                catch (Exception ex)
                {
                    appConfig.Config = appConfig.Config;
                }
            }

            return appConfig;
        }

        #endregion

        #region HELPERS

        private static string AUX_PackJScriptPlugin(string pluginScript)
        {
            // si NO existe el objeto 'wv', pues NO ejecutar el script del plugin
            return "(_=>{ " + "if(!window['" + App.Name + "']) return; /**/ " + pluginScript + Environment.NewLine + " /**/ ;})();";
        }

        #endregion

    }
}