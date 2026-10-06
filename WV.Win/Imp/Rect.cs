using System.Runtime.InteropServices;
using WV.Configs;
using WV.Core.Windowing;
using WV.Enums;
using WV.Interfaces;
using WV.Win.Win32;
using WV.Win.Win32.Structs;

namespace WV.Win.Imp
{
    public sealed class Rect : RectCore
    {
        #region Fields

        private Window? _Win;

        #endregion

        private WebView WV => (WebView)this.WebView;
        private Window Win => _Win!;
        private readonly RectConfig _Config;

        public Rect(IPluginContext context, RectConfig rectConfig, Window win) : base(context, rectConfig)
        {
            _Win = win;
            _Config = rectConfig;
        }

        protected override void Initialize(IPluginContext context, RectConfig rectConfig)
        {
            if(_Win != null)
                SetUpRect(this, rectConfig);
        }

        //-------------------------------------------//

        #region Position

        protected override int XCore
        {
            get 
            {
                return Utils32.GetRECT(WV).X;
            } 
            set
            {
                if(this.Win.State == WindowState.Minimized)
                    return;

                RECT rect = Utils32.GetRECT(this.WV);

                if (rect.X == value)
                    return;

                this.Win._PreventPositionEvent = true;
                Utils32.SetRECT(WV, value, rect.Y, rect.Width, rect.Height);
                this.Win._PreventPositionEvent = false;
            }
        }

        protected override int YCore
        {
            get 
            {
                return Utils32.GetRECT(WV).Y;
            } 
            set
            {
                if (this.Win.State == WindowState.Minimized)
                    return;

                RECT rect = Utils32.GetRECT(this.WV);

                if (rect.Y == value)
                    return;

                this.Win._PreventPositionEvent = true;
                Utils32.SetRECT(WV, rect.X, value, rect.Width, rect.Height);
                this.Win._PreventPositionEvent = false;
            }
        }

        #endregion

        //-------------------------------------------//

        #region Size

        protected override int WidthCore
        {
            get
            {
                return Utils32.GetRECT(WV).Width;
            }
            set
            {
                if (this.Win.State == WindowState.Minimized)
                    return;

                RECT rect = Utils32.GetRECT(this.WV);

                if (rect.Width == value)
                    return;

                if (value > MaxWidth)
                    value = MaxWidth;
                else if (value < MinWidth)
                    value = MinWidth;

                this.Win._PreventSizeEvent = true;
                Utils32.SetRECT(WV, rect.X, rect.Y, value, rect.Height);
                this.Win._PreventSizeEvent = false;
            }
        }

        protected override int HeightCore
        {
            get
            {       
                // La ventana que contiene el WebView es 1 pixel mas bajo cuando está maximizado
                return Utils32.GetRECT(WV).Height;
            }
            set
            {
                if (this.Win.State == WindowState.Minimized)
                    return;

                RECT rect = Utils32.GetRECT(this.WV);

                if (rect.Height == value)
                    return;

                if (value > MaxHeight)
                    value = MaxHeight;
                else if (value < MinHeight)
                    value = MinHeight;

                this.Win._PreventSizeEvent = true;
                Utils32.SetRECT(WV, rect.X, rect.Y, rect.Width, value);
                this.Win._PreventSizeEvent = false;
            }
        }

        #endregion

        //-------------------------------------------//

        #region Max Size

        private int _MaxWidth = App.Window.Rect.MaxWidth;
        private int _MaxHeight = App.Window.Rect.MaxHeight;

        protected override int MaxWidthCore
        {
            get 
            { 
                return _MaxWidth; 
            }
            set
            {
                if (this.Win.State == WindowState.Minimized)
                    return;

                if (value < this.MinWidth)
                    value = this.MinWidth;

                _MaxWidth = value;

                if (this.MaxWidth < this.Width)
                    this.Width = this.MaxWidth;
            }
        }

        protected override int MaxHeightCore
        {
            get 
            { 
                return _MaxHeight; 
            }
            set
            {
                if (this.Win.State == WindowState.Minimized)
                    return;

                if (value < this.MinHeight)
                    value = this.MinHeight;

                _MaxHeight = value;

                if (this.MaxHeight < this.Height)
                    this.Height = this.MaxHeight;
            }
        }

