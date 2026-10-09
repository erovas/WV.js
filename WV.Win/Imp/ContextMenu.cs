using Microsoft.Web.WebView2.Core;
using System.Reflection;
using WV.Core.Browsering;
using WV.Interfaces;

namespace WV.Win.Imp
{
    public class ContextMenu : ContextMenuCore, IContextMenu
    {
        private static readonly Dictionary<string, int> NativeItemCodes = new Dictionary<string, int>
        {
            {"Emoji", 50220},
            {"Undo", 50154},
            {"Redo", 50155},
            {"Cut", 50151},
            {"Copy", 50150},
            {"Paste", 50152},
            {"PasteAndMatchStyle", 50157},
            {"SelectAll", 50156},
            {"WritingDirection", 41120},
            {"Share", 50460},
            {"WebCapture", 52551},
            {"Loop", 50140},
            {"ShowAllControls", 50141},
            {"SaveMediaAs", 50131},
            {"CopyLink", 50132},
            {"CopyLinkToHighlight", 50158},
            {"Print", 35003},
            {"Back", 33000},
            {"Forward", 33001},
            {"Reload", 33002},
            {"SaveAs", 35004},
            {"SaveImageAs", 50120},
            {"CopyImage", 50122},
            {"CopyImageLocation", 50121},
            {"MagnifyImage", 52525},
            {"SaveFrameAs", 50130},
            {"CopyVideoFrame", 50133},
            {"PictureInPicture", 50137},
            {"SaveLinkAs", 50103},
            {"OpenLinkInNewWindow", 50101},
        };

        private class NatItem
        {
            public int Cod;
            public bool Visible = true;
        }

        //===============================================//

        //===============================================//

        #region Fields

        private Dictionary<string, IContextMenuItem>? _itemInstances;
        private Dictionary<string, NatItem>? _dicPropNativeItems;

        #endregion

        private Dictionary<string, NatItem> DicPropNativeItems => _dicPropNativeItems!;
        private Dictionary<string, IContextMenuItem> _ItemInstances => _itemInstances!;
        private string Directory => ((WebView)WebView).InternalBrowser.Directory;

        public ContextMenu(IPluginContext context, string directory) : base(context)
        {
        }

        protected override void Initialize(IPluginContext context)
        {
            _itemInstances = new Dictionary<string, IContextMenuItem>();
            _dicPropNativeItems = new Dictionary<string, NatItem>();

            // Obtener nombres de PROPS Native Items
            PropertyInfo[] props = this.GetType().GetProperties();
            foreach (PropertyInfo pi in props)
            {
                string name = pi.Name.Replace("Item", "");
                if (NativeItemCodes.TryGetValue(name, out int value))
                    _dicPropNativeItems[pi.Name] = new NatItem { Cod = value };
            }
        }

        #region Properties

        protected override bool EnableCore { get; set; } = true;

        #endregion

        #region Properties Native Items

        protected override bool ShowNativeItemsCore { get; set; } = true;

        protected override bool EmojiItemCore 
        { 
            get => this.DicPropNativeItems[nameof(EmojiItem)].Visible; 
            set => this.DicPropNativeItems[nameof(EmojiItem)].Visible = value;
        }

        protected override bool UndoItemCore
        {
            get => this.DicPropNativeItems[nameof(UndoItem)].Visible;
            set => this.DicPropNativeItems[nameof(UndoItem)].Visible = value;
        }

        protected override bool RedoItemCore
        {
            get => this.DicPropNativeItems[nameof(RedoItem)].Visible;
            set => this.DicPropNativeItems[nameof(RedoItem)].Visible = value;
        }

        protected override bool CutItemCore
        {
            get => this.DicPropNativeItems[nameof(CutItem)].Visible;
            set => this.DicPropNativeItems[nameof(CutItem)].Visible = value;
        }

        protected override bool CopyItemCore
        {
            get => this.DicPropNativeItems[nameof(CopyItem)].Visible;
            set => this.DicPropNativeItems[nameof(CopyItem)].Visible = value;
        }

        protected override bool PasteItemCore
        {
            get => this.DicPropNativeItems[nameof(PasteItem)].Visible;
            set => this.DicPropNativeItems[nameof(PasteItem)].Visible = value;
        }

        protected override bool PasteAndMatchStyleItemCore
        {
            get => this.DicPropNativeItems[nameof(PasteAndMatchStyleItem)].Visible;
            set => this.DicPropNativeItems[nameof(PasteAndMatchStyleItem)].Visible = value;
        }

