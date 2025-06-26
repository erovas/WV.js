using Microsoft.Web.WebView2.Core;
using WV.Enums;
using WV.Interfaces;

namespace WV.Win.Imp
{
    public class PrintManager : Plugin, IPrintManager
    {
        #region HELPERS

        private static double INCH2CM(double value)
        {
            return value * 2.54;
        }

        private static double CM2INCH(double value)
        {
            return value / 2.54;
        }

        #endregion

        private CoreWebView2PrintSettings? _PrintSettings;
        private CoreWebView2PrintSettings? PrintSettings 
        {
            get => _PrintSettings;
            set
            {
                _PrintSettings = value;
                _PrintSettings!.PageWidth = CM2INCH(21);
                _PrintSettings!.PageHeight = CM2INCH(29.7);
            }
        }
        private WebView WV { get; }


        private event AppManager.WVEventHandler<PrintStatus, string>? printFinished;
        public event AppManager.WVEventHandler<PrintStatus, string>? PrintFinished
        {
            add
            {
                Plugin.ThrowDispose(this.WV);
                printFinished += value;
            }
            remove
            {
                Plugin.ThrowDispose(this.WV);
                printFinished -= value;
            }
        }

        public bool IsBusy {  get; private set; }

        public PrintManager(WebView wv) : base(wv)
        {
            this.WV = wv;
        }

        #region PROPS

        public PrintOrientation Orientation 
        { 
            get
            {
                Plugin.ThrowDispose(this.WV);
                return (PrintOrientation)this.PrintSettings!.Orientation;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if(value == this.Orientation)
                    return;

                this.PrintSettings!.Orientation = (CoreWebView2PrintOrientation)value;
            }
        }
        public string OrientationText 
        { 
            get => this.Orientation.ToString();
            set
            {
                if (Enum.TryParse(value, out PrintOrientation orientation))
                    this.Orientation = orientation;
            }
        }

        #region MARGIN

        public double MarginBottom 
        { 
            get
            {
                Plugin.ThrowDispose(this.WV);
                return INCH2CM(this.PrintSettings!.MarginBottom);
            }
            set 
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.MarginBottom = CM2INCH(value);
            }
        }

