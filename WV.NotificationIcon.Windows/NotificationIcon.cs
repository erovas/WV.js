using WV.WebView;
using WV.JavaScript;

namespace WV.NotificationIcon.Windows
{
    public class NotificationIcon : Plugin, INotificationIcon
    {
        public static string JScript => "";


        private const string BALLOONTIP_CLICKED = "balloontipclicked";
        private const string BALLOONTIP_CLOSED = "balloontipclosed";
        private const string BALLOONTIP_SHOWN = "balloontipshown";

        #region PRIVATE PROPERTIES

        private NotifyIcon InnerNotifyIcon { get; }

        private Function? InnerFNOnClick { get; set; }
        private Function? InnerFNOnDoubleClick { get; set; }

        private Function? InnerFNOnBalloonTipClicked { get; set; }
        private Function? InnerFNOnBalloonTipClosed { get; set; }
        private Function? InnerFNOnBalloonTipShown { get; set; }

        private List<Function> InnerFNClick { get; } = new List<Function>();
        private List<Function> InnerFNDoubleClick { get; } = new List<Function>();
        private List<Function> InnerFNBalloonTipClicked { get; } = new List<Function>();
        private List<Function> InnerFNBalloonTipClosed { get; } = new List<Function>();
        private List<Function> InnerFNBalloonTipShown { get; } = new List<Function>();

        #endregion

        #region PUBLIC PROPERTIES

        public bool Visible
        {
            get => this.InnerNotifyIcon.Visible;
            set => this.InnerNotifyIcon.Visible = value;
        }

        public string ToolTipText
        {
            get => this.InnerNotifyIcon.Text;
            set => this.InnerNotifyIcon.Text = value;
        }

        private string? _Icon;
        public string? Icon
        {
            get => _Icon;
            set
            {
                try
                {
                    this.InnerNotifyIcon.Icon = new Icon(value);
                    _Icon = value;
                }
                catch (Exception) 
                {
                    this.InnerNotifyIcon.Icon = SystemIcons.Application;
                }
            }
        }

        private INotificationMenu? _ContextMenu;
        public INotificationMenu? ContextMenu 
        {
            get => _ContextMenu;
            set
            {
                _ContextMenu = value;
                if(value != null) 
                    this.InnerNotifyIcon.ContextMenuStrip = ((NotificationMenu)value).InnerMenu;
                else
                    this.InnerNotifyIcon.ContextMenuStrip = null;
            }
        }

        #endregion

        public NotificationIcon(IWebView webView) : base(webView)
        {
            var menu = new NotifyIcon();
            this.InnerNotifyIcon = menu;
            AUX_ConfigMenu(menu);
        }

        #region PUBLIC METHODS

        public void ShowBallonTip(string title, string text, string icon, int timeout)
        {
            var menu = this.InnerNotifyIcon;
            menu.BalloonTipTitle = title;
            menu.BalloonTipText = text;

            if (Enum.TryParse(icon, out ToolTipIcon myStatus))
                menu.BalloonTipIcon = myStatus;
            else
                menu.BalloonTipIcon = ToolTipIcon.None;

            menu.ShowBalloonTip(timeout);
        }

        #endregion

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

                case BALLOONTIP_CLICKED:
                    this.InnerFNBalloonTipClicked.Add(listener);
                    break;

                case BALLOONTIP_CLOSED:
                    this.InnerFNBalloonTipClosed.Add(listener);
                    break;

                case BALLOONTIP_SHOWN:
                    this.InnerFNBalloonTipShown.Add(listener);
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
                BALLOONTIP_CLICKED => this.InnerFNBalloonTipClicked,
                BALLOONTIP_CLOSED => this.InnerFNBalloonTipClosed,
                BALLOONTIP_SHOWN => this.InnerFNBalloonTipShown,
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

        public Function? onballoontipclicked
        {
            get => this.InnerFNOnBalloonTipClicked;
            set
            {
                removeEventListener(BALLOONTIP_CLICKED, this.InnerFNOnBalloonTipClicked);
                addEventListener(BALLOONTIP_CLICKED, value);
                this.InnerFNOnBalloonTipClicked = value;
            }
        }

