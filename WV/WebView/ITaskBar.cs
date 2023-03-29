using WV.WebView.Enums;

namespace WV.WebView
{
    public interface ITaskBar
    {
        /// <summary>
        /// 
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// 
        /// </summary>
        TaskBarStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int Progress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        void Flash();
    }
}
