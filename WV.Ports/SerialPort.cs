using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using WV.JavaScript;
using WV.WebView;

namespace WV.Ports
{
    public class SerialPort : Plugin, IPlugin
    {
        public static string JScript => Resources.SerialPortScript;

        private const string DATA_RECEIVED = "datareceived";
        private const string ERROR_RECEIVED = "errorreceived";
        private const string PING_CHANGED = "pinchanged";

        #region PROPIEDADES PRIVADAS

        private System.IO.Ports.SerialPort InnerSerialPort { get; }

        private Function? InnerFNOnDataReceived { get; set; }
        private Function? InnerFNOnErrorReceived { get; set; }
        private Function? InnerFNOnPinChanged { get; set; }

        private List<Function> InnerFNDataReceived { get; } = new List<Function>();
        private List<Function> InnerFNErrorReceived { get; } = new List<Function>();
        private List<Function> InnerFNPinChanged { get; } = new List<Function>();

        #endregion

        #region PROPIEDADES PUBLICAS

        [ComVisible(false)]
        public Stream? BaseStream => this.InnerSerialPort.BaseStream;

        [ComVisible(false)]
        public Encoding Encoding
        {
            get => this.InnerSerialPort.Encoding;
            set => this.InnerSerialPort.Encoding = value;
        }

        public int BaudRate
        {
            get => this.InnerSerialPort.BaudRate;
            set => this.InnerSerialPort.BaudRate = value;
        }

        public bool BreakState
        {
            get => this.InnerSerialPort.BreakState;
            set => this.InnerSerialPort.BreakState = value;
        }

        public int BytesToRead => this.InnerSerialPort.BytesToRead;

        public int BytesToWrite => this.InnerSerialPort.BytesToWrite;

        public bool CDHolding => this.InnerSerialPort.CDHolding;

        public int DataBits
        {
            get => this.InnerSerialPort.DataBits;
            set => this.InnerSerialPort.DataBits = value;
        }

        public bool DiscardNull
        {
            get => this.InnerSerialPort.DiscardNull;
            set => this.InnerSerialPort.DiscardNull = value;
        }

        public bool DsrHolding => this.InnerSerialPort.DsrHolding;

        public bool DtrEnable
        {
            get => this.InnerSerialPort.DtrEnable;
            set => this.InnerSerialPort.DtrEnable = value;
        }

        public string Handshake
        {
            get => this.InnerSerialPort.Handshake.ToString();
            set
            {
                if(Enum.TryParse(value, out Handshake handshake))
                    this.InnerSerialPort.Handshake = handshake;
            }
        }

        public bool IsOpen => this.InnerSerialPort.IsOpen;

        public string NewLine => this.InnerSerialPort.NewLine;

        public string Parity
        {
            get => this.InnerSerialPort.Parity.ToString();
            set
            {
                if (Enum.TryParse(value, out Parity parity))
                    this.InnerSerialPort.Parity = parity;
            }
        }

        public byte ParityReplace
        {
            get => this.InnerSerialPort.ParityReplace;
            set => this.InnerSerialPort.ParityReplace = value;
        }

        public string PortName
        {
            get => this.InnerSerialPort.PortName;
            set => this.InnerSerialPort.PortName = value;
        }

        public int ReadBufferSize
        {
            get => this.InnerSerialPort.ReadBufferSize; 
            set => this.InnerSerialPort.ReadBufferSize = value;
        }

        public int ReadTimeout
        {
            get => this.InnerSerialPort.ReadTimeout; 
            set => this.InnerSerialPort.ReadTimeout = value;
        }

        public int ReceivedBytesThreshold
        {
            get => this.InnerSerialPort.ReceivedBytesThreshold; 
            set => this.InnerSerialPort.ReceivedBytesThreshold = value;
        }

        public bool RtsEnable
        {
            get => this.InnerSerialPort.RtsEnable; 
            set => this.InnerSerialPort.RtsEnable = value;
        }

