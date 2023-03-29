using System.Runtime.InteropServices;

namespace WV.WebView
{
    public abstract class Plugin : IDisposable
    {
        protected IWebView WebView { get; private set; }
        public string UID { get; }
        public string Name { get; }
        public bool Disposed { get; private set; }
        protected Plugin(IWebView webView)
        {
            this.WebView = webView;
            this.UID = Guid.NewGuid().ToString();
            this.Name = this.GetType().Name;
        }

        [ComVisible(false)]
        public void Dispose()
        {
            Dispose(true);

            // Evitar que el Garbage Collector llame al destructor/Finalizador ~Plugin()
            GC.SuppressFinalize(this);

            this.WebView = null;
            this.Disposed = true;
        }

        protected abstract void Dispose(bool disposing);

        ~Plugin()
        {
            Dispose(false);
            this.WebView = null;
            this.Disposed = true;
        }
    }
}