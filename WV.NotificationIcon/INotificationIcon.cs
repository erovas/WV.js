using WV.JavaScript;
using WV.WebView;

namespace WV.NotificationIcon
{
    public interface INotificationIcon : IPlugin
    {
        bool Visible { get; set; }
        string ToolTipText { get; set; }
        string? Icon { get; set; }
        INotificationMenu? ContextMenu { get; set; }

        void ShowBallonTip(string title, string text, string icon, int timeout);

        void addEventListener(string type, Function? listener, params object[] options);
        void removeEventListener(string type, Function? listener, params object[] options);

        Function? onclick { get; set; }
        Function? ondoubleclick { get; set; }
        Function? onballoontipclicked { get; set; }
        Function? onballoontipclosed { get; set; }
        Function? onballoontipshown { get; set; }
    }
}