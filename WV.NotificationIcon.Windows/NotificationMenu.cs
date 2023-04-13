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

            menu.AllowTransparency = true;
            menu.AutoClose = true;
            menu.BackgroundImage = null;
            int bottom = menu.Bottom;
            //menu.Bounds
            menu.Click += Menu_Click;
            menu.DoubleClick += Menu_DoubleClick;
            menu.Enabled = true;
            menu.Height = bottom;
            //menu.Items
            menu.Left = 0;
            //menu.Right;
            //menu.Opacity 
            menu.Visible = true;
            
        }

        private void Menu_DoubleClick(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Menu_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }
    }
}
