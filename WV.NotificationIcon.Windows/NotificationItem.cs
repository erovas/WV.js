using System.Runtime.InteropServices;
using WV.WebView;
using WV.JavaScript;

namespace WV.NotificationIcon.Windows
{
    public class NotificationItem : Plugin, INotificationItem
    {

        #region PRIVATE PROPERTIES

        [ComVisible(false)]
        public ToolStripMenuItem InnerItem { get; }

        private Function? InnerFNOnClick { get; set; }
        private Function? InnerFNOnDoubleClick { get; set; }
        private Function? InnerFNOnChecked { get; set; }

        private List<Function> InnerFNClick { get; } = new List<Function>();
        private List<Function> InnerFNDoubleClick { get; } = new List<Function>();
        private List<Function> InnerFNChecked { get; } = new List<Function>();

        #endregion

        #region PUBLIC PROPERTIES

        private string? _Image;
        public string? Image
        {
            get => _Image;
            set
            {
                try
                {
                    this.InnerItem.Image = System.Drawing.Image.FromFile(value);
                    _Image = value;
                }
                catch (Exception) { }
            }
        }
        public string Text
        {
            get => this.InnerItem.Text;
            set => this.InnerItem.Text = value;
        }
        public string ToolTipText
        {
            get => this.InnerItem.ToolTipText; 
            set => this.InnerItem.ToolTipText = value;
        }
        public int Width
        {
            get => this.InnerItem.Width; 
            set => this.InnerItem.Width = value;
        }
        public int Height
        {
            get => this.InnerItem.Height;
            set => this.InnerItem.Height = value;
        }
        public string TextDirection
        {
            get => this.InnerItem.TextDirection.ToString();
            set
            {
                if(Enum.TryParse(value, out ToolStripTextDirection direction))
                    this.InnerItem.TextDirection = direction;
            }
        }
        public string TextAlign
        {
            get => this.InnerItem.TextAlign.ToString();
            set
            {
                if(Enum.TryParse(value, out ContentAlignment align))
                    this.InnerItem.TextAlign = align;
            }
        }
        public bool CheckOnClick
        {
            get => this.InnerItem.CheckOnClick;
            set => this.InnerItem.CheckOnClick = value;
        }
        public bool Checked
        {
            get => this.InnerItem.Checked;
            set => this.InnerItem.Checked = value;
        }
        public bool Enabled
        {
            get => this.InnerItem.Enabled; 
            set => this.InnerItem.Enabled = value;
        }
        public string AccessibleDefaultActionDescription
        {
            get => this.InnerItem.AccessibleDefaultActionDescription; 
            set => this.InnerItem.AccessibleDefaultActionDescription = value;
        }
        public string AccessibleDescription
        {
            get => this.InnerItem.AccessibleDescription; 
            set => this.InnerItem.AccessibleDescription = value;
        }
        public string AccessibleName
        {
            get => this.InnerItem.AccessibleName; 
            set => this.InnerItem.AccessibleName = value;
        }
        public bool Selected => this.InnerItem.Selected;
        public IColor BackColor { get; }
        public IColor ForeColor { get; }

        #endregion


        public NotificationItem(IWebView webView) : base(webView) 
        {
            ToolStripMenuItem item = new();

            this.InnerItem = item;
            this.BackColor = new Color(item);
            this.ForeColor = new Color(item, true);
            AUX_ConfigItem(item);
            
            //item.Alignment    get; set; enum
            
            //item.AllowDrop = true;        get; set;
            //item.Anchor = AnchorStyles.None;      get; set;
            //item.AutoSize = true;
            
            item.CheckState = CheckState.Unchecked;
            //item.DoubleClickEnabled = false;
            
            
            item.ShowShortcutKeys = true;
            //item.ShortcutKeys = Keys.NumPad1;
            
            //item.Name = string.Empty;            
        }

        #region EVENTS

        public void addEventListener(string type, Function? listener, params object[] options)
        {
            if (listener == null)
                return;

            type = (type + "").ToLower();

            switch (type)
            {
                case Utils.CLICK:
                    this.InnerFNClick.Add(listener);
                    break;

                case Utils.DOUBLE_CLICK:
                    this.InnerFNDoubleClick.Add(listener);
                    break;

                case Utils.CHECKED_CHANGED:
                    this.InnerFNChecked.Add(listener);
                    break;

                default:
                    throw new Exception("Event ['" + type + "'] not exists");
            }
        }

        public void removeEventListener(string type, Function? listener, params object[] options)
        {
            if (listener == null)
                return;

            type = (type + "").ToLower();
            List<Function> list = type switch
            {
                Utils.CLICK => this.InnerFNClick,
                Utils.DOUBLE_CLICK => this.InnerFNDoubleClick,
                Utils.CHECKED_CHANGED => this.InnerFNChecked,
                _ => throw new Exception("Event ['" + type + "'] not exists"),
            };
            Utils.RemoveInList(list, listener);
        }

        public Function? onclick
        {
            get => this.InnerFNOnClick;
            set
            {
                removeEventListener(Utils.CLICK, this.InnerFNOnClick);
                addEventListener(Utils.CLICK, value);
                this.InnerFNOnClick = value;
            }
        }

        public Function? ondoubleclick
        {
            get => this.InnerFNOnDoubleClick;
            set
            {
                removeEventListener(Utils.DOUBLE_CLICK, this.InnerFNOnDoubleClick);
                addEventListener(Utils.DOUBLE_CLICK, value);
                this.InnerFNOnDoubleClick = value;
            }
        }

        public Function? oncheckedchanged
        {
            get => this.InnerFNOnChecked;
            set
            {
                removeEventListener(Utils.CHECKED_CHANGED, this.InnerFNOnChecked);
                addEventListener(Utils.CHECKED_CHANGED, value);
                this.InnerFNOnChecked = value;
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void Item_Click(object? sender, EventArgs e)
        {
            CursorUtil cursor = new(e);

            foreach (var item in this.InnerFNClick)
                item.Execute(this, cursor.X, cursor.Y, cursor.Button);
        }

        private void Item_DoubleClick(object? sender, EventArgs e)
        {
            CursorUtil cursor = new(e);

            foreach (var item in this.InnerFNDoubleClick)
                item.Execute(this, cursor.X, cursor.Y, cursor.Button);
        }

        private void Item_CheckedChanged(object? sender, EventArgs e)
        {
            CursorUtil cursor = new(e);

            foreach (var item in this.InnerFNChecked)
                item.Execute(this, cursor.X, cursor.Y, cursor.Button);
        }

        #endregion

        #region HELPERS

        private void AUX_ConfigItem(ToolStripMenuItem item)
        {
            item.Click += Item_Click;
            item.DoubleClick += Item_DoubleClick;

            item.CheckedChanged += Item_CheckedChanged;

        }

        private void AUX_DisposeItem(ToolStripMenuItem item)
        {
            item.Click -= Item_Click;
            item.DoubleClick -= Item_DoubleClick;

            item.CheckedChanged -= Item_CheckedChanged;

            //item.DropDownItems.Clear();
            item.Visible = false;  //Para hacerlo invisible en la barra de tareas (soluciona bug visual de windows)
            item.Dispose();

            this.InnerFNOnClick = null;
            this.InnerFNOnDoubleClick = null;

            this.InnerFNClick.Clear();
            this.InnerFNDoubleClick.Clear();
        }


        #endregion

        protected override void Dispose(bool disposing)
        {
            if(this.Disposed)
                return;

            if (disposing)
            {

            }

            AUX_DisposeItem(this.InnerItem);
        }
    
    }
}