        public Function? onballoontipclosed
        {
            get => this.InnerFNOnBalloonTipClosed;
            set
            {
                removeEventListener(BALLOONTIP_CLOSED, this.InnerFNOnBalloonTipClosed);
                addEventListener(BALLOONTIP_CLOSED , value);
                this.InnerFNOnBalloonTipClosed = value;
            }
        }

        public Function? onballoontipshown
        {
            get => this.InnerFNOnBalloonTipShown;
            set
            {
                removeEventListener(BALLOONTIP_SHOWN, this.InnerFNOnBalloonTipShown);
                addEventListener(BALLOONTIP_SHOWN , value);
                this.InnerFNOnBalloonTipShown = value;
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void InnerNotifyIcon_Click(object? sender, EventArgs e)
        {
            CursorUtil cursor = new(e);

            foreach (var item in this.InnerFNClick)
                item.Execute(this, cursor.X, cursor.Y, cursor.Button);
        }

        private void InnerNotifyIcon_DoubleClick(object? sender, EventArgs e)
        {
            CursorUtil cursor = new(e);

            foreach (var item in this.InnerFNDoubleClick)
                item.Execute(this, cursor.X, cursor.Y, cursor.Button);
        }

        private void InnerNotifyIcon_BalloonTipClicked(object? sender, EventArgs e)
        {
            CursorUtil cursor = new(e);

            foreach (var item in this.InnerFNBalloonTipClicked)
                item.Execute(this, cursor.X, cursor.Y, cursor.Button);
        }

        private void InnerNotifyIcon_BalloonTipClosed(object? sender, EventArgs e)
        {
            CursorUtil cursor = new(e);

            foreach (var item in this.InnerFNBalloonTipClosed)
                item.Execute(this, cursor.X, cursor.Y, cursor.Button);
        }

        private void InnerNotifyIcon_BalloonTipShown(object? sender, EventArgs e)
        {
            CursorUtil cursor = new(e);

            foreach (var item in this.InnerFNBalloonTipShown)
                item.Execute(this, cursor.X, cursor.Y, cursor.Button);
        }

        #endregion

        #region HELPERS

        private void AUX_ConfigMenu(NotifyIcon menu)
        {
            menu.Click += InnerNotifyIcon_Click;
            menu.DoubleClick += InnerNotifyIcon_DoubleClick;

            menu.BalloonTipClicked += InnerNotifyIcon_BalloonTipClicked;
            menu.BalloonTipClosed += InnerNotifyIcon_BalloonTipClosed;
            menu.BalloonTipShown += InnerNotifyIcon_BalloonTipShown;

            menu.Visible = false;
            menu.Icon = SystemIcons.Application;
        }

        private void AUX_DisposeMenu(NotifyIcon menu)
        {
            menu.Click -= InnerNotifyIcon_Click;
            menu.DoubleClick -= InnerNotifyIcon_DoubleClick;

            menu.BalloonTipClicked -= InnerNotifyIcon_BalloonTipClicked;
            menu.BalloonTipClosed -= InnerNotifyIcon_BalloonTipClosed;
            menu.BalloonTipShown -= InnerNotifyIcon_BalloonTipShown;

            menu.ContextMenuStrip = null;
            menu.Visible = false;  //Para hacerlo invisible en la barra de tareas (soluciona bug visual de windows)
            menu.Dispose();

            this.InnerFNOnClick = null;
            this.InnerFNOnDoubleClick = null;
            this.InnerFNOnBalloonTipClicked = null;
            this.InnerFNOnBalloonTipClosed = null;
            this.InnerFNOnBalloonTipShown = null;

            this.InnerFNClick.Clear();
            this.InnerFNDoubleClick.Clear();
            this.InnerFNBalloonTipClicked.Clear();
            this.InnerFNBalloonTipClosed.Clear();
            this.InnerFNBalloonTipShown.Clear();
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (this.Disposed)
                return;

            if (disposing)
            {

            }

            AUX_DisposeMenu(this.InnerNotifyIcon);
        }
    
    }
}