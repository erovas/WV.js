using WV.WebView;
using WV.WebView.Enums;

namespace Essential
{
    public class TaskBar
    {
        private ITaskBar WVTaskBar { get; }

        public TaskBar(ITaskBar taskBar) 
        { 
            this.WVTaskBar = taskBar;
        }

        public bool Visible
        {
            get => WVTaskBar.Visible;
            set => WVTaskBar.Visible = value;
        }
        public string Status
        {
            get => WVTaskBar.Status.ToString();
            set
            {
                if (Enum.TryParse(value, out TaskBarStatus myStatus))
                    WVTaskBar.Status = myStatus;
            }
        }
        public int Progress
        {
            get => WVTaskBar.Progress;
            set => WVTaskBar.Progress = value;
        }

        public void Flash()
        {
            WVTaskBar.Flash();
        }
    }
}
