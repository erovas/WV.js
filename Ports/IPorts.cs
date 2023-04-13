using WV.JavaScript;
using WV.WebView;

namespace Ports
{
    public interface IPorts : IPlugin
    {
        int InfiniteTimeout { get; }
        int[] BaudRates { get; }
        string[] GetPortNames();

        void addEventListener(string type, Function? listener, params object[] options);
        void removeEventListener(string type, Function? listener, params object[] options);

        ISerialPort newSerialPort();
        ISerialPort newSerialPort(string portName);
        ISerialPort newSerialPort(string portName, int baudRate);
        ISerialPort newSerialPort(string portName, int baudRate, string parity);
        ISerialPort newSerialPort(string portName, int baudRate, string parity, int dataBits);
        ISerialPort newSerialPort(string portName, int baudRate, string parity, int dataBits, string stopBits);

        IParallelPort newParallelPort();
    }
}