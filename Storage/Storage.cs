using WV;
using WV.WebView;

namespace Storage
{
    public class Storage : Plugin, IPlugin
    {
        public static string JScript => Resources.StorageScript;

        private Dictionary<string, object?> Data { get; set; }

        public Storage(IWebView webView) : base(webView)
        {
            this.Data = App.Storage;
        }

        public int Count => this.Data.Count;

        public string[] Keys => this.Data.Keys.ToArray();

        public object?[] Values => this.Data.Values.ToArray();

        public void Clear()
        {
            this.Data.Clear();
        }

        public bool Exists(string name)
        {
            return String.IsNullOrEmpty(name) ? false : this.Data.ContainsKey(name);
        }

        /// <summary>
        /// Get an existing value
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Null if it does not exist</returns>
        public object? Get(string name)
        {
            if (Exists(name))
                return this.Data[name];

            return null;
        }

        /// <summary>
        /// Set an existing value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns>False if the key does not exist</returns>
        public bool Set(string name, object? value)
        {
            if (!Exists(name))
                return false;

            this.Data[name] = value;
            return true;
        }

        /// <summary>
        /// Add a new value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns>false if the key exists</returns>
        public bool Add(string name, object? value)
        {
            if (Exists(name))
                return false;

            this.Data[name] = value;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Remove(string name)
        {
            if (Exists(name))
                return this.Data.Remove(name);

            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.Disposed)
                return;
            
            if (disposing)
            {

            }

            this.Data = null;
        }
    }
}