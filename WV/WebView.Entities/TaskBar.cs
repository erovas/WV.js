using WV.WebView.Enums;

namespace WV.WebView.Entities
{
    public class TaskBar
    {
        public bool Visible { get; set; } = true;

        public TaskBarStatus Status { get; set; } = TaskBarStatus.Normal;

        public int Progress { get; set; } = 0;
    }
}
