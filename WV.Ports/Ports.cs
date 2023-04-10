using WV;
using WV.WebView;

namespace WV.Ports
{
    public class Ports : Plugin, IPlugin
    {
        public static string JScript => Resources.PortsScript;

        public int InfiniteTimeout => System.IO.Ports.SerialPort.InfiniteTimeout;

        public int[] BaudRate { get; }

        public Ports(IWebView webView) : base(webView)
        {
            this.BaudRate = new int[] {
                1200,
                2400,
                4800,
                9600,
                12900,
                28800,
                38400,
                57600,
                76800,
                115200,
                230400,
                460800,
                576000,
                921600
            };
        }

        public string[] GetPortNames()
        {
            return System.IO.Ports.SerialPort.GetPortNames();
        }




        protected override void Dispose(bool disposing)
        {
            //throw new NotImplementedException();
        }
    }
}
