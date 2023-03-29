using System.Runtime.InteropServices;
using WV.JavaScript;
using WV.JavaScript.Enums;

namespace WV.Windows.HostObject
{
    public class HOWindows : HOBase
    {
        [ComVisible(false)]
        public Dictionary<JSEvent, Function> JSEvents { get; }

        public HOWindows(IWebView webView) : base(webView, true)
        {
            this.JSEvents = new Dictionary<JSEvent, Function>();
        }

        /// <summary>
        /// Add JSEvent
        /// </summary>
        /// <param name="jsEventName"></param>
        /// <param name="fn"></param>
        public void AE(string jsEventName, object fn, string stringified)
        {
            JSEvent jsEvent = JSEvent.state;

            try
            {
                jsEvent = (JSEvent)Enum.Parse(typeof(JSEvent), jsEventName);
            }
            catch (Exception) { }

            this.JSEvents[jsEvent] = new JavaScript.Function(fn, stringified);
        }

        [ComVisible(false)]
        public override void Reloaded()
        {
            base.Reloaded();
            this.JSEvents.Clear();
        }

    }
}