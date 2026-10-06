using WV.Configs;
using WV.Enums;
using WV.Interfaces;
using static WV.App.Delegates;

namespace WV.Core
{
    public abstract class PrintManagerCore : Plugin, IPrintManager
    {
        #region Events

        public event WVEventHandler<PrintStatus, string>? PrintFinished;

        #endregion

        protected PrintManagerCore(IPluginContext context, PrintManagerConfig printManagerConfig) : base(context)
        {
            var txt = $"{nameof(PrintManagerCore)} constructor";
            LogCalling(txt);
            try
            {
                
                Initialize(context, printManagerConfig);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void Initialize(IPluginContext context, PrintManagerConfig printManagerConfig);

        public bool IsBusy
        {
            get
            {
                var txt = nameof(IsBusy);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return IsBusyCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool IsBusyCore { get; }

        public PrintOrientation Orientation
        {
            get
            {
                var txt = nameof(Orientation);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return OrientationCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Orientation)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    OrientationCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract PrintOrientation OrientationCore { get; set; }

        public string OrientationText
        {
            get
            {
                var txt = nameof(OrientationText);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return OrientationTextCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(OrientationText)} = \"{value}\"";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    OrientationTextCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract string OrientationTextCore { get; set; }

        public double MarginBottom
        {
            get
            {
                var txt = nameof(MarginBottom);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return MarginBottomCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(MarginBottom)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    MarginBottomCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract double MarginBottomCore { get; set; }

        public double MarginLeft
        {
            get
            {
                var txt = nameof(MarginLeft);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return MarginLeftCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(MarginLeft)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    MarginLeftCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract double MarginLeftCore { get; set; }

        public double MarginRight
        {
            get
            {
                var txt = nameof(MarginRight);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return MarginRightCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(MarginRight)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    MarginRightCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract double MarginRightCore { get; set; }

        public double MarginTop
        {
            get
            {
                var txt = nameof(MarginTop);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return MarginTopCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(MarginTop)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    MarginTopCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract double MarginTopCore { get; set; }

        public double PageWidth
        {
            get
            {
                var txt = nameof(PageWidth);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return PageWidthCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(PageWidth)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    PageWidthCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract double PageWidthCore { get; set; }

        public double PageHeight
        {
            get
            {
                var txt = nameof(PageHeight);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return PageHeightCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(PageHeight)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    PageHeightCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract double PageHeightCore { get; set; }

        public double ScaleFactor
        {
            get
            {
                var txt = nameof(ScaleFactor);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return ScaleFactorCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(ScaleFactor)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    ScaleFactorCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract double ScaleFactorCore { get; set; }

        public bool PrintBackgrounds
        {
            get
            {
                var txt = nameof(PrintBackgrounds);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return PrintBackgroundsCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(PrintBackgrounds)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    PrintBackgroundsCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool PrintBackgroundsCore { get; set; }

        public bool PrintSelectionOnly
        {
            get
            {
                var txt = nameof(PrintSelectionOnly);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return PrintSelectionOnlyCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(PrintSelectionOnly)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    PrintSelectionOnlyCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool PrintSelectionOnlyCore { get; set; }

        public bool PrintHeaderAndFooter
        {
            get
            {
                var txt = nameof(PrintHeaderAndFooter);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return PrintHeaderAndFooterCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(PrintHeaderAndFooter)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    PrintHeaderAndFooterCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool PrintHeaderAndFooterCore { get; set; }

        public string FooterUri
        {
            get
            {
                var txt = nameof(FooterUri);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return FooterUriCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(FooterUri)} = \"{value}\"";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    FooterUriCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract string FooterUriCore { get; set; }

        public string PageRanges
        {
            get
            {
                var txt = nameof(PageRanges);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return PageRangesCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(PageRanges)} = \"{value}\"";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    PageRangesCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract string PageRangesCore { get; set; }

        public int Copies
        {
            get
            {
                var txt = nameof(Copies);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CopiesCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Copies)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    CopiesCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract int CopiesCore { get; set; }

        public int PagesPerSide
        {
            get
            {
                var txt = nameof(PagesPerSide);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return PagesPerSideCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(PagesPerSide)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    PagesPerSideCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract int PagesPerSideCore { get; set; }

        public string PrinterName
        {
            get
            {
                var txt = nameof(PrinterName);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return PrinterNameCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(PrinterName)} = \"{value}\"";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    PrinterNameCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract string PrinterNameCore { get; set; }

        public PrintDuplex Duplex
        {
            get
            {
                var txt = nameof(Duplex);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return DuplexCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Duplex)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    DuplexCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract PrintDuplex DuplexCore { get; set; }

        public string DuplesText
        {
            get
            {
                var txt = nameof(DuplesText);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return DuplesTextCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(DuplesText)} = \"{value}\"";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    DuplesTextCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }

        }

        protected abstract string DuplesTextCore { get; set; }

        public PrintColorMode ColorMode
        {
            get
            {
                var txt = nameof(ColorMode);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return ColorModeCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(ColorMode)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    ColorModeCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract PrintColorMode ColorModeCore { get; set; }

        public string ColorModeText
        {
            get
            {
                var txt = nameof(ColorModeText);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return ColorModeTextCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(ColorModeText)} = \"{value}\"";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    ColorModeTextCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }

        }

        protected abstract string ColorModeTextCore { get; set; }

        public PrintCollation Collation
        {
            get
            {
                var txt = nameof(Collation);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CollationCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Collation)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    CollationCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract PrintCollation CollationCore { get; set; }

        public string CollationText
        {
            get
            {
                var txt = nameof(CollationText);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CollationTextCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(CollationText)} = \"{value}\"";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    CollationTextCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }

        }

        protected abstract string CollationTextCore { get; set; }

        #region Methods

        public void Print()
        {
            var txt = $"{nameof(Print)}()";
            LogCalling(txt);
            try
            {
                ThrowIfDisposed();
                PrintCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void PrintCore();

        public void PrintToPDF(string ResultFilePath)
        {
            var txt = $"{nameof(PrintToPDF)}(\"{ResultFilePath}\")";
            LogCalling(txt);
            try
            {
                ThrowIfDisposed();
                PrintToPDFCore(ResultFilePath);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void PrintToPDFCore(string ResultFilePath);

        #endregion

        protected void FirePrintFinishedEvent(PrintStatus status)
        {
            if (this.Disposed)
                return;

            this.PrintFinished?.Invoke(WebView, status, status.ToString());
        }

        #region Helpers

        private void LogCalling(string txt, bool isProp = false)
        {
            Logger.Info(Utils.CallingMsg(txt, isProp));
        }

        private void LogFail(string txt, Exception ex, bool isProp = false)
        {
            Logger.Error(Utils.FailMsg(txt, isProp), ex);
        }

        #endregion
    }
}