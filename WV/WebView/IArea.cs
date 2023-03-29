namespace WV.WebView
{
    public interface IArea
    {
        /// <summary>
        /// Gets the y-coordinate of the top edge of Area.
        /// </summary>
        int Top { get; }

        /// <summary>
        /// Gets the y-coordinate that is the sum of the Top and Height property values.
        /// </summary>
        int Bottom { get; }

        /// <summary>
        /// Gets the x-coordinate of the left edge of screen
        /// </summary>
        int Left { get; }

        /// <summary>
        /// Gets the x-coordinate that is the sum of Left and Width property values.
        /// </summary>
        int Right { get; }

        /// <summary>
        /// Gets the width of the area.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the area.
        /// </summary>
        int Height { get; }
    }
}
