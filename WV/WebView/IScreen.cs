namespace WV.WebView
{
    public interface IScreen
    {
        /// <summary>
        /// Gets a value indicating whether a particular display is the primary device.
        /// <para>
        /// true if this display is primary; otherwise, false.
        /// </para>
        /// </summary>
        bool Primary { get; }

        /// <summary>
        /// Gets the device name associated with a display.
        /// <para>
        /// The device name associated with a display.
        /// </para>
        /// </summary>
        string DeviceName { get; }

        /// <summary>
        /// Gets the number of bits of memory, associated with one pixel of data.
        /// <para>
        /// The number of bits of memory, associated with one pixel of data.
        /// </para>
        /// </summary>
        int BitsPerPixel { get; }

        /// <summary>
        /// Gets the working area of the display. The working area is the desktop area of 
        /// the display, excluding taskbars, docked windows, and docked tool bars.
        /// </summary>
        IArea WorkingArea { get; }

        /// <summary>
        /// Gets the bounds of the display.
        /// </summary>
        IArea Bounds { get; }
    }
}
