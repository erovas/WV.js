namespace WV
{
    public interface IFrame
    {
        /// <summary>
        /// Event that is fired when ...
        /// </summary>
        event EventHandler<IWebMessage> WebMessageReceived;

        /// <summary>
        /// The name of the iframe from the iframe html tag declaring it.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Is true when iframe is not in DOM.
        /// </summary>
        bool IsDestroyed { get; }

        /// <summary>
        /// When Frame is created allow create plugin instance in JavaScript
        /// </summary>
        bool EnablePlugins { get; set; }

        /// <summary>
        /// Runs JavaScript code from the javaScript parameter in the current frame.
        /// </summary>
        /// <param name="javaScript"></param>
        /// <returns></returns>
        Task<string> ExecuteScriptAsync(string javaScript);

        /// <summary>
        /// Posts the specified messageAsJson to the current frame.
        /// </summary>
        /// <param name="messageAsJson"></param>
        void SendMessageAsJson(string messageAsJson);

        /// <summary>
        /// Posts a message that is a simple string rather than a JSON string representation
        /// of a JavaScript object.
        /// </summary>
        /// <param name="messageAsString"></param>
        void SendMessageAsString(string messageAsString);
    }
}
