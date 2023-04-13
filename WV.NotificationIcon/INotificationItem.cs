using WV.JavaScript;

namespace WV.NotificationIcon
{
    public interface INotificationItem
    {
        string Image { get; set; }
        string Text { get; set; }
        string ToolTipText { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        string TextDirection { get; set; }
        string TextAlign { get; set; }
        bool CheckOnClick { get; set; }
        bool Checked { get; set; }
        bool Enabled { get; set; }
        string AccessibleDefaultActionDescription { get; set; }
        string AccessibleDescription { get; set; }
        string AccessibleName { get; set; }
        bool Selected { get; }
        IColor BackColor { get; }
        IColor ForeColor { get; }

        void addEventListener(string type, Function? listener, params object[] options);
        void removeEventListener(string type, Function? listener, params object[] options);
        Function? onclick { get; set; }
        Function? ondoubleclick { get; set; }
        Function? oncheckedchanged { get; set; }
    }
}
