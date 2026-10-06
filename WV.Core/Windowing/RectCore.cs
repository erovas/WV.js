using System;
using System.Collections.Generic;
using System.Text;
using WV.Configs;
using WV.Interfaces;

namespace WV.Core.Windowing
{
    public abstract class RectCore : Plugin, IRect
    {
        protected RectCore(IPluginContext context, RectConfig rectConfig) : base(context)
        {
            var txt = $"{nameof(RectCore)} constructor";
            LogCalling(txt);
            try
            {
                Initialize(context, rectConfig);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void Initialize(IPluginContext context, RectConfig rectConfig);

        #region Properties

        public int X
        {
            get
            {
                var txt = nameof(X);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return XCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(X)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    XCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract int XCore { get; set; }

        public int Y
        {
            get
            {
                var txt = nameof(Y);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return YCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Y)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    YCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract int YCore { get; set; }

        public int Width
        {
            get
            {
                var txt = nameof(Width);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return WidthCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Width)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    WidthCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract int WidthCore { get; set; }

        public int Height
        {
            get
            {
                var txt = nameof(Height);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return HeightCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(Height)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    HeightCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract int HeightCore { get; set; }

        public int MaxWidth
        {
            get
            {
                var txt = nameof(MaxWidth);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return MaxWidthCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(MaxWidth)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    MaxWidthCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract int MaxWidthCore { get; set; }

        public int MaxHeight
        {
            get
            {
                var txt = nameof(MaxHeight);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return MaxHeightCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(MaxHeight)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    MaxHeightCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract int MaxHeightCore { get; set; }

        public int MinWidth
        {
            get
            {
                var txt = nameof(MinWidth);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return MinWidthCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(MinWidth)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    MinWidthCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract int MinWidthCore { get; set; }

        public int MinHeight
        {
            get
            {
                var txt = nameof(MinHeight);
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    return MinHeightCore;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
            set
            {
                var txt = $"{nameof(MinHeight)} = {value}";
                LogCalling(txt, true);
                try
                {
                    ThrowIfDisposed();
                    MinHeightCore = value;
                }
                catch (Exception ex)
                {
                    LogFail(txt, ex, true);
                    throw;
                }
            }
        }

        protected abstract int MinHeightCore { get; set; }

        #endregion

        #region Methods

        public int[] GetPosition()
        {
            var txt = $"{nameof(GetPosition)}()";
            LogCalling(txt);
            try
            {
                return GetPositionCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract int[] GetPositionCore();

        public int[] GetPositionAndSize()
        {
            var txt = $"{nameof(GetPositionAndSize)}()";
            LogCalling(txt);
            try
            {
                return GetPositionAndSizeCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract int[] GetPositionAndSizeCore();

        public int[] GetSize()
        {
            var txt = $"{nameof(GetSize)}()";
            LogCalling(txt);
            try
            {
                return GetSizeCore();
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract int[] GetSizeCore();

        public void SetPosition(int x, int y)
        {
            var txt = $"{nameof(SetPosition)}({x}, {y})";
            LogCalling(txt);
            try
            {
                SetPositionCore(x, y);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void SetPositionCore(int x, int y);

        public void SetPositionAndSize(int x, int y, int width, int height)
        {
            var txt = $"{nameof(SetPositionAndSize)}({x}, {y}, {width}, {height})";
            LogCalling(txt);
            try
            {
                SetPositionAndSizeCore(x, y, width, height);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void SetPositionAndSizeCore(int x, int y, int width, int height);

        public void SetSize(int width, int height)
        {
            var txt = $"{nameof(SetSize)}({width}, {height})";
            LogCalling(txt);
            try
            {
                SetSizeCore(width, height);
            }
            catch (Exception ex)
            {
                LogFail(txt, ex);
                throw;
            }
        }

        protected abstract void SetSizeCore(int width, int height);

        #endregion

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