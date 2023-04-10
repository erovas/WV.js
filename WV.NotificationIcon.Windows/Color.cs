namespace WV.NotificationIcon.Windows
{
    public class Color : IColor
    {
        private ToolStrip? InnerMenu { get; }
        private ToolStripItem? InnerItem { get; }

        private bool IsForeColor { get; }
        private System.Drawing.Color InnerColor 
        {
            get
            {
                if(this.IsForeColor) 
                {
                    if (this.InnerMenu != null)
                        return this.InnerMenu.ForeColor;

                    return this.InnerItem.ForeColor;
                }

                if (this.InnerMenu != null)
                    return this.InnerMenu.BackColor;

                return this.InnerItem.BackColor;
            }
            set
            {
                if(this.IsForeColor)
                {
                    if (this.InnerMenu != null)
                        this.InnerMenu.ForeColor = value;
                    else
                        this.InnerItem.ForeColor = value;

                    return;
                }

                if (this.InnerMenu != null)
                    this.InnerMenu.BackColor = value;
                else
                    this.InnerItem.BackColor = value;
            }
        }

        #region PUBLIC PROPERTIES

        public byte R
        {
            get => this.InnerColor.R;
            set => this.InnerColor = System.Drawing.Color.FromArgb(this.A, value, this.G, this.B);
        }

        public byte G
        {
            get => this.InnerColor.G;
            set => this.InnerColor = System.Drawing.Color.FromArgb(this.A, this.R, value, this.B);
        }

        public byte B
        {
            get => this.InnerColor.B;
            set => this.InnerColor = System.Drawing.Color.FromArgb(this.A, this.R, this.G, value);
        }

        public byte A
        {
            get => this.InnerColor.A;
            set => this.InnerColor = System.Drawing.Color.FromArgb(value, this.R, this.G, this.B);
        }

        public bool IsEmpty => this.InnerColor.IsEmpty;

        public bool IsNamedColor => this.InnerColor.IsNamedColor;

        public bool IsSystemColor => this.InnerColor.IsSystemColor;

        public string Name 
        {
            get => this.InnerColor.Name;
            set
            {
                try
                {
                    this.InnerColor = System.Drawing.Color.FromName(value);
                }
                catch (Exception) { }
            }
        }
        public int ARGB 
        {
            get => this.InnerColor.ToArgb(); 
            set
            {
                try
                {
                    this.InnerColor = System.Drawing.Color.FromArgb(value);
                }
                catch (Exception) { }
            }
        }

        public float Brightness => this.InnerColor.GetBrightness();

        public float Hue => this.InnerColor.GetHue();

        public float Saturation => this.InnerColor.GetSaturation();

        #endregion

        public Color(ToolStrip menu, bool isForeColor = false)
        {
            this.InnerMenu = menu;
            this.IsForeColor = isForeColor;
        }

        public Color(ToolStripItem menu, bool isForeColor = false)
        {
            this.InnerItem = menu;
            this.IsForeColor = isForeColor;
        }

        public void SetARGB(int alpha, int red, int green, int blue)
        {
            this.InnerColor = System.Drawing.Color.FromArgb(alpha, red, green, blue);
        }

        public void SetRGB(int red, int green, int blue)
        {
            this.InnerColor = System.Drawing.Color.FromArgb(red, green, blue);
        }

    }
}