using WV.Interfaces;

namespace WV.Core.Browsering
{
    public abstract class ContextMenuItemCore : Plugin, IContextMenuItem
    {
        #region Fields

        protected IContextMenuItem? _parent;
        protected readonly List<IContextMenuItem> _children;
        private readonly IContextMenuItemList _ChildrenList;

        #endregion

        protected ContextMenuItemCore(IPluginContext context, string kind, string label, string icon) : base(context)
        {
            var txt = $"{nameof(ContextMenuItemCore)} constructor";
            LogCalling(txt);
            try
            {
                _children = new List<IContextMenuItem>();
                _ChildrenList = new ContextMenuItemList(_children);
                Initialize(context, kind, label, icon);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void Initialize(IPluginContext context, string kind, string label, string icon);

        #region Properties

        public string Label
        {
            get
            {
                var txt = nameof(Label);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return LabelCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Label)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    LabelCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract string LabelCore { get; set; }

        public string Kind
        {
            get
            {
                var txt = nameof(Kind);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return KindCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Kind)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    KindCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract string KindCore { get; set; }

        public string Icon
        {
            get
            {
                var txt = nameof(Icon);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return IconCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Icon)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    IconCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract string IconCore { get; set; }

        public IContextMenuItem? Parent
        {
            get
            {
                var txt = nameof(Parent);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return _parent;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        public IContextMenuItemList Children
        {
            get
            {
                var txt = nameof(Children);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return _ChildrenList;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        public bool Checked
        {
            get
            {
                var txt = nameof(Checked);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CheckedCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Checked)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    CheckedCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool CheckedCore { get; set; }

        public bool Enabled
        {
            get
            {
                var txt = nameof(Enabled);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return EnabledCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Enabled)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    EnabledCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool EnabledCore { get; set; }

        public bool Visible
        {
            get
            {
                var txt = nameof(Visible);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return VisibleCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Visible)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    VisibleCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool VisibleCore { get; set; }

        public object? Callback
        {
            get
            {
                var txt = nameof(Callback);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return CallbackCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Callback)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    CallbackCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract object? CallbackCore { get; set; }

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

        public void RemoveItem(IContextMenuItem item)
        {
            var txt = $"{nameof(RemoveItem)}({item})";
            LogCalling(txt);
            try
            {
                RemoveItemCore(item);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void RemoveItemCore(IContextMenuItem item);

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
                foreach (var item in _children)
                    item.Dispose();
                
                _children.Clear();
                _parent = null;
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