        protected override bool SelectAllItemCore
        {
            get => this.DicPropNativeItems[nameof(SelectAllItem)].Visible;
            set => this.DicPropNativeItems[nameof(SelectAllItem)].Visible = value;
        }

        protected override bool WritingDirectionItemCore
        {
            get => this.DicPropNativeItems[nameof(WritingDirectionItem)].Visible;
            set => this.DicPropNativeItems[nameof(WritingDirectionItem)].Visible = value;
        }

        protected override bool ShareItemCore
        {
            get => this.DicPropNativeItems[nameof(ShareItem)].Visible;
            set => this.DicPropNativeItems[nameof(ShareItem)].Visible = value;
        }

        protected override bool WebCaptureItemCore
        {
            get => this.DicPropNativeItems[nameof(WebCaptureItem)].Visible;
            set => this.DicPropNativeItems[nameof(WebCaptureItem)].Visible = value;
        }

        protected override bool LoopItemCore
        {
            get => this.DicPropNativeItems[nameof(LoopItem)].Visible;
            set => this.DicPropNativeItems[nameof(LoopItem)].Visible = value;
        }

        protected override bool ShowAllControlsItemCore
        {
            get => this.DicPropNativeItems[nameof(ShowAllControlsItem)].Visible;
            set => this.DicPropNativeItems[nameof(ShowAllControlsItem)].Visible = value;
        }

        protected override bool SaveMediaAsItemCore
        {
            get => this.DicPropNativeItems[nameof(SaveMediaAsItem)].Visible;
            set => this.DicPropNativeItems[nameof(SaveMediaAsItem)].Visible = value;
        }

        protected override bool CopyLinkItemCore
        {
            get => this.DicPropNativeItems[nameof(CopyLinkItem)].Visible;
            set => this.DicPropNativeItems[nameof(CopyLinkItem)].Visible = value;
        }

        protected override bool CopyLinkToHighlightItemCore
        {
            get => this.DicPropNativeItems[nameof(CopyLinkToHighlightItem)].Visible;
            set => this.DicPropNativeItems[nameof(CopyLinkToHighlightItem)].Visible = value;
        }

        protected override bool PrintItemCore
        {
            get => this.DicPropNativeItems[nameof(PrintItem)].Visible;
            set => this.DicPropNativeItems[nameof(PrintItem)].Visible = value;
        }

        protected override bool BackItemCore
        {
            get => this.DicPropNativeItems[nameof(BackItem)].Visible;
            set => this.DicPropNativeItems[nameof(BackItem)].Visible = value;
        }

        protected override bool ForwardItemCore
        {
            get => this.DicPropNativeItems[nameof(ForwardItem)].Visible;
            set => this.DicPropNativeItems[nameof(ForwardItem)].Visible = value;
        }

        protected override bool ReloadItemCore
        {
            get => this.DicPropNativeItems[nameof(ReloadItem)].Visible;
            set => this.DicPropNativeItems[nameof(ReloadItem)].Visible = value;
        }

        protected override bool SaveAsItemCore
        {
            get => this.DicPropNativeItems[nameof(SaveAsItem)].Visible;
            set => this.DicPropNativeItems[nameof(SaveAsItem)].Visible = value;
        }

        protected override bool SaveImageAsItemCore
        {
            get => this.DicPropNativeItems[nameof(SaveImageAsItem)].Visible;
            set => this.DicPropNativeItems[nameof(SaveImageAsItem)].Visible = value;
        }

        protected override bool CopyImageItemCore
        {
            get => this.DicPropNativeItems[nameof(CopyImageItem)].Visible;
            set => this.DicPropNativeItems[nameof(CopyImageItem)].Visible = value;
        }

        protected override bool CopyImageLocationItemCore
        {
            get => this.DicPropNativeItems[nameof(CopyImageLocationItem)].Visible;
            set => this.DicPropNativeItems[nameof(CopyImageLocationItem)].Visible = value;
        }

        protected override bool MagnifyImageItemCore
        {
            get => this.DicPropNativeItems[nameof(MagnifyImageItem)].Visible;
            set => this.DicPropNativeItems[nameof(MagnifyImageItem)].Visible = value;
        }

        protected override bool SaveFrameAsItemCore
        {
            get => this.DicPropNativeItems[nameof(SaveFrameAsItem)].Visible;
            set => this.DicPropNativeItems[nameof(SaveFrameAsItem)].Visible = value;
        }

        protected override bool CopyVideoFrameItemCore
        {
            get => this.DicPropNativeItems[nameof(CopyVideoFrameItem)].Visible;
            set => this.DicPropNativeItems[nameof(CopyVideoFrameItem)].Visible = value;
        }

