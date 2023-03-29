using System.Text.Json.Serialization;

namespace WV.WebView.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WVState
    {
        Minimized,
        Restore,
        Maximized,
        FullScreen
    }
}