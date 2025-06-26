using Microsoft.Web.WebView2.Core;
using WV.Interfaces;

namespace WV.Win.Imp
{
    public class ContextMenuItem : IContextMenuItem
    {
        

        internal CoreWebView2ContextMenuItem Item { get; }
        private Stream? Stream { get; set; }
        private WebView WV { get; }
        private List<IContextMenuItem> InternalChildren { get; } = new List<IContextMenuItem>();

        public ContextMenuItem(WebView wv, CoreWebView2ContextMenuItem item, string? icon, Stream? stream = null)
        { 
            this.WV = wv;
            this.Item = item;
            this.Name = item.Name;
            this.kind = item.Kind.ToString();
            this.Icon = string.IsNullOrWhiteSpace(icon) ? string.Empty : icon;
            this.Stream = stream;

            if(this.Item.Kind != CoreWebView2ContextMenuItemKind.Separator || this.Item.Kind == CoreWebView2ContextMenuItemKind.Submenu)
                this.Item.CustomItemSelected += Item_CustomItemSelected;
        }

        private void Item_CustomItemSelected(object? sender, object e)
        {
            if (sender == null)
                return;

            CoreWebView2ContextMenuItem item = (CoreWebView2ContextMenuItem)sender;

            if(item.Kind == CoreWebView2ContextMenuItemKind.CheckBox || item.Kind == CoreWebView2ContextMenuItemKind.Radio)
                item.IsChecked = !item.IsChecked;

            this.JSFn?.Execute(item.Kind.ToString(), item.IsChecked);
        }

        public string Name { get; }

        public string kind { get; }

        public string Icon { get; }

        public IContextMenuItem? Parent { get; internal set; }

        public IContextMenuItem[] Children
        {
            get
            {
                Plugin.ThrowDispose(this.WV);
                return this.Children.ToArray();
            }
        }

        public bool Checked 
        {
            get => this.Item.IsChecked;
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.Item.IsChecked = value;
            }
        }

        public bool Enabled 
        {
            get => this.Item.IsEnabled;
            set
            {
                Plugin.ThrowDispose(this.WV);
                this.Item.IsEnabled = value;
            }
        }

        #region Visible

        public bool _Visible = true;
        public bool Visible 
        {
            get => _Visible; 
            set
            {
                Plugin.ThrowDispose(this.WV);
                _Visible = value;
            }
        }

        #endregion

        #region Callback

        private IJSFunction? JSFn { get; set; }
        public object? Callback 
        {
            get => JSFn!.Raw;
            set
            {
                Plugin.ThrowDispose(this.WV);

                if(value == this.JSFn?.Raw)
                    return;

                this.JSFn?.Dispose();
                this.JSFn = null;

                if (value == null)
                    return;

                this.JSFn = IJSFunction.Create(value);
            }
        }

        #endregion

        public void AddItem(IContextMenuItem item)
        {
            Plugin.ThrowDispose(this.WV);
            this.CheckKind(this.Item);
            this.AllowInsertItem(item);

            ContextMenuItem rawItem = GetRamItem(item);

            this.Item.Children.Add(rawItem.Item);
            this.InternalChildren.Add(item);

            rawItem.Parent = this;

        }

        public void Clear()
        {
            Plugin.ThrowDispose(this.WV);

            this.Item.Children.Clear();

            foreach (var item in this.InternalChildren)
                ((ContextMenuItem)item).Parent = null;

            this.InternalChildren.Clear();
        }
        
        public void InsertItem(int index, IContextMenuItem item)
        {
            Plugin.ThrowDispose(this.WV);
            this.CheckKind(this.Item);
            this.AllowInsertItem(item);

            ContextMenuItem rawItem = GetRamItem(item);

            this.Item.Children.Insert(index, rawItem.Item);
            this.InternalChildren.Insert(index, item);

            rawItem.Parent = this;
        }

        public void RemoveItem(IContextMenuItem item)
        {
            Plugin.ThrowDispose(this.WV);
            this.CheckKind(this.Item);

            ContextMenuItem rawItem = GetRamItem(item);

            this.Item.Children.Remove(rawItem.Item);
            this.InternalChildren.Remove(item);

            rawItem.Parent = null;
        }

        public void RemoveItemAt(int index)
        {
            Plugin.ThrowDispose(this.WV);
            this.CheckKind(this.Item);

            ContextMenuItem rawItem = GetRamItem(this.InternalChildren[index]);

            this.Item.Children.RemoveAt(index);
            this.InternalChildren.RemoveAt(index);

            rawItem.Parent = null;
        }

        #region HELPERS

        private void AllowInsertItem(IContextMenuItem item)
        {

            if (item.Parent != null)
                throw new Exception("This item belongs to a submenu");

            if (this.InternalChildren.Contains(item))
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

        #endregion

        #region DISPOSE

        internal bool Disposed;

        private void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (this.Item.Kind != CoreWebView2ContextMenuItemKind.Separator || this.Item.Kind == CoreWebView2ContextMenuItemKind.Submenu)
                this.Item.CustomItemSelected -= Item_CustomItemSelected;

            this.JSFn = null!;
            this.Parent = null;
            this.Stream?.Dispose();
            this.Stream = null!;
            //this.Item.Children.Clear();   // Causa excepción

            foreach (var item in this.InternalChildren)
                item.Dispose();
            
            this.InternalChildren.Clear();
        }

        public void Dispose()
        {
            if(Disposed) 
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
    }
}