        protected override bool PictureInPictureItemCore
        {
            get => this.DicPropNativeItems[nameof(PictureInPictureItem)].Visible;
            set => this.DicPropNativeItems[nameof(PictureInPictureItem)].Visible = value;
        }

        protected override bool SaveLinkAsItemCore
        {
            get => this.DicPropNativeItems[nameof(SaveLinkAsItem)].Visible;
            set => this.DicPropNativeItems[nameof(SaveLinkAsItem)].Visible = value;
        }

        protected override bool OpenLinkInNewWindowItemCore
        {
            get => this.DicPropNativeItems[nameof(OpenLinkInNewWindowItem)].Visible;
            set => this.DicPropNativeItems[nameof(OpenLinkInNewWindowItem)].Visible = value;
        }


        #endregion

        #region Methods

        protected override IContextMenuItem CreateContextItemCore(string label, string kind, string? icon = null, object? callback = null)
        {
            var ctx = CreateCTX();
            var item = new ContextMenuItem(ctx, kind, label, icon ?? string.Empty, Directory){ Callback = callback };
            this._ItemInstances.Add(item.UID, item);
            return item;
        }

        protected override IContextMenuItem CreateContextItemSeparatorCore()
        {
            var ctx = CreateCTX();
            var item = new ContextMenuItem(ctx, CoreWebView2ContextMenuItemKind.Separator.ToString(), string.Empty, string.Empty, Directory);
            this._ItemInstances.Add(item.UID, item);
            return item;
        }

        protected override void AddItemCore(IContextMenuItem item)
        {
            this.AllowInsertItem(item);
            this._children.Add(item);
        }

        protected override void InsertItemCore(int index, IContextMenuItem item)
        {
            this.AllowInsertItem(item);
            this._children.Insert(index, item);
        }

        protected override bool RemoveItemCore(IContextMenuItem item)
        {
            return this._children.Remove(item);
        }

        protected override void RemoveItemAtCore(int index)
        {
            this._children.RemoveAt(index);
        }

        protected override void ClearCore()
        {            
            this._children.Clear();
        }

        #endregion

        protected override void DisposeCore(bool disposing)
        {
            if (!disposing)
                return;

            _dicPropNativeItems?.Clear();
            _dicPropNativeItems = null;

            foreach (var item in this._itemInstances!.Values)
                item.Dispose();

            _itemInstances.Clear();
            _itemInstances = null;
        }

        #region HELPERS

        private void AllowInsertItem(IContextMenuItem item)
        {
            if (item.Parent != null)
                throw new Exception("This item belongs to a submenu");

            if (this._children.Contains(item))
                throw new Exception("This item already exists");
        }

        internal void ContextMenuHandler(CoreWebView2 coreWV2, CoreWebView2ContextMenuRequestedEventArgs e)
        {
            if (!this.EnableCore)
            {
                e.MenuItems.Clear();
                e.Handled = true;
                return;
            }

            // Lista de ítems del menú contextual
            var menuItems = e.MenuItems.ToList();

            //Dejar vacio el menu original
            e.MenuItems.Clear();

            // Si se quiere mostrar los items nativos
            if (this.ShowNativeItemsCore)
            {
                // Quitar todos los separadores del menu original
                menuItems.RemoveAll(item => item.CommandId == -1);

                // Quitar los items nativos que el usuario NO quiere mostrar
                foreach (var item in this.DicPropNativeItems.Values)
                    menuItems.RemoveAll(x => x.CommandId == item.Cod && !item.Visible);

                // Inyectar los items nativos que se quieren mostrar
                foreach (var item in menuItems)
                    e.MenuItems.Add(item);
            }
                
            // Inyectar Custom Items
            foreach (var item in this._children)
                if(item.Visible)
                    e.MenuItems.Add(((ContextMenuItem)item)._Item);
        }

        internal void ToDefault()
        {
            ClearListeners();
            ClearEvents();

            foreach (var item in this.DicPropNativeItems)
                item.Value.Visible = true;

            this._children.Clear();
            
            foreach (var item in this._ItemInstances.Values)
                item.Dispose();

            this._ItemInstances.Clear();
        }

        private IPluginContext CreateCTX()
        {
            return Utils.CreateContext(WebView, Logger, nameof(ContextMenuItem), Logger.Source, OnItemDispose);
        }

        private void OnItemDispose(string uid)
        {
            this._ItemInstances.Remove(uid);
        }



        #endregion

    }
}