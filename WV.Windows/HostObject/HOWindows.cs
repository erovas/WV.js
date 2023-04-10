using System.Runtime.InteropServices;
using WV.JavaScript;
using WV.JavaScript.Enums;

namespace WV.Windows.HostObject
{
    public class HOWindows : HOBase
    {
        [ComVisible(false)]
        public Dictionary<JSEvent, Function> JSEvents { get; }

        public string[] Args { get; }

        public HOWindows(IWebView webView, string[] args) : base(webView)
        {
            this.JSEvents = new Dictionary<JSEvent, Function>();
            this.Args = args;
        }

        /// <summary>
        /// Add JSEvent
        /// </summary>
        /// <param name="jsEventName"></param>
        /// <param name="fn"></param>
        public void AE(string jsEventName, object fn, string stringified)
        {
            if(Enum.TryParse(jsEventName, out JSEvent jsEvent))
                this.JSEvents[jsEvent] = new JavaScript.Function(fn, stringified);
        }

        protected override void Dispose(bool disposing)
        {
            if(this.Disposed)
                return;

            if (disposing)
            {

            }

            this.JSEvents?.Clear();
        }

    }
}