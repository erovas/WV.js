using WV.Enums;
using WV.Interfaces;
using static WV.AppManager;
using Microsoft.Web.WebView2.Core;

namespace WV.Win.Imp
{
    public class PrintManager : Plugin, IPrintManager
    {
        #region Statics

        private static double INCH2CM(double value)
        {
            return value * 2.54;
        }

        private static double CM2INCH(double value)
        {
            return value / 2.54;
        }

        #endregion

        //=======================================//

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
        private WebView WV => (WebView)this.WebView;

        //=======================================//

        #region Events 

        public event WVEventHandler<PrintStatus, string>? PrintFinished;

        #endregion

        public PrintManager(IContext ctx) : base(ctx)
        {
            
        }

        #region Properties

        public bool IsBusy { get; private set; }

        public PrintOrientation Orientation 
        { 
            get
            {
                ThrowIfDisposed();
                return (PrintOrientation)this.PrintSettings!.Orientation;
            }
            set
            {
                ThrowIfDisposed();

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
                ThrowIfDisposed();
                return INCH2CM(this.PrintSettings!.MarginBottom);
            }
            set 
            {
                ThrowIfDisposed();
                this.PrintSettings!.MarginBottom = CM2INCH(value);
            }
        }

        public double MarginLeft
        {
            get
            {
                ThrowIfDisposed();
                return INCH2CM(this.PrintSettings!.MarginLeft);
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.MarginLeft = CM2INCH(value);
            }
        }

        public double MarginRight
        {
            get
            {
                ThrowIfDisposed();
                return INCH2CM(this.PrintSettings!.MarginRight);
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.MarginRight = CM2INCH(value);
            }
        }

        public double MarginTop
        {
            get
            {
                ThrowIfDisposed();
                return INCH2CM(this.PrintSettings!.MarginTop);
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.MarginTop = CM2INCH(value);
            }
        }

        #endregion

        #region PAGE SIZE

        public double PageWidth 
        { 
            get
            {
                ThrowIfDisposed();
                return INCH2CM(this.PrintSettings!.PageWidth);
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.PageWidth = CM2INCH(value);
            }
        }

        public double PageHeight
        {
            get
            {
                ThrowIfDisposed();
                return INCH2CM(this.PrintSettings!.PageHeight);
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.PageHeight = CM2INCH(value);
            }
        }

        #endregion


        public double ScaleFactor 
        { 
            get
            {
                ThrowIfDisposed();
                return this.PrintSettings!.ScaleFactor;
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.ScaleFactor = value;
            }
        }

        public bool PrintBackgrounds 
        { 
            get
            {
                ThrowIfDisposed();
                return this.PrintSettings!.ShouldPrintBackgrounds;
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.ShouldPrintBackgrounds = value;
            }
        }

        public bool PrintSelectionOnly
        {
            get
            {
                ThrowIfDisposed();
                return this.PrintSettings!.ShouldPrintSelectionOnly;
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.ShouldPrintSelectionOnly = value;
            }
        }

        public bool PrintHeaderAndFooter
        {
            get
            {
                ThrowIfDisposed();
                return this.PrintSettings!.ShouldPrintHeaderAndFooter;
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.ShouldPrintHeaderAndFooter = value;
            }
        }

        public string FooterUri
        {
            get 
            {
                ThrowIfDisposed();
                return this.PrintSettings!.FooterUri;
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.FooterUri = value;
            }
        }

        public string PageRanges
        {
            get
            {
                ThrowIfDisposed();
                return this.PrintSettings!.PageRanges;
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.PageRanges = value;
            }
        }

        public int Copies
        {
            get
            {
                ThrowIfDisposed();
                return this.PrintSettings!.Copies;
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.Copies = value;
            }
        }

        public int PagesPerSide
        {
            get
            {
                ThrowIfDisposed();
                return this.PrintSettings!.PagesPerSide;
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.PagesPerSide = value;
            }
        }

        public string PrinterName
        {
            get
            {
                ThrowIfDisposed();
                return this.PrintSettings!.PrinterName;
            }
            set
            {
                ThrowIfDisposed();
                this.PrintSettings!.PrinterName = value;
            }
        }

        public PrintDuplex Duplex
        {
            get 
            {
                ThrowIfDisposed();
                return (PrintDuplex)this.PrintSettings!.Duplex;
            }
            set 
            { 
                ThrowIfDisposed();
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
                ThrowIfDisposed();
                return (PrintColorMode)this.PrintSettings!.ColorMode;
            }
            set
            {
                ThrowIfDisposed();
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
                ThrowIfDisposed();
                return (PrintCollation)this.PrintSettings!.Collation;
            }
            set
            {
                ThrowIfDisposed();
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

        //=======================================//

        #region Methods

        public void Print()
        {
            ThrowIfDisposed();

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
                this.FireEvent((PrintStatus)result);
                this.IsBusy = false;
            });
        }

        public void PrintToPDF(string ResultFilePath)
        {
            ThrowIfDisposed();

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
                this.FireEvent(status);
                this.IsBusy = false;
            });
        }

        #endregion

        //=======================================//

        #region Protected Methods

        protected override void ThrowIfDisposed()
        {
            base.ThrowIfDisposed();
            Plugin.ThrowIfDisposed(this.WV);
        }

        #endregion

        //=======================================//

        #region Internal Methods

        internal void ClearAllEvents()
        {
            this.ClearListeners();
            this.ClearEvents();
        }

        internal void ToDefault()
        {
            this.PrintSettings = this.WV.WVController?.CoreWebView2.Environment.CreatePrintSettings();
            this.IsBusy = false;
        }

        #endregion

        //=======================================//

        #region Private Methods

        private void FireEvent(PrintStatus status)
        {
            if (this.Disposed)
                return;

            this.PrintFinished?.Invoke(this.WV, status, status.ToString());
        }

        #endregion

    }
}