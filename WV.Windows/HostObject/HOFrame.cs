namespace WV.Windows.HostObject
{
    public class HOFrame : HOBase
    {
        public HOFrame(IWebView webView) : base(webView)
        {
            this.InnerIsFrame = true;
        }

        protected override void Dispose(bool disposing)
        {
            
        }
    }
}