        #endregion

        //-------------------------------------------//

        #region Min Size

        private const int LowMinWidth = App.Window.Rect.MinWidth;
        private const int LowMinHeight = App.Window.Rect.MinHeight;

        private int _MinWidth = LowMinWidth;
        private int _MinHeight = LowMinHeight;
        protected override int MinWidthCore
        {
            get 
            { 
                return _MinWidth; 
            }
            set
            {
                if (this.Win.State == WindowState.Minimized)
                    return;

                if (value < LowMinWidth)
                    value = LowMinWidth;

                _MinWidth = value;

                if (this.MinWidth > this.MaxWidth)
                    this.MaxWidth = this.MinWidth;

                if (this.MinWidth > this.Width)
                    this.Width = this.MinWidth;
            }
        }

        protected override int MinHeightCore
        {
            get 
            { 
                return _MinHeight; 
            }
            set
            {
                if (this.Win.State == WindowState.Minimized)
                    return;

                if (value < LowMinHeight)
                    value = LowMinHeight;

                _MinHeight = value;

                if (this.MinHeight > this.MaxHeight)
                    this.MaxHeight = this.MinHeight;

                if (this.MinHeight > this.Height)
                    this.Height = this.MinHeight;
            }
        }

        #endregion

        //-------------------------------------------//

        #region METHODS

        protected override void SetSizeCore(int width, int height)
        {
            if (this.Win.State == WindowState.Minimized)
                    return;

            RECT rect = Utils32.GetRECT(this.WV);

            if (rect.Width == width && rect.Height == height)
                return;

            if (width > this.MaxWidth)
                width = this.MaxWidth;
            else if (width < this.MinWidth)
                width = this.MinWidth;

            if (height > this.MaxHeight)
                height = this.MaxHeight;
            else if (height < this.MinHeight)
                height = this.MinHeight;

            this.Win._PreventSizeEvent = true;
            Utils32.SetRECT(WV, rect.X, rect.Y, width, height);
            this.Win._PreventSizeEvent = false;
        }

        protected override int[] GetSizeCore()
        {
            RECT rect = Utils32.GetRECT(this.WV);
            return [rect.Width, rect.Height];
        }

        protected override void SetPositionCore(int x, int y)
        {
            if (this.Win.State == WindowState.Minimized)
                return;

            RECT rect = Utils32.GetRECT(this.WV);

            if (rect.X == x && rect.Y == y)
                return;

            this.Win._PreventPositionEvent = true;
            Utils32.SetRECT(this.WV, x, y, rect.Width, rect.Height);
            this.Win._PreventPositionEvent = false;
        }

        protected override int[] GetPositionCore()
        {
            RECT rect = Utils32.GetRECT(this.WV);
            return [rect.X, rect.Y];
        }

        protected override void SetPositionAndSizeCore(int x, int y, int width, int height)
        {
            if (width > this.MaxWidthCore)
                width = this.MaxWidthCore;
            else if (width < this.MinWidthCore)
                width = this.MinWidthCore;

            if (height > this.MaxHeightCore)
                height = this.MaxHeightCore;
            else if (height < this.MinHeightCore)
                height = this.MinHeightCore;

            this.Win._PreventSizeEvent = true;
            this.Win._PreventPositionEvent = true;
            Utils32.SetRECT(this.WV, x, y, width, height);
            this.Win._PreventSizeEvent = false;
            this.Win._PreventPositionEvent = false;
        }

        protected override int[] GetPositionAndSizeCore()
        {
            RECT rect = Utils32.GetRECT(this.WV);
            return [rect.X, rect.Y, rect.Width, rect.Height];
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _Win = null;
        }

        internal void ToDefault()
        {
            SetUpRect(this, _Config);
        }

        private static void SetUpRect(Rect rect, RectConfig config)
        {
            rect.XCore = config.X;
            rect.YCore = config.Y;

            rect.MinWidth = config.MinWidth;
            rect.MinHeight = config.MinHeight;

            rect.MaxWidth = config.MaxWidth;
            rect.MaxHeight = config.MaxHeight;
        }
    }
}