        public string StopBits
        {
            get => this.InnerSerialPort.StopBits.ToString();
            set
            {
                if (Enum.TryParse(value, out StopBits stopBits))
                    this.InnerSerialPort.StopBits = stopBits;
            }
        }

        public int WriteBufferSize
        {
            get => this.InnerSerialPort.WriteBufferSize;
            set => this.InnerSerialPort.WriteBufferSize = value;
        }

        public int WriteTimeout
        {
            get => this.InnerSerialPort.WriteTimeout;
            set => this.InnerSerialPort.WriteTimeout = value;
        }

        #endregion

        #region CONSTRUCTORES

        public SerialPort(IWebView webView) : base(webView)
        {
            this.InnerSerialPort = new System.IO.Ports.SerialPort();
            AUX_SetEvents();
        }

        public SerialPort(IWebView webView, string portName) : this(webView)
        {
            this.PortName = portName;
        }

        public SerialPort(IWebView webView, string portName, int baudRate) : this(webView, portName)
        {
            this.BaudRate = baudRate;
        }

        public SerialPort(IWebView webView, string portName, int baudRate, string parity) : this(webView, portName, baudRate)
        {
            this.Parity = parity;
        }

        public SerialPort(IWebView webView, string portName, int baudRate, string parity, int dataBits) : this(webView, portName, baudRate, parity)
        {
            this.DataBits = dataBits;
        }

        public SerialPort(IWebView webView, string portName, int baudRate, string parity, int dataBits, string stopBits) : this(webView, portName, baudRate, parity, dataBits)
        {
            this.StopBits = stopBits;
        }

        #endregion

        #region METODOS PUBLICOS

        public void Open()
        {
            this.InnerSerialPort.Open();
        }

        public void Close()
        {
            this.InnerSerialPort.Close();
        }

        public void DiscardInBuffer()
        {
            this.InnerSerialPort.DiscardInBuffer();
        }

        public void DiscardOutBuffer()
        {
            this.InnerSerialPort.DiscardOutBuffer();
        }

        public int ReadBytes(Uint8Array buffer, int offset, int count)
        {
            byte[] bytes = buffer.CSValue;
            return this.InnerSerialPort.Read(bytes, offset, count);
        }

        public int ReadChars(Int32Array buffer, int offset, int count)
        {
            int[] myBuffer = buffer.CSValue;
            char[] chars = new char[myBuffer.Length];

            for (int i = 0; i < chars.Length; i++)
                chars[i] = (char)myBuffer[i];
            
            int output = this.InnerSerialPort.Read(chars, offset, count);

            for (int i = 0; i < chars.Length; i++)
                myBuffer[i] = int.Parse(chars[i].ToString());

            return output;
        }

        public int ReadByte()
        {
            return this.InnerSerialPort.ReadByte();
        }

        public string ReadChar()
        {
            return ((char)this.InnerSerialPort.ReadChar()).ToString();
        }

        public string ReadExisting()
        {
            return this.InnerSerialPort.ReadExisting();
        }

        public string ReadLine()
        {
            return this.InnerSerialPort.ReadLine();
        }

        public string ReadTo(string value)
        {
            return this.InnerSerialPort.ReadTo(value);
        }

        public void WriteBytes(Uint8Array buffer, int offset, int count)
        {
            byte[] bytes = buffer.CSValue;
            this.InnerSerialPort.Write(bytes, offset, count);
        }

        public void WriteChars(Int32Array buffer, int offset, int count)
        {
            int[] myBuffer = buffer.CSValue;
            char[] chars = new char[myBuffer.Length];

            for (int i = 0; i < chars.Length; i++)
                chars[i] = (char)myBuffer[i];

            this.InnerSerialPort.Write(chars, offset, count);
        }

        public void Write(string text)
        {
            this.InnerSerialPort.Write(text);
        }

        public void WriteLine(string text)
        {
            this.InnerSerialPort.WriteLine(text);
        }

        #endregion

        #region EVENTOS

