namespace WV.Windows
{
    public class WebMessage : IWebMessage
    {
        public string Source { get; }

        public string? MessageAsString { get; }

        public string MessageAsJson { get; }

        public WebMessage(string source, string? msgString, string msgJson) 
        {
            this.Source = source;
            this.MessageAsString = msgString;
            this.MessageAsJson = msgJson;
        }
    }
}