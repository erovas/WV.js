using Microsoft.Web.WebView2.Core;
using WV.Configs;
using WV.Core;
using WV.Enums;
using WV.Interfaces;

namespace WV.Win.Imp
{
    public class PrintManager : PrintManagerCore, IPrintManager
    {
        #region Fields

        private CoreWebView2PrintSettings? _printSettings;
        private bool _isBusy;

        #endregion

        private CoreWebView2PrintSettings PrintSettings 
        {
            get
            {
                if (_printSettings is null)
                    _printSettings = CreatePrintSettings() ?? throw new NullReferenceException();

                return _printSettings;
            }
        }

        private CoreWebView2Controller WVController => ((WebView)this.WebView).InternalBrowser.WVController;

        //=======================================//

        public PrintManager(IPluginContext ctx, PrintManagerConfig printManagerConfig) : base(ctx, printManagerConfig)
        {
            
        }

        protected override void Initialize(IPluginContext context, PrintManagerConfig printManagerConfig)
        {
            
        }

        #region Properties

        protected override bool IsBusyCore => _isBusy;

        protected override PrintOrientation OrientationCore
        { 
            get
            {
                return (PrintOrientation)this.PrintSettings.Orientation;
            }
            set
            {
                if(value == this.OrientationCore)
                    return;

                this.PrintSettings.Orientation = (CoreWebView2PrintOrientation)value;
            }
        }

        protected override string OrientationTextCore
        { 
            get => this.OrientationCore.ToString();
            set
            {
                if (Enum.TryParse(value, out PrintOrientation orientation))
                    this.OrientationCore = orientation;
            }
        }

        #region MARGIN

        protected override double MarginBottomCore
        { 
            get => Utils.INCH2CM(this.PrintSettings.MarginBottom);
            set => this.PrintSettings.MarginBottom = Utils.CM2INCH(value);
        }

        protected override double MarginLeftCore
        {
            get => Utils.INCH2CM(this.PrintSettings.MarginLeft);
            set => this.PrintSettings.MarginLeft = Utils.CM2INCH(value);
        }

        protected override double MarginRightCore
        {
            get => Utils.INCH2CM(this.PrintSettings.MarginRight);
            set => this.PrintSettings.MarginRight = Utils.CM2INCH(value);
        }

        protected override double MarginTopCore
        {
            get => Utils.INCH2CM(this.PrintSettings.MarginTop);
            set => this.PrintSettings.MarginTop = Utils.CM2INCH(value);
        }

        #endregion

        #region PAGE SIZE

        protected override double PageWidthCore
        { 
            get => Utils.INCH2CM(this.PrintSettings.PageWidth);
            set => this.PrintSettings.PageWidth = Utils.CM2INCH(value);
        }

        protected override double PageHeightCore
        {
            get => Utils.INCH2CM(this.PrintSettings.PageHeight);
            set => this.PrintSettings!.PageHeight = Utils.CM2INCH(value);
        }

        #endregion


        protected override double ScaleFactorCore
        { 
            get => this.PrintSettings.ScaleFactor;
            set => this.PrintSettings.ScaleFactor = value;
        }

        protected override bool PrintBackgroundsCore
        { 
            get => this.PrintSettings.ShouldPrintBackgrounds;
            set => this.PrintSettings!.ShouldPrintBackgrounds = value;
        }

        protected override bool PrintSelectionOnlyCore
        {
            get => this.PrintSettings.ShouldPrintSelectionOnly;
            set => this.PrintSettings.ShouldPrintSelectionOnly = value;
        }

        protected override bool PrintHeaderAndFooterCore
        {
            get => this.PrintSettings.ShouldPrintHeaderAndFooter;
            set => this.PrintSettings.ShouldPrintHeaderAndFooter = value;
        }

        protected override string FooterUriCore
        {
            get => this.PrintSettings.FooterUri;
            set => this.PrintSettings.FooterUri = value;
        }

        protected override string PageRangesCore
        {
            get => this.PrintSettings.PageRanges;
            set => this.PrintSettings.PageRanges = value;
        }

        protected override int CopiesCore
        {
            get => this.PrintSettings!.Copies;
            set => this.PrintSettings!.Copies = value;
        }

        protected override int PagesPerSideCore
        {
            get => this.PrintSettings.PagesPerSide;
            set => this.PrintSettings.PagesPerSide = value;
        }

        protected override string PrinterNameCore
        {
            get => this.PrintSettings.PrinterName;
            set => this.PrintSettings.PrinterName = value;
        }

        protected override PrintDuplex DuplexCore
        {
            get => (PrintDuplex)this.PrintSettings.Duplex;
            set => this.PrintSettings.Duplex = (CoreWebView2PrintDuplex)value;
        }

        protected override string DuplesTextCore
        { 
            get => this.DuplexCore.ToString();
            set
            {
                if (Enum.TryParse(value, out PrintDuplex Duplex))
                    this.DuplexCore = Duplex;
            }
        }

        protected override PrintColorMode ColorModeCore
        {
            get => (PrintColorMode)this.PrintSettings.ColorMode;
            set => this.PrintSettings.ColorMode = (CoreWebView2PrintColorMode)value;
        }

        protected override string ColorModeTextCore
        {
            get => this.ColorModeCore.ToString();
            set
            {
                if (Enum.TryParse(value, out PrintColorMode color))
                    this.ColorModeCore = color;
            }
        }

        protected override PrintCollation CollationCore
        {
            get => (PrintCollation)this.PrintSettings.Collation;
            set => this.PrintSettings.Collation = (CoreWebView2PrintCollation)value;
        }

        protected override string CollationTextCore
        {
            get => this.CollationCore.ToString();
            set
            {
                if (Enum.TryParse(value, out PrintCollation collation))
                    this.CollationCore = collation;
            }
        }

        #endregion

        //=======================================//

        #region Methods

        protected override void PrintCore()
        {
            if (_isBusy || WVController == null)
                return;

            _isBusy = true;

            Task.Run(async () =>
            {
                if (WVController == null)
                {
                    _isBusy = false;
                    return;
                }

                var result = await WVController.CoreWebView2.PrintAsync(this.PrintSettings);
                _isBusy = false;
                this.FirePrintFinishedEvent((PrintStatus)result);
            });
        }

        protected override void PrintToPDFCore(string ResultFilePath)
        {
            if (this.IsBusy || WVController == null)
                return;

            _isBusy = true;

            Task.Run(async () =>
            {
                if (WVController == null)
                {
                    _isBusy = false;
                    return;
                }

                bool result = await WVController.CoreWebView2.PrintToPdfAsync(ResultFilePath ,this.PrintSettings);
                PrintStatus status = result ? PrintStatus.Succeeded : PrintStatus.OtherError;
                _isBusy = false;
                this.FirePrintFinishedEvent(status);
            });
        }

        #endregion


        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _printSettings = null;
        }

        //=======================================//

        #region Internal Methods

        internal void ToDefault()
        {
            this.ClearListeners();
            this.ClearEvents();
            _printSettings = CreatePrintSettings();
            _isBusy = false;
        }

        #endregion

        //=======================================//

        #region Private Methods

        private CoreWebView2PrintSettings? CreatePrintSettings()
        {
            var pm = WVController?.CoreWebView2.Environment.CreatePrintSettings();

            if(pm is null)
                return null;

            pm.PageWidth = Utils.CM2INCH(21);
            pm.PageHeight = Utils.CM2INCH(29.7);
            return pm;
        }

        #endregion

    }
}