        public void addEventListener(string type, Function? listener, params object[] options)
        {
            if (listener == null)
                return;

            type = (type + "").ToLower();

            switch (type)
            {
                case DATA_RECEIVED:
                    this.InnerFNDataReceived.Add(listener); 
                    break;

                case ERROR_RECEIVED:
                    this.InnerFNErrorReceived.Add(listener);
                    break;

                case PING_CHANGED:
                    this.InnerFNPinChanged.Add(listener);
                    break;

                default:
                    throw new Exception("Event ['" + type + "'] not exists" );
            }
        }

        public void removeEventListener(string type, Function? listener, params object[] options)
        {
            if (listener == null)
                return;

            type = (type + "").ToLower();
            List<Function> list = type switch
            {
                DATA_RECEIVED => this.InnerFNDataReceived,
                ERROR_RECEIVED => this.InnerFNErrorReceived,
                PING_CHANGED => this.InnerFNPinChanged,
                _ => throw new Exception("Event ['" + type + "'] not exists"),
            };
            AUX_RemoveInList(list, listener);
        }

        public Function? ondatareceived
        {
            get => this.InnerFNOnDataReceived;
            set
            {
                removeEventListener(DATA_RECEIVED, this.InnerFNOnDataReceived);
                addEventListener(DATA_RECEIVED, value);
                this.InnerFNOnDataReceived = value;
            }
        }

        public Function? onerrorreceived
        {
            get => this.InnerFNOnErrorReceived;
            set
            {
                removeEventListener(ERROR_RECEIVED, this.InnerFNOnErrorReceived);
                addEventListener(ERROR_RECEIVED, value);
                this.InnerFNOnErrorReceived = value;
            }
        }

        public Function? onpinchanged
        {
            get => this.InnerFNOnPinChanged;
            set
            {
                removeEventListener(PING_CHANGED, this.InnerFNOnPinChanged);
                addEventListener(PING_CHANGED, value);
                this.InnerFNOnPinChanged = value;
            }
        }

        #endregion

        #region METODOS PRIVADOS

        protected override void Dispose(bool disposing)
        {
            if(this.Disposed)
                return;

            if(disposing) 
            {
                
            }

            AUX_RemoveEvents();
            this.InnerSerialPort.Dispose();
        }

        private static void AUX_RemoveInList(List<Function> list, Function? fn)
        {
            if(fn  == null)
                return;

            Function? item = list.Where(x => x.JSValue == fn.JSValue).FirstOrDefault();

            if(item != null)
                list.Remove(item);
        }

        private void AUX_SetEvents()
        {
            this.InnerSerialPort.DataReceived += InnerSerialPort_DataReceived;
            this.InnerSerialPort.ErrorReceived += InnerSerialPort_ErrorReceived;
            this.InnerSerialPort.PinChanged += InnerSerialPort_PinChanged;
        }

        private void AUX_RemoveEvents()
        {
            this.InnerFNOnDataReceived = null;
            this.InnerFNOnErrorReceived = null;
            this.InnerFNOnPinChanged = null;

            this.InnerFNDataReceived.Clear();
            this.InnerFNErrorReceived.Clear();
            this.InnerFNPinChanged.Clear();

            this.InnerSerialPort.DataReceived -= InnerSerialPort_DataReceived;
            this.InnerSerialPort.ErrorReceived -= InnerSerialPort_ErrorReceived;
            this.InnerSerialPort.PinChanged -= InnerSerialPort_PinChanged;
        }

        private void InnerSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            foreach (var item in this.InnerFNDataReceived)
                item.Execute(e.EventType.ToString(), e.EventType);
        }

        private void InnerSerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            foreach (var item in this.InnerFNErrorReceived)
                item.Execute(e.EventType.ToString(), e.EventType);
        }

        private void InnerSerialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            foreach (var item in this.InnerFNPinChanged)
                item.Execute(e.EventType.ToString(), e.EventType);
        }

        #endregion
    }
}