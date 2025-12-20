namespace WV.Interfaces
{
    public interface IContextMenuItem : IDisposable
    {
        #region PROPS

        /// <summary>
        /// Text to show.
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// "Command", "CheckBox", "Radio", "Submenu"
        /// </summary>
        string Kind { get; set; }

        /// <summary>
        /// Absolute or relative path to icon.
        /// </summary>
        string Icon { get; set; }

        /// <summary>
        /// It is different from null if the item belongs to a submenu.
        /// </summary>
        IContextMenuItem? Parent { get; }

        /// <summary>
        /// Gets the items contained in this item as static array.
        /// </summary>
        IContextMenuItem[] Children { get; }

        /// <summary>
        /// Gets or sets checked.
        /// </summary>
        bool Checked { get; set; }

        /// <summary>
        /// Gets or sets enable.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets visibility.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets callback function.
        /// </summary>
        object? Callback { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Add the item to the end of the submenu.
        /// </summary>
        /// <param name="item"></param>
        void AddItem(IContextMenuItem item);

        /// <summary>
        /// Insert the item at the specified position of the submenu.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        void InsertItem(int index, IContextMenuItem item);

        /// <summary>
        /// Removes the item of the submenu. Return true if removed.
        /// </summary>
        /// <param name="item"></param>
        void RemoveItem(IContextMenuItem item);

        /// <summary>
        /// Removes the item to the specified position of the submenu.
        /// </summary>
        /// <param name="index"></param>
        void RemoveItemAt(int index);

        /// <summary>
        /// Remove all items of the submenu.
        /// </summary>
        void Clear();

        #endregion

    }
}