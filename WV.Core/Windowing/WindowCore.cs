using System.Reflection.Metadata;
using WV.Configs;
using WV.Enums;
using WV.Interfaces;
using static WV.App.Delegates;

namespace WV.Core.Windowing
{
    public abstract class WindowCore : Plugin, IWindow
    {
        #region Fields

        protected IRect? _Rect { get; private set; }

        #endregion

        #region Events

        public event WVEventHandler<WindowState, string>? StateChanged;
        public event WVEventHandler? Closing;
        public event WVEventHandler<int, int>? PositionChanged;
        public event WVEventHandler<bool>? Activated;
        public event WVEventHandler<bool>? EnabledEvent;
        public event WVEventHandler<bool>? Visible;
        public event WVEventHandler<int, int>? SizeChanged;
        public event WVRawEventHandler? Raw;

        #endregion

        public WindowCore(IPluginContext context, WindowConfig windowConfig) : base(context)
        {
            var txt = $"{nameof(WindowCore)} constructor";
            LogCalling(txt);
            try
            {
                _Rect = CreateRect(context, windowConfig.Rect);
                Initialize(context, windowConfig);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract IRect CreateRect(IPluginContext context, RectConfig rectConfig);

        protected abstract void Initialize(IPluginContext context, WindowConfig windowConfig);

        #region Properties

        public IRect Rect 
        { 
            get
            {
                var txt = nameof(Rect);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return _Rect!;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            } 
        }

        public WindowState State
        {
            get
            {
                var txt = nameof(State);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return StateCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(State)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    StateCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract WindowState StateCore { get; set; }

        public string StateText
        {
            get
            {
                var txt = nameof(StateText);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return StateTextCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(StateText)} = \"{value}\"";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    StateTextCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract string StateTextCore { get; set; }

        public string Title
        {
            get
            {
                var txt = nameof(Title);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return TitleCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Title)} = \"{value}\"";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    TitleCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract string TitleCore { get; set; }

        public bool TopMost
        { 
            get
            {
                var txt = nameof(TopMost);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return TopMostCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(TopMost)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    TopMostCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool TopMostCore { get; set; }

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

        public bool IsVisible
        {
            get
            {
                var txt = nameof(IsVisible);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return IsVisibleCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool IsVisibleCore { get; }

        public bool PreventClose
        {
            get
            {
                var txt = nameof(PreventClose);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return PreventCloseCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(PreventClose)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    PreventCloseCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool PreventCloseCore { get; set; }

        public bool IsActive
        {
            get
            {
                var txt = nameof(IsActive);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return IsActiveCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool IsActiveCore { get; }

        public bool AllowSnap
        {
            get
            {
                var txt = nameof(AllowSnap);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return AllowSnapCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(AllowSnap)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    AllowSnapCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool AllowSnapCore { get; set; }

        public bool ClickThrough
        {
            get
            {
                var txt = nameof(ClickThrough);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return ClickThroughCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(ClickThrough)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    ClickThroughCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract bool ClickThroughCore { get; set; }

        #endregion

        #region Methods

        public void Close()
        {
            var txt = $"{nameof(Close)}()";
            LogCalling(txt);
            try
            {
                CloseCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void CloseCore();

        public void Drag()
        {
            var txt = $"{nameof(Drag)}()";
            LogCalling(txt);
            try
            {
                DragCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void DragCore();

        public void Hide()
        {
            var txt = $"{nameof(Hide)}()";
            LogCalling(txt);
            try
            {
                HideCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void HideCore();

        public void Maximize()
        {
            var txt = $"{nameof(Maximize)}()";
            LogCalling(txt);
            try
            {
                MaximizeCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void MaximizeCore();

        public void Minimize()
        {
            var txt = $"{nameof(Minimize)}()";
            LogCalling(txt);
            try
            {
                MinimizeCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void MinimizeCore();

        public void Normalize()
        {
            var txt = $"{nameof(Normalize)}()";
            LogCalling(txt);
            try
            {
                NormalizeCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void NormalizeCore();

        public void ResizeBottom()
        {
            var txt = $"{nameof(ResizeBottom)}()";
            LogCalling(txt);
            try
            {
                ResizeBottomCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void ResizeBottomCore();

        public void ResizeBottomLeft()
        {
            var txt = $"{nameof(ResizeBottomLeft)}()";
            LogCalling(txt);
            try
            {
                ResizeBottomLeftCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void ResizeBottomLeftCore();

        public void ResizeBottomRight()
        {
            var txt = $"{nameof(ResizeBottomRight)}()";
            LogCalling(txt);
            try
            {
                ResizeBottomRightCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void ResizeBottomRightCore();

        public void ResizeLeft()
        {
            var txt = $"{nameof(ResizeLeft)}()";
            LogCalling(txt);
            try
            {
                ResizeLeftCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void ResizeLeftCore();

        public void ResizeRight()
        {
            var txt = $"{nameof(ResizeRight)}()";
            LogCalling(txt);
            try
            {
                ResizeRightCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void ResizeRightCore();

        public void ResizeTop()
        {
            var txt = $"{nameof(ResizeTop)}()";
            LogCalling(txt);
            try
            {
                ResizeTopCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void ResizeTopCore();

        public void ResizeTopLeft()
        {
            var txt = $"{nameof(ResizeTopLeft)}()";
            LogCalling(txt);
            try
            {
                ResizeTopLeftCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void ResizeTopLeftCore();

        public void ResizeTopRight()
        {
            var txt = $"{nameof(ResizeTopRight)}()";
            LogCalling(txt);
            try
            {
                ResizeTopRightCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void ResizeTopRightCore();

        public void Restore()
        {
            var txt = $"{nameof(Restore)}()";
            LogCalling(txt);
            try
            {
                RestoreCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void RestoreCore();

        public void Show()
        {
            var txt = $"{nameof(Show)}()";
            LogCalling(txt);
            try
            {
                ShowCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void ShowCore();

        public void ShowBehind()
        {
            var txt = $"{nameof(ShowBehind)}()";
            LogCalling(txt);
            try
            {
                ShowBehindCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void ShowBehindCore();

        public void ToCenter()
        {
            var txt = $"{nameof(ToCenter)}()";
            LogCalling(txt);
            try
            {
                ToCenterCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void ToCenterCore();

        #endregion

        protected sealed override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Rect?.Dispose();
                _Rect = null;
            }
            DisposeCore(disposing);
        }

        protected abstract void DisposeCore(bool disposing);

        protected virtual void FireStateChangedEvent(WindowState value, string text)
        {
            if (this.Disposed)
                return;

            this.StateChanged?.Invoke(WebView, value, text);
        }

        protected virtual void FireCloseEvent()
        {
            if (this.Disposed)
                return;

            this.Closing?.Invoke(WebView);
        }

        protected virtual void FirePositionChangedEvent(int x, int y)
        {
            if (this.Disposed)
                return;

            this.PositionChanged?.Invoke(WebView, x, y);
        }

        protected virtual void FireActivatedEvent(bool active)
        {
            if (this.Disposed)
                return;

            this.Activated?.Invoke(WebView, active);
        }

        protected virtual void FireEnabledEvent(bool enabled)
        {
            if (this.Disposed)
                return;

            this.EnabledEvent?.Invoke(WebView, enabled);
        }

        protected virtual void FireVisibleEvent(bool visible)
        {
            if (this.Disposed)
                return;

            this.Visible?.Invoke(WebView, visible);
        }

        protected virtual void FireSizeChangedEvent(int width, int heigth)
        {
            if (this.Disposed)
                return;

            this.SizeChanged?.Invoke(WebView, width, heigth);
        }

        protected virtual bool FireRawEvent(params object[] paramss)
        {
            bool handled = false;
            this.Raw?.Invoke(WebView, paramss, ref handled);
            return handled;
        }

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