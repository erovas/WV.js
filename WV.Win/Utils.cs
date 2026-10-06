using WV.Enums;
using WV.Win.Imp;
using WV.Win.Win32;
using WV.Win.Win32.Enums;
using System.Diagnostics;
using WV.Win.Win32.Structs;
using System.Globalization;
using Microsoft.Web.WebView2.Core;
using System.Runtime.InteropServices;
using WV.Interfaces;
using WV.Core.Pluging;

namespace WV.Win
{
    internal static class Utils
    {

        #region PROPS

        public static readonly string UserDataFolder = Path.Combine(App.Directory, "userdata");

        /// <summary>
        /// ["es-ES", CoreWebView2Environment]
        /// </summary>
        public static Dictionary<string, CoreWebView2Environment> LangEnvironments { get; } = new();

        /// <summary>
        /// 
        /// </summary>
        public static string HostObjectName { get; }

        /// <summary>
        /// 
        /// </summary>
        public static List<string> JScripts { get; } = new();


        /// <summary>
        /// Instancias de la ventana principal contenedora
        /// </summary>
        public static Dictionary<long, WebView> WinInstances { get; } = new();


        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<string, string> MimeTypes { get; }


        public static string URL { get; }


        #endregion

        static Utils() 
        {
            URL = "https://" + App.Window.Title + "/";
            HostObjectName = "-_-" + Guid.NewGuid().ToString() + "-_-";

            MimeTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                // Texto y Documentos
                { "txt", "text/plain" },
                { "html", "text/html" },
                { "htm", "text/html" },
                { "css", "text/css" },
                { "js", "text/javascript" },
                { "json", "application/json" },
                { "xml", "application/xml" },
                { "pdf", "application/pdf" },
                { "csv", "text/csv" },
        
                // Imágenes
                { "png", "image/png" },
                { "jpg", "image/jpeg" },
                { "jpeg", "image/jpeg" },
                { "gif", "image/gif" },
                { "bmp", "image/bmp" },
                { "webp", "image/webp" },
                { "svg", "image/svg+xml" },
                { "ico", "image/x-icon" },
                { "tiff", "image/tiff" },
        
                // Audio
                { "mp3", "audio/mpeg" },
                { "wav", "audio/wav" },
                { "ogg", "audio/ogg" },
                { "aac", "audio/aac" },
                { "flac", "audio/flac" },
                { "m4a", "audio/mp4" },
                { "weba", "audio/webm" },
        
                // Video
                { "mp4", "video/mp4" },
                { "webm", "video/webm" },
                { "avi", "video/x-msvideo" },
                { "mov", "video/quicktime" },
                { "mkv", "video/x-matroska" },
                { "mpeg", "video/mpeg" },
                { "3gp", "video/3gpp" },
        
                // Archivos comprimidos
                { "zip", "application/zip" },
                { "rar", "application/x-rar-compressed" },
                { "7z", "application/x-7z-compressed" },
                { "tar", "application/x-tar" },
                { "gz", "application/gzip" },
        
                // Ofimática
                { "doc", "application/msword" },
                { "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { "xls", "application/vnd.ms-excel" },
                { "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { "ppt", "application/vnd.ms-powerpoint" },
                { "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                { "odt", "application/vnd.oasis.opendocument.text" },
        
                // Otros
                { "exe", "application/octet-stream" },
                { "dll", "application/octet-stream" },
                { "bin", "application/octet-stream" },
                { "rtf", "application/rtf" },
                { "woff", "font/woff" },
                { "woff2", "font/woff2" },
                { "ttf", "font/ttf" },
                { "otf", "font/otf" },

                // Extras
                { "epub", "application/epub+zip" },
                { "psd", "image/vnd.adobe.photoshop" }
            };

        }

        public static string GetMimeType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant().TrimStart('.');
            return MimeTypes.TryGetValue(extension, out string? mimeType)
           ? mimeType
           : "application/octet-stream"; // Valor por defecto
        }

        public static string GetRealExecutablePath()
        {
            // Primero intentar obtener la ruta real del proceso
            var processPath = Process.GetCurrentProcess().MainModule?.FileName;

            // Si estamos en modo single-file publicado
            if (!string.IsNullOrEmpty(processPath) && !processPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                return processPath;
            
            // Fallback para desarrollo
            return Environment.GetCommandLineArgs()[0];
        }


        public static async Task<string> DownloadWebView2Bootstrapper()
        {
            string url = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";
            string tempFile = Path.Combine(Path.GetTempPath(), "MicrosoftEdgeWebview2Setup.exe");

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                using (var fs = new FileStream(tempFile, FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }

            return tempFile;
        }

        public static bool InstallWebView2Runtime(string installerPath)
        {
            try
            {
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = installerPath,
                        // Sin argumentos = interfaz gráfica normal
                        UseShellExecute = true,
                        Verb = "runas" // Opcional: solicitar permisos de administrador
                    }
                };

                process.Start();
                process.WaitForExit(); // Esperar a que el usuario complete la instalación

                return process.ExitCode == 0;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                File.Delete(installerPath);
            }
        }
    
        public static string GetLanguage(string? language)
        {
            if (string.IsNullOrWhiteSpace(language))
                return CultureInfo.CurrentCulture.Name;

            try
            {
                var culture = CultureInfo.GetCultureInfo(language);
                return culture.Name;
            }
            catch (Exception)
            {
                return CultureInfo.CurrentCulture.Name;
            }
        }

        public static bool IsUri(string uri)
        {
            if (IsValidUri(uri, new[] { Uri.UriSchemeHttp, Uri.UriSchemeHttps, Uri.UriSchemeFtp, Uri.UriSchemeFile }))
                return true;

            return false;
        }

        private static bool IsValidUri(string input, string[] validSchemes)
        {
            if (Uri.TryCreate(input, UriKind.Absolute, out Uri? uri))
                foreach (var scheme in validSchemes)
                    if (uri.Scheme.Equals(scheme, StringComparison.OrdinalIgnoreCase))
                        return true;

            return false;
        }

        public static bool IsLocalPath(string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input)) 
                    return false;

                // Verifica caracteres inválidos
                if (input.IndexOfAny(Path.GetInvalidPathChars()) != -1) 
                    return false;

                // Verifica si es una ruta absoluta según el sistema operativo
                return Path.IsPathRooted(input);
            }
            catch
            {
                return false;
            }
        }

        public static IPluginContext CreateContext(IWebView? wv, ILogger log, string name, string? source = null, Action<string>? onDisposed = null)
        {
            return WV.Core.Utils.CreateContext(wv, log, name, source, onDisposed);
        }

        public static double INCH2CM(double value)
        {
            return value * 2.54;
        }

        public static double CM2INCH(double value)
        {
            return value / 2.54;
        }
    }
}
