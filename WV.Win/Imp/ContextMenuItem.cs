using Microsoft.Web.WebView2.Core;
using WV.Core.Browsering;
using WV.Interfaces;

namespace WV.Win.Imp
{
    public class ContextMenuItem : ContextMenuItemCore
    {
        #region Fields

        private string _label = string.Empty;
        private CoreWebView2ContextMenuItemKind _kind;
        private string _icon = string.Empty;
        private bool _visible;
        private IJSFunction? _callback;
        internal CoreWebView2ContextMenuItem? _item;
        private Stream? _stream;

        #endregion

        internal CoreWebView2ContextMenuItem _Item => this._item!;

        private string Directory => ((WebView)WebView).InternalBrowser.Directory;

        internal ContextMenuItem(IPluginContext context, string kind, string label, string icon, string directory) : base(context, kind, label, icon)
        {
        }

        protected override void Initialize(IPluginContext context, string kind, string label, string icon)
        {
            this._label = GetLabel(label);
            this._kind = GetKind(kind);
            this._icon = icon;
            this._visible = true;
            this._stream = GetStream(icon, Directory);
            CreateNativeItem();
        }

        #region Properties

        protected override string LabelCore 
        {
            get
            {
                return this._label; 
            }
            set
            {
                if (value == _label) 
                    return;

                _label = GetLabel(value);
                CreateNativeItem();
            }
        }

        protected override string KindCore
        { 
            get
            {
                return _kind.ToString();
            }
            set
            {
                if (value == Kind) 
                    return;

                _kind = GetKind(value);
                CreateNativeItem();
            }
        }

        protected override string IconCore
        {
            get
            {
                return _icon;
            }
            set
            {
                if (value == _icon) 
                    return;
                
                _icon = value;
                _stream = GetStream(_icon, Directory);
                CreateNativeItem();
            }
        }

        protected override bool CheckedCore
        {
            get
            {
                return this._Item.IsChecked;
            }
            set
            {
                this._Item.IsChecked = value;
            }
        }

        protected override bool EnabledCore
        {
            get
            {
                return this._Item.IsEnabled;
            }
            set
            {
                this._Item.IsEnabled = value;
            }
        }

        protected override bool VisibleCore
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        protected override object? CallbackCore
        {
            get
            {
                return _callback?.Raw;
            }
            set
            {
                if (value == this._callback?.Raw)
                    return;

                this._callback?.Dispose();
                this._callback = null;

                if (value == null)
                    return;

                this._callback = IJSFunction.Create(value);
            }
        }

        #endregion

        #region Methods

        protected override void AddItemCore(IContextMenuItem item)
        {
            this.CheckKind(this._Item);
            this.AllowInsertItem(item);

            ContextMenuItem rawItem = GetRawItem(item);

            this._Item.Children.Add(rawItem._Item);
            this._children.Add(item);

            rawItem._parent = this;

        }

        protected override void InsertItemCore(int index, IContextMenuItem item)
        {
            this.CheckKind(this._Item);
            this.AllowInsertItem(item);

            ContextMenuItem rawItem = GetRawItem(item);

            this._Item.Children.Insert(index, rawItem._Item);
            this._children.Insert(index, item);

            rawItem._parent = this;
        }

        protected override void RemoveItemCore(IContextMenuItem item)
        {
            this.CheckKind(this._Item);

            ContextMenuItem rawItem = GetRawItem(item);

            this._Item.Children.Remove(rawItem._Item);
            this._children.Remove(item);

            rawItem._parent = null;
        }

        protected override void RemoveItemAtCore(int index)
        {
            this.CheckKind(this._Item);

            ContextMenuItem rawItem = GetRawItem(this._children[index]);

            this._Item.Children.RemoveAt(index);
            this._children.RemoveAt(index);

            rawItem._parent = null;
        }

        protected override void ClearCore()
        {
            this._Item.Children.Clear();

            foreach (var item in this._children)
                ((ContextMenuItem)item)._parent = null;

            this._children.Clear();
        }

        #endregion

        protected override void DisposeCore(bool disposing)
        {
            if(!disposing)
                return;

            if(_item != null)
            {
                if(_item.Kind != CoreWebView2ContextMenuItemKind.Separator || _item.Kind == CoreWebView2ContextMenuItemKind.Submenu)
                    _item.CustomItemSelected -= Item_CustomItemSelected;

                //_item.Children.Clear(); // Causa excepción
                _item = null;
            }

            _callback?.Dispose();
            _callback = null;

            _stream?.Dispose();
            _stream = null;
        }

        #region HELPERS

        private void CreateNativeItem()
        {
            var wv = (WebView)WebView;
            var WVController = wv.InternalBrowser.WVController;

            if (WVController == null)
                throw new Exception(wv.Name + " no ready");

            // Si ya habia un Item creado, quitarle el evento
            if(_item != null && IsSelecteable(_kind))
                _item.CustomItemSelected -= Item_CustomItemSelected;

            _item = WVController.CoreWebView2.Environment.CreateContextMenuItem(_label, _stream, _kind);

            if(IsSelecteable(_kind))
                _item.CustomItemSelected += Item_CustomItemSelected;
        }

        private void Item_CustomItemSelected(object? sender, object e)
        {
            if (sender == null)
                return;

            CoreWebView2ContextMenuItem item = (CoreWebView2ContextMenuItem)sender;

            if (item.Kind == CoreWebView2ContextMenuItemKind.CheckBox || item.Kind == CoreWebView2ContextMenuItemKind.Radio)
                item.IsChecked = !item.IsChecked;

            this._callback?.Execute(item.Kind.ToString(), item.IsChecked);
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

        private static Stream? GetStream(string icon, string directory)
        {
            if (string.IsNullOrWhiteSpace(icon))
                return null;

            string fullPath = icon;

            if (!Path.IsPathFullyQualified(icon))
                fullPath = Path.Combine(directory, icon);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found: '" + icon + "'");

            return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private void AllowInsertItem(IContextMenuItem item)
        {

            if (item.Parent != null)
                throw new Exception("This item belongs to a submenu");

            if (this._children.Contains(item))
                throw new Exception("This item already exists");
        }
        
        private void CheckKind(CoreWebView2ContextMenuItem item)
        {
            if (item.Kind == CoreWebView2ContextMenuItemKind.Submenu)
                return;

            throw new Exception("This item is not a submenu");
        }

        private ContextMenuItem GetRawItem(IContextMenuItem item)
        {
            ContextMenuItem rawItem = (ContextMenuItem)item;

            if (rawItem.Disposed)
                throw new InvalidOperationException("item is disposed");

            return rawItem;
        }

        #endregion

    }
}