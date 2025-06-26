namespace WV.Interfaces
{
    public interface IContextMenuItem : IDisposable
    {
        #region PROPS

        string Name { get; }

        /// <summary>
        /// "Command", "CheckBox", "Radio", "Submenu"
        /// </summary>
        string kind { get; }

        string Icon { get; }

        IContextMenuItem? Parent { get; }

        IContextMenuItem[] Children { get; }

        bool Checked { get; set; }

        bool Enabled { get; set; }

        bool Visible { get; set; }

        object? Callback { get; set; }

        #endregion

        #region METHODS

        void AddItem(IContextMenuItem item);

        void InsertItem(int index, IContextMenuItem item);

        void RemoveItem(IContextMenuItem item);

        void RemoveItemAt(int index);

        void Clear();

        #endregion

    }
}