        public double MarginLeft
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return INCH2CM(this.PrintSettings!.MarginLeft);
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.MarginLeft = CM2INCH(value);
            }
        }

        public double MarginRight
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return INCH2CM(this.PrintSettings!.MarginRight);
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.MarginRight = CM2INCH(value);
            }
        }

        public double MarginTop
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return INCH2CM(this.PrintSettings!.MarginTop);
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.MarginTop = CM2INCH(value);
            }
        }

        #endregion

        #region PAGE SIZE

        public double PageWidth 
        { 
            get
            {
                Plugin.ThrowDispose(this.WV);
                return INCH2CM(this.PrintSettings!.PageWidth);
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.PageWidth = CM2INCH(value);
            }
        }

        public double PageHeight
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return INCH2CM(this.PrintSettings!.PageHeight);
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.PageHeight = CM2INCH(value);
            }
        }

        #endregion


        public double ScaleFactor 
        { 
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.PrintSettings!.ScaleFactor;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.ScaleFactor = value;
            }
        }

        public bool PrintBackgrounds 
        { 
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.PrintSettings!.ShouldPrintBackgrounds;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.ShouldPrintBackgrounds = value;
            }
        }

        public bool PrintSelectionOnly
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.PrintSettings!.ShouldPrintSelectionOnly;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.ShouldPrintSelectionOnly = value;
            }
        }

        public bool PrintHeaderAndFooter
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.PrintSettings!.ShouldPrintHeaderAndFooter;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.ShouldPrintHeaderAndFooter = value;
            }
        }

        public string FooterUri
        {
            get 
            {
                Plugin.ThrowDispose(this.WV);
                return this.PrintSettings!.FooterUri;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.FooterUri = value;
            }
        }

        public string PageRanges
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.PrintSettings!.PageRanges;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.PageRanges = value;
            }
        }

        public int Copies
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.PrintSettings!.Copies;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.Copies = value;
            }
        }

        public int PagesPerSide
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.PrintSettings!.PagesPerSide;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.PagesPerSide = value;
            }
        }

        public string PrinterName
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.PrintSettings!.PrinterName;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.PrinterName = value;
            }
        }

        public PrintDuplex Duplex
        {
            get 
            {
                Plugin.ThrowDispose(this.WV);
                return (PrintDuplex)this.PrintSettings!.Duplex;
            }
            set 
            { 
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.Duplex = (CoreWebView2PrintDuplex)value;
            }
        }
        public string DuplesText 
        { 
            get => this.Duplex.ToString();
            set
            {
                if (Enum.TryParse(value, out PrintDuplex Duplex))
                    this.Duplex = Duplex;
            }
        }

        public PrintColorMode ColorMode
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return (PrintColorMode)this.PrintSettings!.ColorMode;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.ColorMode = (CoreWebView2PrintColorMode)value;
            }
        }

        public string ColorModeText
        {
            get => this.ColorMode.ToString();
            set
            {
                if (Enum.TryParse(value, out PrintColorMode color))
                    this.ColorMode = color;
            }
        }

        public PrintCollation Collation
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return (PrintCollation)this.PrintSettings!.Collation;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.PrintSettings!.Collation = (CoreWebView2PrintCollation)value;
            }
        }

        public string CollationText
        {
            get => this.Collation.ToString();
            set
            {
                if (Enum.TryParse(value, out PrintCollation collation))
                    this.Collation = collation;
            }
        }

        #endregion

        #region METHODS

        public void Print()
        {
            Plugin.ThrowDispose(this.WV);

            if (this.IsBusy || this.PrintSettings == null || this.WV.WVController == null)
                return;

            this.IsBusy = true;

            Task.Run(async () =>
            {
                if (this.PrintSettings == null || this.WV.WVController == null)
                {
                    this.IsBusy = false;
                    return;
                }

                var result = await this.WV.WVController.CoreWebView2.PrintAsync(this.PrintSettings);
                this.FireEvent((PrintStatus)result, result.ToString());
                this.IsBusy = false;
            });
        }

        public void PrintToPDF(string ResultFilePath)
        {
            Plugin.ThrowDispose(this.WV);

            if (this.IsBusy || this.PrintSettings == null || this.WV.WVController == null)
                return;

            this.IsBusy = true;

            Task.Run(async () =>
            {
                if (this.PrintSettings == null || this.WV.WVController == null)
                {
                    this.IsBusy = false;
                    return;
                }

                bool result = await this.WV.WVController.CoreWebView2.PrintToPdfAsync(ResultFilePath ,this.PrintSettings);
                PrintStatus status = result ? PrintStatus.Succeeded : PrintStatus.OtherError;
                this.FireEvent(status, status.ToString());
                this.IsBusy = false;
            });
        }

        #endregion

        private void FireEvent(PrintStatus status, string strStatus)
        {
            this.JSfn?.Execute(status, strStatus);
            this.printFinished?.Invoke(this.WV, status, strStatus);
        }

        internal void ClearEvents()
        {
            this.JSfn = null;
            this.CleanJSEvents();
            this.printFinished = null;
        }

        internal void ToDefault()
        {
            this.PrintSettings = this.WV.WVController?.CoreWebView2.Environment.CreatePrintSettings();
            this.IsBusy = false;
        }


        private IJSFunction? JSfn {  get; set; }
        public object? OnPrintFinished
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.JSfn?.Raw;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);

                if (value == JSfn?.Raw)
                    return;

                this.JSfn?.Dispose();
                this.JSfn = null;

                if (value == null)
                    return;

                this.JSfn = IJSFunction.Create(value);
            }
        }

        #region DISPOSE

        protected override void Dispose(bool disposing)
        {
            
        }

        public override void Dispose()
        {
            
        }

        #endregion
    }
}