namespace WV.Enums
{
    public enum PrintDuplex
    {
        /// <summary>
        /// The default duplex for a printer.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Print on only one side of the sheet.
        /// </summary>
        OneSided = 1,

        /// <summary>
        /// Print on both sides of the sheet, flipped along the long edge.
        /// </summary>
        TwoSidedLongEdge = 2,

        /// <summary>
        /// Print on both sides of the sheet, flipped along the short edge.
        /// </summary>
        TwoSidedShortEdge = 3
    }
}