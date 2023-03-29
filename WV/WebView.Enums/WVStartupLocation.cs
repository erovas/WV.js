using System.Text.Json.Serialization;

namespace WV.WebView.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WVStartupLocation
    {
        Manual = 0,
        CenterScreen = 1
    }
}