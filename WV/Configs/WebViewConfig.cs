namespace WV.Configs
{
    public sealed class WebViewConfig
    {
        public int Version { get; init; } = 1;

        public LoggerConfig Logger { get; init; } = new();

        public BrowserConfig Browser { get; init; } = new();

        public PluginsConfig Plugins { get; init; } = new();

        public WindowConfig Window { get; init; } = new();

        public PrintManagerConfig PrintManager { get; init; } = new();
    }
}