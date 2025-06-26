using WV.Enums;

namespace WV.Interfaces
{
    public interface IContextMenu
    {
        #region PROPS

        /// <summary>
        /// Enable or disable context menu
        /// </summary>
        bool Enable {  get; set; }

        IContextMenuItem[] Children { get; }

        #endregion

        #region PROPS Native Items

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

        IContextMenuItem CreateContextItem(string name, string kind, string? icon = null, object? callback = null);

        IContextMenuItem CreateContextItemSeparator();

        void AddItem(IContextMenuItem item);

        void InsertItem(int index, IContextMenuItem item);

        void RemoveItem(IContextMenuItem item);

        void RemoveItemAt(int index);

        void Clear();

        #endregion

    }
}