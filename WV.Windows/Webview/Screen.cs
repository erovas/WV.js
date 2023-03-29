using WV.WebView;

namespace WV.Windows.Webview
{
    public class Screen : IScreen
    {
        public bool Primary { get; }
        public string DeviceName { get; }
        public int BitsPerPixel { get; }
        public IArea WorkingArea { get; }
        public IArea Bounds { get; }

        public Screen(System.Windows.Forms.Screen sc)
        {
            Primary = sc.Primary;
            DeviceName = sc.DeviceName;
            BitsPerPixel = sc.BitsPerPixel;
            WorkingArea = new Area(sc.WorkingArea);
            Bounds = new Area(sc.Bounds);
        }
    }
}
