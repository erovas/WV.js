using WV.Enums;

namespace WV.Configs
{
    public sealed class WindowConfig
    {
        public string Title { get; init; } = App.Window.Title;
        public WindowState State { get; init; } = App.Window.State;
        public bool CenterScreen { get; init; } = false;
        public RectConfig Rect 
        {  
            get; 
            init
            {
                field = value is null? new RectConfig() : value;
            } 
        }  = new();
    }
}