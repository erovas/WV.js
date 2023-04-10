using System.Reflection;
using WV.WebView;

namespace WV.Essential
{
    public class FileSystem : Plugin, IPlugin
    {

        public static string JScript => Resources.FileSystemScript;

        public FileSystem(IWebView webView) : base(webView)
        {
        }

        /// <summary>
        /// Gets the current working directory where application was launched.
        /// </summary>
        /// <returns></returns>
        public string CurrentDirectory => Directory.GetCurrentDirectory();


        /// <summary>
        /// Gets the current working directory of the application.
        /// </summary>
        public string? CurrentAppDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Returns the path of the current user's temporary folder.
        /// </summary>
        /// <returns></returns>
        public string TempPath => Path.GetTempPath();


        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="path"></param>
        public void Delete(string path)
        {
            File.Delete(path);
        }

        /// <summary>
        /// Creates or overwrites a file in the specified path.
        /// </summary>
        /// <param name="path"></param>
        public void Create(string path)
        {
            var data = File.Create(path);
            data.Close();
        }

        /// <summary>
        /// Moves a specified file to a new location, providing the option to specify a new file name.
        /// </summary>
        /// <param name="pathFrom"></param>
        /// <param name="pathTo"></param>
        public void Move(string pathFrom, string pathTo)
        {
            File.Move(pathFrom, pathTo);
        }

        /// <summary>
        /// Opens a text file, reads all the text in the file, and then closes the file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string Read(string path)
        {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Creates a new file, writes the specified string to the file, and then closes the file. 
        /// If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        public void Write(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        /// <summary>
        /// Opens a file, appends the specified string to the file, and then closes the file.
        /// If the file does not exist, this method creates a file, writes the specified
        /// string to the file, then closes the file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        public void Append(string path, string text)
        {
            File.AppendAllText(path, text);
        }

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns>
        /// true if path refers to an existing directory; false if the directory does not
        /// exist or an error occurs when trying to determine if the specified directory exists.
        /// </returns>
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Creates all directories and subdirectories in the specified path unless they already exist.
        /// </summary>
        /// <param name="path">The directory to create.</param>
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Deletes the specified directory and, if indicated, any subdirectories and files
        /// in the directory.
        /// </summary>
        /// <param name="path">The name of the directory to remove.</param>
        public void DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
        }

        /// <summary>
        /// Returns an array of directory full names in a specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>
        /// An array of the full names (including paths) for the directories
        /// in the directory specified by path.
        /// </returns>
        public string[] EnumerateDirectories(string path)
        {
            var data = Directory.EnumerateDirectories(path);
            return data.ToArray();
        }

        /// <summary>
        /// Returns an array of full file names in a specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>
        /// An array of the full names (including paths) for the files in
        /// the directory specified by path.
        /// </returns>
        public string[] EnumerateFiles(string path)
        {
            var data = Directory.EnumerateFiles(path);
            return data.ToArray();
        }

        /// <summary>
        /// Gets the path to the special folder that is identified by the especified name
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public string GetFolderPath(string folder)
        {
            if (Enum.TryParse(folder, out Environment.SpecialFolder _folder))
                return Environment.GetFolderPath(_folder);

            return "";
        }

        protected override void Dispose(bool disposing)
        {
            //throw new NotImplementedException();
        }
    }
}
