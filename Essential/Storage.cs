using WV;
using WV.WebView;

namespace Essential
{
    public class Storage : Plugin
    {

        private Dictionary<string, string> Data { get; set; }

        public Storage(IWebView webView) : base(webView)
        {
            Data = App.Storage;
        }

        public int Count => this.Data.Count;

        public string[] Keys => this.Data.Keys.ToArray();

        public string[] Values => this.Data.Values.ToArray();

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
        public string? Get(string name)
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
        public bool Set(string name, string value)
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
        public bool Add(string name, string value)
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
            this.Data = null;
        }
    }
}