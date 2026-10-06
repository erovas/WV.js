using System.Text.Json.Serialization;
using WV.Enums;

namespace WV.Configs
{
    public sealed class PrintManagerConfig
    {
        #region Properties

        public double MarginBottom { get; }

        public double MarginLeft { get; }

        public double MarginRight { get; }

        public double MarginTop { get; }

        public double PageWidth { get; }

        public double PageHeight { get; }

        public double ScaleFactor { get; }

        public bool PrintBackgrounds { get; }

        public bool PrintSelectionOnly { get; }

        public bool PrintHeaderAndFooter { get; }

        public string? FooterUri { get; }

        public int PagesPerSide { get; }

        public PrintColorMode ColorMode { get; }

        public PrintCollation Collation { get; }

        public PrintOrientation Orientation { get; }

        #endregion

        [JsonConstructor]
        public PrintManagerConfig(
            double marginBottom = App.PrintManager.MarginBottom, double marginLeft = App.PrintManager.MarginLeft, 
            double marginRight = App.PrintManager.MarginRight, double marginTop = App.PrintManager.MarginTop,
            double pageWidth = App.PrintManager.PageWidth, double pageHeight = App.PrintManager.PageHeight, 
            double scaleFactor = App.PrintManager.ScaleFactor, bool printBackgrounds = App.PrintManager.PrintBackgrounds,
            bool printSelectionOnly = App.PrintManager.PrintSelectionOnly, bool printHeaderAndFooter = App.PrintManager.PrintHeaderAndFooter, 
            string? footerUri = App.PrintManager.FooterUri, int pagesPerSide = App.PrintManager.PagesPerSide,
            PrintColorMode colorMode = App.PrintManager.ColorMode, PrintCollation collation = App.PrintManager.Collation, 
            PrintOrientation orientation = App.PrintManager.Orientation
            )
        {
            PageHeight = Utils.Clamp(pageHeight, App.PrintManager.MinPageHeight, App.PrintManager.MaxPageHeight);
            PageWidth = Utils.Clamp(pageWidth, App.PrintManager.MinPageWidth, App.PrintManager.MaxPageWidth);

            MarginTop = Utils.Clamp(marginTop, 0, PageHeight);
            MarginBottom = Utils.Clamp(marginBottom, 0, PageHeight);

            MarginLeft = Utils.Clamp(marginLeft, 0, PageWidth);
            MarginRight = Utils.Clamp(marginRight, 0, PageWidth);

            ScaleFactor = Utils.Clamp(scaleFactor, App.PrintManager.MinScaleFactor, App.PrintManager.MaxScaleFactor);
            PrintBackgrounds = printBackgrounds;
            PrintSelectionOnly = printSelectionOnly;
            PrintHeaderAndFooter = printHeaderAndFooter;
            FooterUri = footerUri;
            PagesPerSide = SnapToAllowed(pagesPerSide);
            ColorMode = colorMode;
            Collation = collation;
            Orientation = orientation;
        }

        public static int SnapToAllowed(int value)
        {
            foreach (var step in App.PrintManager.PagesPerSideValues)
                if (value <= step)
                    return step;
            
            return App.PrintManager.PagesPerSideValues[^1]; // 16 para cualquier valor mayor
        }
    }
}