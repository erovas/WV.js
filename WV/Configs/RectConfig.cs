using System.Text.Json.Serialization;

namespace WV.Configs
{
    public sealed class RectConfig
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public int MaxWidth { get; }
        public int MaxHeight { get; }

        public int MinWidth { get; }
        public int MinHeight { get; }

        [JsonConstructor]
        public RectConfig(
            int x = 0, int y = 0, 
            int width = App.Window.Rect.Width, int height = App.Window.Rect.Height,
            int maxWidth = App.Window.Rect.MaxWidth, int maxHeight = App.Window.Rect.MaxHeight, 
            int minWidth = App.Window.Rect.MinWidth, int minHeight = App.Window.Rect.MinHeight)
        {
            X = x;
            Y = y;

            // MinValue nunca puede ser menor que AppManager.MinWindowValue
            MinWidth = Math.Max(minWidth, App.Window.Rect.MinWidth);
            MinHeight = Math.Max(minHeight, App.Window.Rect.MinHeight);

            // MaxValue nunca puede ser menor que MinValue
            MaxWidth = Math.Max(maxWidth, MinWidth);
            MaxHeight = Math.Max(maxHeight, MinHeight);

            // Aplicamos el clamp después de tener Min/Max calculados
            Width = Utils.Clamp(width, MinWidth, MaxWidth);
            Height = Utils.Clamp(height, MinHeight, MaxHeight);
        }
    }
}