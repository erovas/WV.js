using WV.Enums;
using static WV.AppManager;

namespace WV.Interfaces
{
    public interface IPrintManager
    {
        event WVEventHandler<PrintStatus, string> PrintFinished;

        #region PROPS

        /// <summary>
        /// Gets a value indicating that the printmanager is printing.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Gets or sets Print orientation.
        /// </summary>
        PrintOrientation Orientation { get; set; }

        /// <summary>
        /// Gets or sets Print orientation as text.
        /// </summary>
        string OrientationText { get; set; }

        /// <summary>
        /// Gets or sets the bottom margin in centimeters. The default is 1 cm.
        /// </summary>
        double MarginBottom { get; set; }

        /// <summary>
        /// Gets or sets the left margin in centimeters. The default is 1 cm.
        /// </summary>
        double MarginLeft { get; set; }

        /// <summary>
        /// Gets or sets the right margin in centimeters. The default is 1 cm.
        /// </summary>
        double MarginRight { get; set; }

        /// <summary>
        /// Gets or sets the top margin in centimeters. The default is 1 cm.
        /// </summary>
        double MarginTop { get; set; }

        /// <summary>
        /// Gets or sets the page width in centimeters. The default width is 21 cm.
        /// </summary>
        double PageWidth { get; set; }

        /// <summary>
        /// Gets or sets the page height in centimeters. The default height is 29.7 cm.
        /// </summary>
        double PageHeight { get; set; }

        /// <summary>
        /// Gets or sets the scale factor is a value between 0.1 and 2.0. The default is 1.
        /// </summary>
        double ScaleFactor { get; set; }

        /// <summary>
        /// True if background colors and images should be printed. The default value is false.
        /// </summary>
        bool PrintBackgrounds { get; set; }

        /// <summary>
        /// True if only the current end user's selection of HTML in the document should be printed.
        /// The default value is false.
        /// </summary>
        bool PrintSelectionOnly { get; set; }

        /// <summary>
        /// True if header and footer should be printed. The default value is false.
        /// <para>
        /// The header consists of the date and time of printing, and the title of the page. 
        /// The footer consists of the URI and page number. The height of the header and footer is 0.5 cm
        /// </para>
        /// </summary>
        bool PrintHeaderAndFooter { get; set; }

        /// <summary>
        /// Gets or sets the URI in the footer if PrintHeaderAndFooter is true. The default value is the current URI.
        /// <para>
        /// If an empty string or null value is provided, no URI is shown in the footer.
        /// </para>
        /// </summary>
        string FooterUri { get; set; }

        /// <summary>
        /// Gets or sets the page range to print. Defaults to empty string, which means print all pages.
        /// <para>
        /// The PageRanges property is a list of page ranges specifying one or more pages
        /// that should be printed separated by commas. Any whitespace between page ranges
        /// is ignored. A valid page range is either a single integer identifying the page
        /// to print, or a range in the form [start page]-[last page] where start page and
        /// last page are integers identifying the first and last inclusive pages respectively
        /// to print. Every page identifier is an integer greater than 0 unless wildcards
        /// are used (see below examples). The first page is 1. In a page range of the form
        /// [start page]-[last page] the start page number must be larger than 0 and less
        /// than or equal to the document's total page count. If the start page is not present,
        /// then 1 is used as the start page. The last page must be larger than the start
        /// page. If the last page is not present, then the document total page count is
        /// used as the last page. Repeating a page does not print it multiple times. To
        /// print multiple times, use the Microsoft.Web.WebView2.Core.CoreWebView2PrintSettings.Copies
        /// property. The pages are always printed in ascending order, even if specified
        /// in non-ascending order. If page range is not valid or if a page is greater than
        /// document total page count, ArgumentException is thrown. The following examples
        /// assume a document with 20 total pages.
        /// </para>
        /// <para>
        /// ExampleResultNotes <br/>
        /// "2"Page 2 <br/>
        /// "1-4, 9, 3-6, 10, 11"Pages 1-6, 9-11 <br/>
        /// "1-4, -6" Pages 1-6 The "-6" is interpreted as "1-6". <br/>
        /// "2-" Pages 2-20The "2-" is interpreted as "pages 2 to the end of the document". <br/>
        /// <br/>
        /// "4-2, 11, -6"Invalid"4-2" is an invalid range. <br/>
        /// "-"Pages 1-20The "-" is interpreted as "page 1 to the end of the document". <br/>
        /// "1-4dsf, 11"Invalid <br/>
        /// "2-2"Page 2 <br/>
        /// </para>
        /// </summary>
        string PageRanges { get; set; }

        /// <summary>
        /// Gets or sets number of copies to print. Minimum value is 1 and the maximum copies count is 999.
        /// The default value is 1.
        /// </summary>
        int Copies { get; set; }

        /// <summary>
        /// Gets or sets prints multiple pages of a document on a single piece of paper. Choose from 1, 2, 4, 6, 9 or 16.
        /// The default value is 1.
        /// </summary>
        int PagesPerSide { get; set; }

        /// <summary>
        /// The name of the printer to use. Defaults to empty string.
        /// <para>
        /// If the printer name is empty string or null, then it
        /// prints to the default printer on the user OS. If provided printer name doesn't
        /// match with the name of any installed printers on the user OS, the event returns
        /// with PrintStatus.PrinterUnavailable.
        /// method.
        /// </para>
        /// </summary>
        string PrinterName { get; set; }

        /// <summary>
        /// Printer duplex settings. The default value is PrintDuplex.Default.
        /// <para>
        /// Printing uses default value of printer's duplex if an invalid value is provided for the specific printer.
        /// </para>
        /// </summary>
        PrintDuplex Duplex { get; set; }

        /// <summary>
        /// Printer duplex settings. The default value is PrintDuplex.Default.
        /// <para>
        /// Printing uses default value of printer's duplex if an invalid value is provided for the specific printer.
        /// </para>
        /// </summary>
        string DuplesText { get; set; }

        /// <summary>
        /// Printer color mode. The default value is PrintColorMode.Default.
        /// <para>
        /// Printing uses default value of printer supported color if an invalid value is provided for the specific printer.
        /// </para>
        /// </summary>
        PrintColorMode ColorMode { get; set; }

        /// <summary>
        /// Printer color mode. The default value is PrintColorMode.Default.
        /// <para>
        /// Printing uses default value of printer supported color if an invalid value is provided for the specific printer.
        /// </para>
        /// </summary>
        string ColorModeText { get; set; }

        /// <summary>
        /// Printer collation. The default value is PrintCollation.Default.
        /// <para>
        /// Printing uses default value of printer's collation if an invalid value is provided for the specific printer.
        /// </para>
        /// </summary>
        PrintCollation Collation { get; set; }

        /// <summary>
        /// Printer collation. The default value is PrintCollation.Default.
        /// <para>
        /// Printing uses default value of printer's collation if an invalid value is provided for the specific printer.
        /// </para>
        /// </summary>
        string CollationText { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Print the current web page to the specified printer with the provided settings.
        /// </summary>
        void Print();

        /// <summary>
        /// Print the current page to PDF with the provided settings.
        /// </summary>
        /// <param name="ResultFilePath"></param>
        void PrintToPDF(string ResultFilePath);

        #endregion

        /// <summary>
        /// PrintFinished event.
        /// </summary>
        //object? OnPrintFinished { get; set; }

        /// <summary>
        /// Appends an event listener for events whose type attribute value is type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        //void AddEventListener(string type, object callback);

        /// <summary>
        /// Removes the event listener in target's event listener list with the same type and callback.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        //void RemoveEventListener(string type, object callback);
    }
}