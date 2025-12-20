using Microsoft.Web.WebView2.Core;
using WV.Interfaces;

namespace WV.Win.Imp
{
    public class ContextMenuItem : IContextMenuItem
    {
        #region Private Fields

        private string label;
        private CoreWebView2ContextMenuItemKind kind;
        private string icon;
        private IContextMenuItem? parent;
        private List<IContextMenuItem> children;
        //private bool checked 
        //private bool enabled;
        private bool visible;
        private IJSFunction? callback;

        #endregion

        internal CoreWebView2ContextMenuItem Item { get; set; }
        private Stream? stream { get; set; }
        private WebView WV { get; }

        internal ContextMenuItem(WebView wv, string kind, string label, string icon)
        {
            this.label = string.Empty;
            this.icon = string.Empty;
            this.visible = true;
            this.children = new List<IContextMenuItem>();
            this.WV = wv;

            this.kind = GetKind(kind);
            this.label = GetLabel(label);
            this.icon = icon;
            this.stream = GetStream(icon);
            CreateItem();
        }

        #region PROPS

        public string Label 
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.Label; 
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                if (value == label) return;
                label = GetLabel(value);
                CreateItem();
            }
        }

        public string Kind 
        { 
            get
            {
                Plugin.ThrowDispose(this.WV);
                return kind.ToString();
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                if (value == Kind) return;
                kind = GetKind(value);
                CreateItem();
            }
        }

        public string Icon
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return icon;
            }
            set
            {
                Plugin.ThrowDispose(this.WV);
                if (value == icon) return;
                icon = value;
                stream = GetStream(icon);
                CreateItem();
            }
        }

        public IContextMenuItem? Parent 
        { 
            get
            {
                ThrowDispose();
                return parent;
            }
            internal set
            {
                ThrowDispose(); 
                parent = value;
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

        public bool Checked 
        {
            get
            {
                ThrowDispose();
                return this.Item.IsChecked;
            }
            set
            {
                ThrowDispose();
                this.Item.IsChecked = value;
            }
        }

        public bool Enabled 
        {
            get
            {
                ThrowDispose();
                return this.Item.IsEnabled;
            }
            set
            {
                ThrowDispose();
                this.Item.IsEnabled = value;
            }
        }

        public bool Visible 
        {
            get
            {
                ThrowDispose();
                return visible;
            }
            set
            {
                ThrowDispose();
                visible = value;
            }
        }

        public object? Callback 
        {
            get
            {
                ThrowDispose();
                return callback!.Raw;
            }
            set
            {
                ThrowDispose();

                if (value == this.callback?.Raw)
                    return;

                this.callback?.Dispose();
                this.callback = null;

                if (value == null)
                    return;

                this.callback = IJSFunction.Create(value);
            }
        }

        #endregion

        #region METHODS

        public void AddItem(IContextMenuItem item)
        {
            ThrowDispose();
            this.CheckKind(this.Item);
            this.AllowInsertItem(item);

            ContextMenuItem rawItem = GetRamItem(item);

            this.Item.Children.Add(rawItem.Item);
            this.children.Add(item);

            rawItem.Parent = this;

        }

        public void InsertItem(int index, IContextMenuItem item)
        {
            ThrowDispose();
            this.CheckKind(this.Item);
            this.AllowInsertItem(item);

            ContextMenuItem rawItem = GetRamItem(item);

            this.Item.Children.Insert(index, rawItem.Item);
            this.children.Insert(index, item);

            rawItem.Parent = this;
        }

        public void RemoveItem(IContextMenuItem item)
        {
            ThrowDispose();
            this.CheckKind(this.Item);

            ContextMenuItem rawItem = GetRamItem(item);

            this.Item.Children.Remove(rawItem.Item);
            this.children.Remove(item);

            rawItem.Parent = null;
        }

        public void RemoveItemAt(int index)
        {
            ThrowDispose();
            this.CheckKind(this.Item);

            ContextMenuItem rawItem = GetRamItem(this.children[index]);

            this.Item.Children.RemoveAt(index);
            this.children.RemoveAt(index);

            rawItem.Parent = null;
        }

        public void Clear()
        {
            ThrowDispose();

            this.Item.Children.Clear();

            foreach (var item in this.children)
                ((ContextMenuItem)item).Parent = null;

            this.children.Clear();
        }

        #endregion

        #region DISPOSE

        internal bool Disposed;

        private void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (this.Item != null && this.Item.Kind != CoreWebView2ContextMenuItemKind.Separator || this.Item.Kind == CoreWebView2ContextMenuItemKind.Submenu)
                this.Item.CustomItemSelected -= Item_CustomItemSelected;

            this.callback = null!;
            this.Parent = null;
            this.stream?.Dispose();
            this.stream = null!;
            //this.Item.Children.Clear();   // Causa excepción

            foreach (var item in this.children)
                item.Dispose();

            this.children.Clear();
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            Dispose(true);

            // Evitar que el Garbage Collector llame al destructor/Finalizador ~Plugin()
            GC.SuppressFinalize(this);

            Disposed = true;
        }

        ~ContextMenuItem()
        {
            Dispose(false);
            Disposed = true;
        }

        #endregion

        #region HELPERS

        private void CreateItem()
        {
            if (this.WV.WVController == null)
                throw new Exception(this.WV.Name + " no ready");

            // Si ya habia un Item creado, quitarle el evento
            if(this.Item != null && IsSelecteable(kind))
                this.Item.CustomItemSelected -= Item_CustomItemSelected;

            this.Item = this.WV.WVController.CoreWebView2.Environment.CreateContextMenuItem(label, stream, kind);

            if(IsSelecteable(kind))
                this.Item.CustomItemSelected += Item_CustomItemSelected;
        }

        private void Item_CustomItemSelected(object? sender, object e)
        {
            if (sender == null)
                return;

            CoreWebView2ContextMenuItem item = (CoreWebView2ContextMenuItem)sender;

            if (item.Kind == CoreWebView2ContextMenuItemKind.CheckBox || item.Kind == CoreWebView2ContextMenuItemKind.Radio)
                item.IsChecked = !item.IsChecked;

            this.callback?.Execute(item.Kind.ToString(), item.IsChecked);
        }

        private bool IsSelecteable(CoreWebView2ContextMenuItemKind kind)
        {
            return !(kind == CoreWebView2ContextMenuItemKind.Separator || kind == CoreWebView2ContextMenuItemKind.Submenu);
        }

        private static string GetLabel(string label)
        {
            return label ?? string.Empty;
        }

        private static CoreWebView2ContextMenuItemKind GetKind(string kind)
        {
            if (!Enum.TryParse(kind, out CoreWebView2ContextMenuItemKind ekind))
                throw new Exception("Unknown kind '" + kind + "'.");

            return ekind;
        }

        private static Stream? GetStream(string icon)
        {
            if (string.IsNullOrWhiteSpace(icon))
                return null;

            string fullPath = icon;

            if (!Path.IsPathFullyQualified(icon))
                fullPath = AppManager.SrcPath + "/" + icon;

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found: '" + icon + "'");

            return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private void AllowInsertItem(IContextMenuItem item)
        {

            if (item.Parent != null)
                throw new Exception("This item belongs to a submenu");

            if (this.children.Contains(item))
                throw new Exception("This item already exists");
        }
        private void CheckKind(CoreWebView2ContextMenuItem item)
        {
            if (item.Kind == CoreWebView2ContextMenuItemKind.Submenu)
                return;

            throw new Exception("This item is not a submenu");
        }

        private ContextMenuItem GetRamItem(IContextMenuItem item)
        {
            ContextMenuItem rawItem = (ContextMenuItem)item;

            if (rawItem.Disposed)
                throw new InvalidOperationException("item is disposed");

            return rawItem;
        }

        private void ThrowDispose()
        {
            Plugin.ThrowDispose(this.WV);
        }

        #endregion

    }
}