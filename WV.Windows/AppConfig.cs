using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows.Shapes;
using WV.WebView.Entities;

namespace WV.Windows
{
    internal class AppConfig : IDisposable
    {
        private static string GetPath(string? path, string folderName)
        {
            if (Uri.TryCreate(path, UriKind.Absolute, out Uri? uri))
            {
                path = uri.AbsolutePath;
                string[] split = path.Split("/");
                if (string.IsNullOrWhiteSpace(split[split.Length - 1]))
                    path += folderName;
            }
            else
                path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + folderName;
                //path = Directory.GetCurrentDirectory() + "/" + folderName;

            Directory.CreateDirectory(path);
            return path;
        }

        private ConnectionStringsSection? InnerSection { get; set; }
        private Configuration? InnerConfiguration { get; set; }

        public bool Released
        {
            get => GetString("released") == true.ToString();
            set => SetString("released", value.ToString());
        }

        public bool Protected
        {
            get => GetString("protected") == true.ToString() && this.InnerSection != null && this.InnerSection.SectionInformation.IsProtected;
            set
            {
                if (this.InnerSection == null)
                    return;

                if(value && !this.InnerSection.SectionInformation.IsProtected)
                    this.InnerSection.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                else if(!value && this.InnerSection.SectionInformation.IsProtected)
                    this.InnerSection.SectionInformation.UnprotectSection();

                SetString("protected", value.ToString());
            }
        }

        public Config Config
        {
            get
            {
                Config _Config = new();
                try
                {
                    string configString = GetString("config") + "";
                    _Config = JsonSerializer.Deserialize<Config>(configString) ?? _Config;
                }
                catch (Exception) { }

                return _Config;
            }
            set
            {
                Config? _Config = value;
                _Config ??= new();

                SetString("config", JsonSerializer.Serialize(_Config));
            }
        }

        public string PluginPath 
        { 
            get
            {
                string? path = GetPath(GetString("plugins-path"), "plugins");
                return path;
            }
            set
            {
                string? path = GetPath(value, "plugins");
                SetString("plugins-path", path);
            }
        }

        public string SrcPath
        {
            get
            {
                string? path = GetPath(GetString("src-path"), "src");
                return path;
            }
            set
            {
                string? path = GetPath(value, "src");
                SetString("src-path", path);
            }
        }

        public string UserDataPath
        {
            get
            {
                string? path = GetPath(GetString("user-data-path"), "userdata");
                return path;
            }
            set
            {
                string? path = GetPath(value, "userdata");
                SetString("user-data-path", path);
            }
        }


        public AppConfig() {
            this.InnerConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            this.InnerSection = (ConnectionStringsSection)this.InnerConfiguration.GetSection("connectionStrings");
        }

        public void Dispose()
        {
            this.InnerSection = null;
            this.InnerConfiguration = null;
        }

        private string? GetString(string key) 
        {
            return this.InnerSection?.ConnectionStrings[key].ConnectionString;
        }

        private void SetString(string key, string value) 
        {
            if (this.InnerSection == null || this.InnerConfiguration == null)
                return;

            this.InnerSection.ConnectionStrings[key].ConnectionString = value;
            this.InnerConfiguration.Save();
        }
    }
}