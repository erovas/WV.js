using WV.Enums;

namespace WV.Interfaces
{
    public interface IContextMenu
    {
        #region PROPS

        /// <summary>
        /// Enable or disable context menu.
        /// </summary>
        bool Enable {  get; set; }

        /// <summary>
        /// Gets all cotext menu items.
        /// </summary>
        IContextMenuItem[] Children { get; }

        #endregion

        #region PROPS Native Items

        /// <summary>
        /// Enable or disable native items.
        /// </summary>
        bool ShowNativeItems { get; set; }

        bool EmojiItem { get; set; }

        bool UndoItem { get; set; }

        bool RedoItem { get; set; }

        bool CutItem { get; set; }

        bool CopyItem { get; set; }

        bool PasteItem { get; set; }

        bool PasteAndMatchStyleItem { get; set; }

        bool SelectAllItem { get; set; }

        bool WritingDirectionItem { get; set; }

        bool ShareItem { get; set; }

        bool WebCaptureItem { get; set; }

        bool LoopItem { get; set; }

        bool ShowAllControlsItem { get; set; }

        /// <summary>
        /// Save media as... (audio/video)
        /// </summary>
        bool SaveMediaAsItem { get; set; }


        bool CopyLinkItem { get; set; }


        bool CopyLinkToHighlightItem { get; set; }

        bool PrintItem { get; set; }

        bool BackItem { get; set; }

        bool ForwardItem { get; set; }

        bool ReloadItem { get; set; }

        bool SaveAsItem { get; set; }

        bool SaveImageAsItem { get; set; }

        bool CopyImageItem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool CopyImageLocationItem { get; set; }

        bool MagnifyImageItem { get; set; }

        bool SaveFrameAsItem { get; set; }

        bool CopyVideoFrameItem { get; set; }

        bool PictureInPictureItem { get; set; }

        bool SaveLinkAsItem { get; set; }

        bool OpenLinkInNewWindowItem { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Create an IContextMenuItem instance.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="kind">"Command", "CheckBox", "Radio", "Separator", "Submenu"</param>
        /// <param name="icon">Absolute o relative path to icon</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        IContextMenuItem CreateContextItem(string label, string kind, string? icon = null, object? callback = null);

        /// <summary>
        /// Create an IContextMenuItem instance as separator.
        /// </summary>
        /// <returns></returns>
        IContextMenuItem CreateContextItemSeparator();

        /// <summary>
        /// Add the item at the end.
        /// </summary>
        /// <param name="item"></param>
        void AddItem(IContextMenuItem item);

        /// <summary>
        /// Insert the item at the specified position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        void InsertItem(int index, IContextMenuItem item);

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item"></param>
        bool RemoveItem(IContextMenuItem item);

        /// <summary>
        /// Removes the item to the specified position.
        /// </summary>
        /// <param name="index"></param>
        void RemoveItemAt(int index);

        /// <summary>
        /// Remove all items.
        /// </summary>
        void Clear();

        #endregion

    }
}