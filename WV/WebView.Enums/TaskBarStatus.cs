using System.Text.Json.Serialization;

namespace WV.WebView.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskBarStatus
    {
        None,
        Indeterminate,
        Normal,
        Error,
        Paused
    }
}