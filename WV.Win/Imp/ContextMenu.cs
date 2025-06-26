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
        
        private List<IContextMenuItem> InternalInstances {  get; } = new();
        private List<IContextMenuItem> InternalChildren { get; } = new();
        private WebView WV { get; }

        private CoreWebView2Controller WVController
        {
            get
            {
                if (this.WV.WVController == null)
                    throw new Exception(this.WV.Name + " no ready");

                return this.WV.WVController;
            }
        }

        private Dictionary<string, NatItem> DicNativeItems { get; } = new();

        public ContextMenu(WebView wv) 
        { 
            this.WV = wv;
            PropertyInfo[] props = this.GetType().GetProperties();
            foreach (PropertyInfo pi in props) 
            {
                string name = pi.Name.Replace("Item", "");
                if(NativeItemCodes.TryGetValue(name, out int value))
                    this.DicNativeItems[pi.Name] = new NatItem { Cod = value };
            }

        }

        public bool Enable { get; set; } = true;

        public IContextMenuItem[] Children
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.InternalChildren.ToArray();
            }
        }


        #region PROPS Native Items

        public bool ShowNativeItems { get; set; } = true;

        public bool EmojiItem 
        { 
            get => this.DicNativeItems[nameof(EmojiItem)].Visible; 
            set => this.DicNativeItems[nameof(EmojiItem)].Visible = value;
        }

        public bool UndoItem
        {
            get => this.DicNativeItems[nameof(UndoItem)].Visible;
            set => this.DicNativeItems[nameof(UndoItem)].Visible = value;
        }

        public bool RedoItem
        {
            get => this.DicNativeItems[nameof(RedoItem)].Visible;
            set => this.DicNativeItems[nameof(RedoItem)].Visible = value;
        }

        public bool CutItem
        {
            get => this.DicNativeItems[nameof(CutItem)].Visible;
            set => this.DicNativeItems[nameof(CutItem)].Visible = value;
        }

        public bool CopyItem
        {
            get => this.DicNativeItems[nameof(CopyItem)].Visible;
            set => this.DicNativeItems[nameof(CopyItem)].Visible = value;
        }

        public bool PasteItem
        {
            get => this.DicNativeItems[nameof(PasteItem)].Visible;
            set => this.DicNativeItems[nameof(PasteItem)].Visible = value;
        }

        public bool PasteAndMatchStyleItem
        {
            get => this.DicNativeItems[nameof(PasteAndMatchStyleItem)].Visible;
            set => this.DicNativeItems[nameof(PasteAndMatchStyleItem)].Visible = value;
        }

        public bool SelectAllItem
        {
            get => this.DicNativeItems[nameof(SelectAllItem)].Visible;
            set => this.DicNativeItems[nameof(SelectAllItem)].Visible = value;
        }

        public bool WritingDirectionItem
        {
            get => this.DicNativeItems[nameof(WritingDirectionItem)].Visible;
            set => this.DicNativeItems[nameof(WritingDirectionItem)].Visible = value;
        }

        public bool ShareItem
        {
            get => this.DicNativeItems[nameof(ShareItem)].Visible;
            set => this.DicNativeItems[nameof(ShareItem)].Visible = value;
        }

        public bool WebCaptureItem
        {
            get => this.DicNativeItems[nameof(WebCaptureItem)].Visible;
            set => this.DicNativeItems[nameof(WebCaptureItem)].Visible = value;
        }

        public bool LoopItem
        {
            get => this.DicNativeItems[nameof(LoopItem)].Visible;
            set => this.DicNativeItems[nameof(LoopItem)].Visible = value;
        }

        public bool ShowAllControlsItem
        {
            get => this.DicNativeItems[nameof(ShowAllControlsItem)].Visible;
            set => this.DicNativeItems[nameof(ShowAllControlsItem)].Visible = value;
        }

        public bool SaveMediaAsItem
        {
            get => this.DicNativeItems[nameof(SaveMediaAsItem)].Visible;
            set => this.DicNativeItems[nameof(SaveMediaAsItem)].Visible = value;
        }

        public bool CopyLinkItem
        {
            get => this.DicNativeItems[nameof(CopyLinkItem)].Visible;
            set => this.DicNativeItems[nameof(CopyLinkItem)].Visible = value;
        }

        public bool CopyLinkToHighlightItem
        {
            get => this.DicNativeItems[nameof(CopyLinkToHighlightItem)].Visible;
            set => this.DicNativeItems[nameof(CopyLinkToHighlightItem)].Visible = value;
        }

        public bool PrintItem
        {
            get => this.DicNativeItems[nameof(PrintItem)].Visible;
            set => this.DicNativeItems[nameof(PrintItem)].Visible = value;
        }

        public bool BackItem
        {
            get => this.DicNativeItems[nameof(BackItem)].Visible;
            set => this.DicNativeItems[nameof(BackItem)].Visible = value;
        }

        public bool ForwardItem
        {
            get => this.DicNativeItems[nameof(ForwardItem)].Visible;
            set => this.DicNativeItems[nameof(ForwardItem)].Visible = value;
        }

        public bool ReloadItem
        {
            get => this.DicNativeItems[nameof(ReloadItem)].Visible;
            set => this.DicNativeItems[nameof(ReloadItem)].Visible = value;
        }

        public bool SaveAsItem
        {
            get => this.DicNativeItems[nameof(SaveAsItem)].Visible;
            set => this.DicNativeItems[nameof(SaveAsItem)].Visible = value;
        }

        public bool SaveImageAsItem
        {
            get => this.DicNativeItems[nameof(SaveImageAsItem)].Visible;
            set => this.DicNativeItems[nameof(SaveImageAsItem)].Visible = value;
        }

        public bool CopyImageItem
        {
            get => this.DicNativeItems[nameof(CopyImageItem)].Visible;
            set => this.DicNativeItems[nameof(CopyImageItem)].Visible = value;
        }

        public bool CopyImageLocationItem
        {
            get => this.DicNativeItems[nameof(CopyImageLocationItem)].Visible;
            set => this.DicNativeItems[nameof(CopyImageLocationItem)].Visible = value;
        }

        public bool MagnifyImageItem
        {
            get => this.DicNativeItems[nameof(MagnifyImageItem)].Visible;
            set => this.DicNativeItems[nameof(MagnifyImageItem)].Visible = value;
        }

        public bool SaveFrameAsItem
        {
            get => this.DicNativeItems[nameof(SaveFrameAsItem)].Visible;
            set => this.DicNativeItems[nameof(SaveFrameAsItem)].Visible = value;
        }

        public bool CopyVideoFrameItem
        {
            get => this.DicNativeItems[nameof(CopyVideoFrameItem)].Visible;
            set => this.DicNativeItems[nameof(CopyVideoFrameItem)].Visible = value;
        }

        public bool PictureInPictureItem
        {
            get => this.DicNativeItems[nameof(PictureInPictureItem)].Visible;
            set => this.DicNativeItems[nameof(PictureInPictureItem)].Visible = value;
        }

        public bool SaveLinkAsItem
        {
            get => this.DicNativeItems[nameof(SaveLinkAsItem)].Visible;
            set => this.DicNativeItems[nameof(SaveLinkAsItem)].Visible = value;
        }

        public bool OpenLinkInNewWindowItem
        {
            get => this.DicNativeItems[nameof(OpenLinkInNewWindowItem)].Visible;
            set => this.DicNativeItems[nameof(OpenLinkInNewWindowItem)].Visible = value;
        }


        #endregion

        public void Clear()
        {
            Plugin.ThrowDispose(this.WV);
            this.InternalChildren.Clear();
        }

        public IContextMenuItem CreateContextItem(string name, string kind, string? icon = null, object? callback = null)
        {
            Plugin.ThrowDispose(this.WV);

            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name) + " can`t be null or empty");

            if (!Enum.TryParse(kind, out CoreWebView2ContextMenuItemKind ekind))
                throw new Exception("Unknown kind '" + kind + "'.");

            Stream? stream = null;

            if (!string.IsNullOrWhiteSpace(icon))
            {
                string fullPath = icon;

                if (!Path.IsPathFullyQualified(icon))
                    fullPath = AppManager.SrcPath + "/" + icon;

                if (!File.Exists(fullPath))
                    throw new FileNotFoundException("File not found: '" + icon + "'");

                stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            CoreWebView2ContextMenuItem subItem = this.WVController.CoreWebView2.Environment.CreateContextMenuItem(
                name,
                stream,
                ekind
            );

            var newItem = new ContextMenuItem(this.WV, subItem, icon, stream)
            {
                Callback = callback,
            };

            this.InternalInstances.Add(newItem);

            return newItem;
        }

        public IContextMenuItem CreateContextItemSeparator()
        {
            Plugin.ThrowDispose(this.WV);

            CoreWebView2ContextMenuItem subItem = this.WVController.CoreWebView2.Environment.CreateContextMenuItem(
                "",
                null,
                CoreWebView2ContextMenuItemKind.Separator
            );

            var newItem = new ContextMenuItem(this.WV, subItem, null);

            this.InternalInstances.Add(newItem);

            return newItem;
        }

        public void AddItem(IContextMenuItem item)
        {
            Plugin.ThrowDispose(this.WV);
            this.AllowInsertItem(item);
            this.InternalChildren.Add(item);
        }

        public void InsertItem(int index, IContextMenuItem item)
        {
            Plugin.ThrowDispose(this.WV);
            this.AllowInsertItem(item);
            this.InternalChildren.Insert(index, item);
        }

        public void RemoveItem(IContextMenuItem item)
        {
            Plugin.ThrowDispose(this.WV);
            this.InternalChildren.Remove(item);
        }

        public void RemoveItemAt(int index)
        {
            Plugin.ThrowDispose(this.WV);
            this.InternalChildren.RemoveAt(index);
        }

        private void AllowInsertItem(IContextMenuItem item)
        {
            if (item.Parent != null)
                throw new Exception("This item belongs to a submenu");

            if (this.InternalChildren.Contains(item))
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
                foreach (var item in this.DicNativeItems.Values)
                    menuItems.RemoveAll(x => x.CommandId == item.Cod && !item.Visible);

                // Inyectar los items nativos que se quieren mostrar
                foreach (var item in menuItems)
                    e.MenuItems.Add(item);
            }
                
            // Inyectar Custom Items
            foreach (var item in this.InternalChildren)
                if(item.Visible)
                    e.MenuItems.Add(((ContextMenuItem)item).Item);
        }

        internal void ClearEvents()
        {
            this.ToDefault();
        }

        internal void ToDefault()
        {
            foreach (var item in this.DicNativeItems)
                item.Value.Visible = true;

            this.InternalChildren.Clear();
            
            foreach (var item in this.InternalInstances)
                item.Dispose();

            this.InternalInstances.Clear();
        }
    }
}