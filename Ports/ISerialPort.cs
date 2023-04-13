using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WV.JavaScript;

namespace Ports
{
    public interface ISerialPort
    {
        int BaudRate { get; set; }
        bool BreakState { get; set; }
        int BytesToRead { get; }
        int BytesToWrite { get; }
        bool CDHolding { get; }
        int DataBits { get; set; }
        bool DiscardNull { get; set; }
        bool DsrHolding { get; }
        bool DtrEnable { get; set; }
        string Handshake { get; set; }
        bool IsOpen { get; }
        string NewLine { get; }
        string Parity { get; set; }
        byte ParityReplace { get; set; }
        string PortName { get; set; }
        int ReadBufferSize { get; set; }
        int ReadTimeout { get; set; }
        int ReceivedBytesThreshold { get; set; }
        bool RtsEnable { get; set; }
        string StopBits { get; set; }
        int WriteBufferSize { get; set; }
        int WriteTimeout { get; set; }

        void Open();
        void Close();
        void DiscardInBuffer();
        void DiscardOutBuffer();
        int ReadBytes(Uint8Array buffer, int offset, int count);
        int ReadChars(Int32Array buffer, int offset, int count);
        int ReadByte();
        string ReadChar();
        string ReadExisting();
        string ReadLine();
        string ReadTo(string value);
        void WriteBytes(Uint8Array buffer, int offset, int count);
        void WriteChars(Int32Array buffer, int offset, int count);
        void Write(string text);
        void WriteLine(string text);

        void addEventListener(string type, Function? listener, params object[] options);
        void removeEventListener(string type, Function? listener, params object[] options);

        Function? onerrorreceived { get; set; }
        Function? onpinchanged { get; set; }

    }
}
