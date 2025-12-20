using Microsoft.Web.WebView2.Core;
using System.Reflection;
using WV.Interfaces;

namespace WV.Win.Imp
{
    public class ContextMenu : IContextMenu
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

        #region Private fields

        private bool enable;
        private List<IContextMenuItem> children;

        #endregion

        private WebView WV { get; }
        private Dictionary<string, NatItem> DicPropNativeItems { get; } = new();
        private List<IContextMenuItem> InternalInstances { get; } = new();

        public ContextMenu(WebView wv) 
        {
            this.enable = true;
            this.children = new List<IContextMenuItem>();
            this.WV = wv;

            // Obtener nombres de PROPS Native Items
            PropertyInfo[] props = this.GetType().GetProperties();
            foreach (PropertyInfo pi in props) 
            {
                string name = pi.Name.Replace("Item", "");
                if(NativeItemCodes.TryGetValue(name, out int value))
                    this.DicPropNativeItems[pi.Name] = new NatItem { Cod = value };
            }

        }

        #region PROPS

        public bool Enable 
        { 
            get
            {
                ThrowDispose();
                return enable;
            }
            set
            {
                ThrowDispose();
                enable = value;
            }
        }

        public IContextMenuItem[] Children
        {
            get
            {
                ThrowDispose();
                return this.children.ToArray();
            }
        }

        #endregion

        #region PROPS Native Items

        public bool ShowNativeItems { get; set; } = true;

        public bool EmojiItem 
        { 
            get => this.DicPropNativeItems[nameof(EmojiItem)].Visible; 
            set => this.DicPropNativeItems[nameof(EmojiItem)].Visible = value;
        }

        public bool UndoItem
        {
            get => this.DicPropNativeItems[nameof(UndoItem)].Visible;
            set => this.DicPropNativeItems[nameof(UndoItem)].Visible = value;
        }

        public bool RedoItem
        {
            get => this.DicPropNativeItems[nameof(RedoItem)].Visible;
            set => this.DicPropNativeItems[nameof(RedoItem)].Visible = value;
        }

        public bool CutItem
        {
            get => this.DicPropNativeItems[nameof(CutItem)].Visible;
            set => this.DicPropNativeItems[nameof(CutItem)].Visible = value;
        }

        public bool CopyItem
        {
            get => this.DicPropNativeItems[nameof(CopyItem)].Visible;
            set => this.DicPropNativeItems[nameof(CopyItem)].Visible = value;
        }

        public bool PasteItem
        {
            get => this.DicPropNativeItems[nameof(PasteItem)].Visible;
            set => this.DicPropNativeItems[nameof(PasteItem)].Visible = value;
        }

        public bool PasteAndMatchStyleItem
        {
            get => this.DicPropNativeItems[nameof(PasteAndMatchStyleItem)].Visible;
            set => this.DicPropNativeItems[nameof(PasteAndMatchStyleItem)].Visible = value;
        }

        public bool SelectAllItem
        {
            get => this.DicPropNativeItems[nameof(SelectAllItem)].Visible;
            set => this.DicPropNativeItems[nameof(SelectAllItem)].Visible = value;
        }

        public bool WritingDirectionItem
        {
            get => this.DicPropNativeItems[nameof(WritingDirectionItem)].Visible;
            set => this.DicPropNativeItems[nameof(WritingDirectionItem)].Visible = value;
        }

        public bool ShareItem
        {
            get => this.DicPropNativeItems[nameof(ShareItem)].Visible;
            set => this.DicPropNativeItems[nameof(ShareItem)].Visible = value;
        }

        public bool WebCaptureItem
        {
            get => this.DicPropNativeItems[nameof(WebCaptureItem)].Visible;
            set => this.DicPropNativeItems[nameof(WebCaptureItem)].Visible = value;
        }

        public bool LoopItem
        {
            get => this.DicPropNativeItems[nameof(LoopItem)].Visible;
            set => this.DicPropNativeItems[nameof(LoopItem)].Visible = value;
        }

        public bool ShowAllControlsItem
        {
            get => this.DicPropNativeItems[nameof(ShowAllControlsItem)].Visible;
            set => this.DicPropNativeItems[nameof(ShowAllControlsItem)].Visible = value;
        }

        public bool SaveMediaAsItem
        {
            get => this.DicPropNativeItems[nameof(SaveMediaAsItem)].Visible;
            set => this.DicPropNativeItems[nameof(SaveMediaAsItem)].Visible = value;
        }

        public bool CopyLinkItem
        {
            get => this.DicPropNativeItems[nameof(CopyLinkItem)].Visible;
            set => this.DicPropNativeItems[nameof(CopyLinkItem)].Visible = value;
        }

        public bool CopyLinkToHighlightItem
        {
            get => this.DicPropNativeItems[nameof(CopyLinkToHighlightItem)].Visible;
            set => this.DicPropNativeItems[nameof(CopyLinkToHighlightItem)].Visible = value;
        }

        public bool PrintItem
        {
            get => this.DicPropNativeItems[nameof(PrintItem)].Visible;
            set => this.DicPropNativeItems[nameof(PrintItem)].Visible = value;
        }

