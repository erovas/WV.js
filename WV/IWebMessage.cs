namespace WV
{
    public interface IWebMessage
    {
        /// <summary>
        /// Gets the URI of the document that sent this web message.
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Gets the message posted from the WebView/Frame content to the host as a string.
        /// </summary>
        string? MessageAsString { get; }

        /// <summary>
        /// Gets the message posted from the WebView/Frame content to the host converted to a JSON string.
        /// </summary>
        string MessageAsJson { get; }
    }
}