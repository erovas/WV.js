using WV.Interfaces;

namespace WV.Core.Browsering
{
    public abstract class ContextMenuCore : Plugin, IContextMenu
    {
        #region Fields

        protected List<IContextMenuItem>? _children {  get; private set; }

        #endregion

        protected ContextMenuCore(IPluginContext context) : base(context)
        {
            var txt = $"{nameof(ContextMenuCore)} constructor";
            LogCalling(txt);
            try
            {
                _children = new List<IContextMenuItem>();
                Initialize(context);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void Initialize(IPluginContext context);

        #region Properties

        public bool Enable
        {
            get
            {
                var txt = nameof(Enable);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return EnableCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Enable)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    EnableCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool EnableCore { get; set; }

        public IContextMenuItem[] Children
        {
            get
            {
                var txt = nameof(Children);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return _children!.ToArray();
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        #endregion

        #region Properties Native Items

        public bool ShowNativeItems
        {
            get
            {
                var txt = nameof(ShowNativeItems);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return ShowNativeItemsCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(ShowNativeItems)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    ShowNativeItemsCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool ShowNativeItemsCore { get; set; }

        public bool EmojiItem
        {
            get
            {
                var txt = nameof(EmojiItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return EmojiItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(EmojiItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    EmojiItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool EmojiItemCore { get; set; }

        public bool UndoItem
        {
            get
            {
                var txt = nameof(UndoItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return UndoItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(UndoItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    UndoItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool UndoItemCore { get; set; }

        public bool RedoItem
        {
            get
            {
                var txt = nameof(RedoItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return RedoItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(RedoItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    RedoItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool RedoItemCore { get; set; }

        public bool CutItem
        {
            get
            {
                var txt = nameof(CutItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CutItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(CutItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    CutItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool CutItemCore { get; set; }

        public bool CopyItem
        {
            get
            {
                var txt = nameof(CopyItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CopyItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(CopyItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    CopyItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool CopyItemCore { get; set; }

        public bool PasteItem
        {
            get
            {
                var txt = nameof(PasteItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return PasteItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(PasteItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    PasteItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool PasteItemCore { get; set; }

        public bool PasteAndMatchStyleItem
        {
            get
            {
                var txt = nameof(PasteAndMatchStyleItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return PasteAndMatchStyleItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(PasteAndMatchStyleItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    PasteAndMatchStyleItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool PasteAndMatchStyleItemCore { get; set; }

        public bool SelectAllItem
        {
            get
            {
                var txt = nameof(SelectAllItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return SelectAllItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(SelectAllItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    SelectAllItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool SelectAllItemCore { get; set; }

        public bool WritingDirectionItem
        {
            get
            {
                var txt = nameof(WritingDirectionItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return WritingDirectionItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(WritingDirectionItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    WritingDirectionItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool WritingDirectionItemCore { get; set; }

        public bool ShareItem
        {
            get
            {
                var txt = nameof(ShareItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return ShareItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(ShareItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    ShareItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool ShareItemCore { get; set; }

        public bool WebCaptureItem
        {
            get
            {
                var txt = nameof(WebCaptureItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return WebCaptureItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(WebCaptureItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    WebCaptureItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool WebCaptureItemCore { get; set; }

        public bool LoopItem
        {
            get
            {
                var txt = nameof(LoopItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return LoopItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(LoopItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    LoopItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool LoopItemCore { get; set; }

        public bool ShowAllControlsItem
        {
            get
            {
                var txt = nameof(ShowAllControlsItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return ShowAllControlsItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(ShowAllControlsItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    ShowAllControlsItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool ShowAllControlsItemCore { get; set; }

        public bool SaveMediaAsItem
        {
            get
            {
                var txt = nameof(SaveMediaAsItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return SaveMediaAsItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(SaveMediaAsItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    SaveMediaAsItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool SaveMediaAsItemCore { get; set; }

        public bool CopyLinkItem
        {
            get
            {
                var txt = nameof(CopyLinkItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CopyLinkItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(CopyLinkItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    CopyLinkItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool CopyLinkItemCore { get; set; }

        public bool CopyLinkToHighlightItem
        {
            get
            {
                var txt = nameof(CopyLinkToHighlightItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CopyLinkToHighlightItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(CopyLinkToHighlightItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    CopyLinkToHighlightItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool CopyLinkToHighlightItemCore { get; set; }

        public bool PrintItem
        {
            get
            {
                var txt = nameof(PrintItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return PrintItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(PrintItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    PrintItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool PrintItemCore { get; set; }

        public bool BackItem
        {
            get
            {
                var txt = nameof(BackItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return BackItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(BackItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    BackItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool BackItemCore { get; set; }

        public bool ForwardItem
        {
            get
            {
                var txt = nameof(ForwardItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return ForwardItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(ForwardItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    ForwardItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool ForwardItemCore { get; set; }

        public bool ReloadItem
        {
            get
            {
                var txt = nameof(ReloadItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return ReloadItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(ReloadItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    ReloadItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool ReloadItemCore { get; set; }

        public bool SaveAsItem
        {
            get
            {
                var txt = nameof(SaveAsItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return SaveAsItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(SaveAsItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    SaveAsItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool SaveAsItemCore { get; set; }

        public bool SaveImageAsItem
        {
            get
            {
                var txt = nameof(SaveImageAsItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return SaveImageAsItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(SaveImageAsItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    SaveImageAsItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool SaveImageAsItemCore { get; set; }

        public bool CopyImageItem
        {
            get
            {
                var txt = nameof(CopyImageItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CopyImageItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(CopyImageItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    CopyImageItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool CopyImageItemCore { get; set; }

        public bool CopyImageLocationItem
        {
            get
            {
                var txt = nameof(CopyImageLocationItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CopyImageLocationItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(CopyImageLocationItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    CopyImageLocationItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool CopyImageLocationItemCore { get; set; }

        public bool MagnifyImageItem
        {
            get
            {
                var txt = nameof(MagnifyImageItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return MagnifyImageItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(MagnifyImageItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    MagnifyImageItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool MagnifyImageItemCore { get; set; }

        public bool SaveFrameAsItem
        {
            get
            {
                var txt = nameof(SaveFrameAsItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return SaveFrameAsItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(SaveFrameAsItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    SaveFrameAsItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool SaveFrameAsItemCore { get; set; }

        public bool CopyVideoFrameItem
        {
            get
            {
                var txt = nameof(CopyVideoFrameItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CopyVideoFrameItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(CopyVideoFrameItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    CopyVideoFrameItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool CopyVideoFrameItemCore { get; set; }

        public bool PictureInPictureItem
        {
            get
            {
                var txt = nameof(PictureInPictureItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return PictureInPictureItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(PictureInPictureItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    PictureInPictureItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool PictureInPictureItemCore { get; set; }

        public bool SaveLinkAsItem
        {
            get
            {
                var txt = nameof(SaveLinkAsItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return SaveLinkAsItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(SaveLinkAsItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    SaveLinkAsItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool SaveLinkAsItemCore { get; set; }

        public bool OpenLinkInNewWindowItem
        {
            get
            {
                var txt = nameof(OpenLinkInNewWindowItem);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return OpenLinkInNewWindowItemCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(OpenLinkInNewWindowItem)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    OpenLinkInNewWindowItemCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool OpenLinkInNewWindowItemCore { get; set; }

        #endregion

        #region Methods

        public void AddItem(IContextMenuItem item)
        {
            var txt = $"{nameof(AddItem)}({item})";
            LogCalling(txt);
            try
            {
                AddItemCore(item);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void AddItemCore(IContextMenuItem item);

        public void Clear()
        {
            var txt = $"{nameof(Clear)}()";
            LogCalling(txt);
            try
            {
                ClearCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void ClearCore();

        public IContextMenuItem CreateContextItem(string label, string kind, string? icon = null, object? callback = null)
        {
            var txt = $"{nameof(CreateContextItem)}(\"{label}\", \"{kind}\", \"{icon}\", {callback})";
            LogCalling(txt);
            try
            {
                return CreateContextItemCore(label, kind, icon, callback);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract IContextMenuItem CreateContextItemCore(string label, string kind, string? icon = null, object? callback = null);

        public IContextMenuItem CreateContextItemSeparator()
        {
            var txt = $"{nameof(CreateContextItemSeparator)}()";
            LogCalling(txt);
            try
            {
                return CreateContextItemSeparatorCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract IContextMenuItem CreateContextItemSeparatorCore();

        public void InsertItem(int index, IContextMenuItem item)
        {
            var txt = $"{nameof(InsertItem)}({index}, {item})";
            LogCalling(txt);
            try
            {
                InsertItemCore(index, item);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void InsertItemCore(int index, IContextMenuItem item);

        public bool RemoveItem(IContextMenuItem item)
        {
            var txt = $"{nameof(RemoveItem)}({item})";
            LogCalling(txt);
            try
            {
                return RemoveItemCore(item);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract bool RemoveItemCore(IContextMenuItem item);

        public void RemoveItemAt(int index)
        {
            var txt = $"{nameof(RemoveItemAt)}({index})";
            LogCalling(txt);
            try
            {
                RemoveItemAtCore(index);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void RemoveItemAtCore(int index);

        #endregion

        protected sealed override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var item in _children!)
                    item.Dispose();

                _children.Clear();
                _children = null;
            }
            DisposeCore(disposing);
        }

        protected abstract void DisposeCore(bool disposing);


        #region Helpers

        private void LogCalling(string txt, bool isProp = false)
        {
            Logger.Info(Utils.CallingMsg(txt, isProp));
        }

        private void LogFail(string txt, Exception ex, bool isProp = false)
        {
            Logger.Error(Utils.FailMsg(txt, isProp), ex);
        }

        #endregion
    }
}