        public bool BackItem
        {
            get => this.DicPropNativeItems[nameof(BackItem)].Visible;
            set => this.DicPropNativeItems[nameof(BackItem)].Visible = value;
        }

        public bool ForwardItem
        {
            get => this.DicPropNativeItems[nameof(ForwardItem)].Visible;
            set => this.DicPropNativeItems[nameof(ForwardItem)].Visible = value;
        }

        public bool ReloadItem
        {
            get => this.DicPropNativeItems[nameof(ReloadItem)].Visible;
            set => this.DicPropNativeItems[nameof(ReloadItem)].Visible = value;
        }

        public bool SaveAsItem
        {
            get => this.DicPropNativeItems[nameof(SaveAsItem)].Visible;
            set => this.DicPropNativeItems[nameof(SaveAsItem)].Visible = value;
        }

        public bool SaveImageAsItem
        {
            get => this.DicPropNativeItems[nameof(SaveImageAsItem)].Visible;
            set => this.DicPropNativeItems[nameof(SaveImageAsItem)].Visible = value;
        }

        public bool CopyImageItem
        {
            get => this.DicPropNativeItems[nameof(CopyImageItem)].Visible;
            set => this.DicPropNativeItems[nameof(CopyImageItem)].Visible = value;
        }

        public bool CopyImageLocationItem
        {
            get => this.DicPropNativeItems[nameof(CopyImageLocationItem)].Visible;
            set => this.DicPropNativeItems[nameof(CopyImageLocationItem)].Visible = value;
        }

        public bool MagnifyImageItem
        {
            get => this.DicPropNativeItems[nameof(MagnifyImageItem)].Visible;
            set => this.DicPropNativeItems[nameof(MagnifyImageItem)].Visible = value;
        }

        public bool SaveFrameAsItem
        {
            get => this.DicPropNativeItems[nameof(SaveFrameAsItem)].Visible;
            set => this.DicPropNativeItems[nameof(SaveFrameAsItem)].Visible = value;
        }

        public bool CopyVideoFrameItem
        {
            get => this.DicPropNativeItems[nameof(CopyVideoFrameItem)].Visible;
            set => this.DicPropNativeItems[nameof(CopyVideoFrameItem)].Visible = value;
        }

        public bool PictureInPictureItem
        {
            get => this.DicPropNativeItems[nameof(PictureInPictureItem)].Visible;
            set => this.DicPropNativeItems[nameof(PictureInPictureItem)].Visible = value;
        }

        public bool SaveLinkAsItem
        {
            get => this.DicPropNativeItems[nameof(SaveLinkAsItem)].Visible;
            set => this.DicPropNativeItems[nameof(SaveLinkAsItem)].Visible = value;
        }

        public bool OpenLinkInNewWindowItem
        {
            get => this.DicPropNativeItems[nameof(OpenLinkInNewWindowItem)].Visible;
            set => this.DicPropNativeItems[nameof(OpenLinkInNewWindowItem)].Visible = value;
        }


        #endregion

        #region METHODS

        public IContextMenuItem CreateContextItem(string label, string kind, string? icon = null, object? callback = null)
        {
            ThrowDispose();
            var item = new ContextMenuItem(this.WV, kind, label, icon ?? string.Empty){ Callback = callback };
            this.InternalInstances.Add(item);
            return item;
        }

        public IContextMenuItem CreateContextItemSeparator()
        {
            ThrowDispose();
            var item = new ContextMenuItem(this.WV, CoreWebView2ContextMenuItemKind.Separator.ToString(), string.Empty, string.Empty);
            this.InternalInstances.Add(item);
            return item;
        }

        public void AddItem(IContextMenuItem item)
        {
            ThrowDispose();
            this.AllowInsertItem(item);
            this.children.Add(item);
        }

        public void InsertItem(int index, IContextMenuItem item)
        {
            ThrowDispose();
            this.AllowInsertItem(item);
            this.children.Insert(index, item);
        }

        public bool RemoveItem(IContextMenuItem item)
        {
            ThrowDispose();
            return this.children.Remove(item);
        }

        public void RemoveItemAt(int index)
        {
            ThrowDispose();
            this.children.RemoveAt(index);
        }

        public void Clear()
        {
            ThrowDispose();
            this.children.Clear();
        }

        #endregion

        #region HELPERS

        private void AllowInsertItem(IContextMenuItem item)
        {
            if (item.Parent != null)
                throw new Exception("This item belongs to a submenu");

            if (this.children.Contains(item))
                throw new Exception("This item already exists");
        }

        internal void ContextMenuHandler(CoreWebView2 coreWV2, CoreWebView2ContextMenuRequestedEventArgs e)
        {
            if (!this.Enable)
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
            if (this.ShowNativeItems)
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
            foreach (var item in this.children)
                if(item.Visible)
                    e.MenuItems.Add(((ContextMenuItem)item).Item);
        }

        internal void ClearEvents()
        {
            this.ToDefault();
        }

        internal void ToDefault()
        {
            foreach (var item in this.DicPropNativeItems)
                item.Value.Visible = true;

            this.children.Clear();
            
            foreach (var item in this.InternalInstances)
                item.Dispose();

            this.InternalInstances.Clear();
        }

        private void ThrowDispose()
        {
            Plugin.ThrowDispose(this.WV);
        }

        #endregion

    }
}