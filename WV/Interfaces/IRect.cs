namespace WV.Interfaces
{
    public interface IRect
    {
        /// <summary>
        /// Window position in X
        /// </summary>
        int X { get; set; }

        /// <summary>
        /// Window position in Y
        /// </summary>
        int Y { get; set; }

        /// <summary>
        /// Window width
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Window Height
        /// </summary>
        int Height { get; set; }

        /// <summary>
        /// Maximum window width
        /// </summary>
        int MaxWidth { get; set; }

        /// <summary>
        /// Maximum window height
        /// </summary>
        int MaxHeight { get; set; }

        /// <summary>
        /// Minimun window width
        /// </summary>
        int MinWidth { get; set; }

        /// <summary>
        /// Minimun window height
        /// </summary>
        int MinHeight { get; set; }

        /// <summary>
        /// Set size of window
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void SetSize(int width, int height);

        /// <summary>
        /// Get size of window
        /// </summary>
        /// <returns></returns>
        int[] GetSize();

        /// <summary>
        /// Set position of window
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void SetPosition(int x, int y);

        /// <summary>
        /// Get position of window
        /// </summary>
        /// <returns></returns>
        int[] GetPosition();

        /// <summary>
        /// Set position and size of window
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void SetPositionAndSize(int x, int y, int width, int height);

        /// <summary>
        /// Get position and size of window
        /// </summary>
        /// <returns></returns>
        int[] GetPositionAndSize();
    }
}