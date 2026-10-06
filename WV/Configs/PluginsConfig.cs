namespace WV.Configs
{
    public sealed class PluginsConfig
    {
        public string Directory 
        {
            get; 
            init
            {
                field = string.IsNullOrWhiteSpace(value) ? App.Plugins.Directory : value;
            }
        } = App.Plugins.Directory;

        public string[] Load { get; init; } = [];
    }
}