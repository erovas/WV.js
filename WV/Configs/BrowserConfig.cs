using WV.Enums;

namespace WV.Configs
{
    public sealed class BrowserConfig
    {
        public string? Language { get; init; }

        public string Directory 
        { 
            get; 
            init
            {
                field = string.IsNullOrWhiteSpace(value) ? App.Browser.Directory : value;
            }
        } = App.Browser.Directory;

        public string Uri 
        { 
            get; 
            init
            {
                field = string.IsNullOrWhiteSpace(value) ? App.Browser.Uri : value;
            }
        } = App.Browser.Uri;

        public bool HotReload { get; init; } = App.Browser.HotReload;

        public bool AcceleratorKeys { get; init; } = App.Browser.AcceleratorKeys;

        public bool SwipeNavigation { get; init; } = App.Browser.SwipeNavigation;

        public double ZoomFactor 
        { 
            get; 
            init
            {
                field = value > App.Browser.MaxZoomFactor || value < App.Browser.MinZoomFactor ? App.Browser.ZoomFactor : value;
            }
        } = App.Browser.ZoomFactor;

        public bool Muted { get; init; } = App.Browser.Muted;

        public bool StatusBarEnabled { get; init; } = App.Browser.StatusBarEnabled;

        public BrowserColorScheme ColorScheme { get; init; } = App.Browser.ColorScheme;
    }
}