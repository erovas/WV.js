using System.Text.Json.Serialization;

namespace WV.JavaScript.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum JSEvent
    {
        state,
        position,
        activated,
        enabled,
        closing,
        sysmenu,
        error,
        playingaudio,
        muted
    }
}