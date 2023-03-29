using Microsoft.Web.WebView2.Core;
using System.Runtime.InteropServices;
using WV.Windows.HostObject;

namespace WV.Windows
{
    public class Frame : IFrame
    {
        #region PRIVATE

        [ComVisible(false)]
        public CoreWebView2Frame InnerIFrame { get; }
        private List<EventHandler<IWebMessage>> InnerDelegatesWMR { get; }
        private HOFrame InnerHOFrame { get; }

        private event EventHandler<IWebMessage> InnerWebMessageReceived = delegate { };

        #endregion

        #region PROPERTIES

        public string Name => this.InnerIFrame.Name;

        private bool _IsDestroyed;
        public bool IsDestroyed
        {
            get => _IsDestroyed || this.InnerIFrame.IsDestroyed() > 0;
            private set => _IsDestroyed = value;
        }

        public bool EnablePlugins 
        {
            get => this.InnerHOFrame.InnerEnablePlugins;
            set => this.InnerHOFrame.InnerEnablePlugins = value;
        }

        public event EventHandler<IWebMessage> WebMessageReceived
        {
            add
            {
                if (value == null)
                    return;

                InnerWebMessageReceived += value;
                InnerDelegatesWMR.Add(value);

                if (InnerDelegatesWMR.Count == 1)
                    this.InnerIFrame.WebMessageReceived += IFrame_WebMessageReceived;
            }
            remove
            {
                if (value == null)
                    return;

                InnerWebMessageReceived -= value;
                InnerDelegatesWMR.Remove(value);

                if (InnerDelegatesWMR.Count == 0)
                    this.InnerIFrame.WebMessageReceived -= IFrame_WebMessageReceived;
            }
        }

        #endregion

        #region METHODS

        public Task<string> ExecuteScriptAsync(string javaScript)
        {
            return this.InnerIFrame.ExecuteScriptAsync(javaScript);
        }

        public void SendMessageAsJson(string messageAsJson)
        {
            this.InnerIFrame.PostWebMessageAsJson(messageAsJson);
        }

        public void SendMessageAsString(string messageAsString)
        {
            this.InnerIFrame.PostWebMessageAsString(messageAsString);
        }

        #endregion

        public Frame(CoreWebView2Frame frame, IWebView webview)
        {
            this.InnerIFrame = frame;
            this.InnerDelegatesWMR = new List<EventHandler<IWebMessage>>();
            this.InnerHOFrame = new HOFrame(webview);
            this.InnerIFrame.AddHostObjectToScript(AppManager.HostObjectName, this.InnerHOFrame, new List<string> { "*" });
            this.InnerIFrame.Destroyed += IFrame_Destroyed;
        }

        private void IFrame_Destroyed(object? sender, object e)
        {
            this.IsDestroyed = true;
            this.InnerHOFrame.Dispose();
            this.InnerIFrame.Destroyed -= IFrame_Destroyed;
            this.InnerIFrame.WebMessageReceived -= IFrame_WebMessageReceived;
        }

        private void IFrame_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            InnerWebMessageReceived?.Invoke(this, new WebMessage(e.Source, e.TryGetWebMessageAsString(), e.WebMessageAsJson));
        }

    }
}