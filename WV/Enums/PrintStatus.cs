namespace WV.Enums
{
    public enum PrintStatus
    {
        /// <summary>
        /// Indicates that the print operation is succeeded.
        /// </summary>
        Succeeded = 0,

        /// <summary>
        /// Indicates that the printer is not available.
        /// </summary>
        PrinterUnavailable = 1,

        /// <summary>
        /// Indicates that the print operation is failed.
        /// </summary>
        OtherError = 2
    }
}