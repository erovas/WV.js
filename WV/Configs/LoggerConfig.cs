using WV.Enums;

namespace WV.Configs
{
    public sealed class LoggerConfig
    {
        public string Source 
        { 
            get; 
            init
            {
                field = string.IsNullOrWhiteSpace(value) ? App.Logger.Source : value;
            } 
        } = App.Logger.Source;

        public bool Enabled { get; init; } = App.Logger.Enabled;

        public LogLevel MinimumLevel { get; init; } = App.Logger.MinimumLevel; // Trace|Debug|Info|Warning|Error|Critical|None
        
        public string Directory 
        { 
            get; 
            init
            {
                field = string.IsNullOrWhiteSpace(value) ? App.Logger.Directory : value;
            }
        } = App.Logger.Directory;

        public long MaxFileBytes 
        { 
            get; 
            init
            {
                field = value <= 0 ? App.Logger.MaxFileBytes : value;
            }
        } = App.Logger.MaxFileBytes;
    }
}