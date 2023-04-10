using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WV.WebView;

namespace WV.NotificationIcon.Windows
{
    public class NotificationMenu : Plugin, INotificationMenu
    {
        [ComVisible(false)]
        public ContextMenuStrip InnerMenu { get; }

        #region PUBLIC PROPERTIES

        public double Opacity
        {
            get => this.InnerMenu.Opacity;
            set => this.InnerMenu.Opacity = value;
        }

        public IColor BackColor { get; }

        public IColor ForeColor { get; }

        #endregion

        public NotificationMenu(IWebView webView, ContextMenuStrip menu) : base(webView)
        {
            this.InnerMenu = menu;
            this.BackColor = new Color(menu);
            this.ForeColor = new Color(menu, true);
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }
    }
}
