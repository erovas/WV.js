namespace Ports
{
    public static class Resources
    {
        public static string PortsScript => "";
        public static string SerialPortScript => "";
        public static string ParallelPortScript => "";

        public static int[] BaudRates { get; }

        static Resources()
        {
            BaudRates = new int[] {
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
    }
}