namespace WV.NotificationIcon
{
    public interface IColor
    {
        bool IsEmpty { get; }
        bool IsNamedColor { get; }
        bool IsSystemColor { get; }
        float Brightness { get; }
        float Hue { get; }
        float Saturation { get; }

        byte R { get; set; }
        byte G { get; set; }
        byte B { get; set; }
        byte A { get; set; }
        string Name { get; set; }
        int ARGB { get; set; }

        void SetRGB(int red, int green, int blue);
        void SetARGB(int alpha, int red, int green, int blue);
    }
}