using System.Collections.Concurrent;
using WV.Win.Win32.Enums;

namespace WV.Win.Win32
{
    //https://github.com/rgwood/MaximalWebView/blob/main/MaximalWebView/UiThreadSynchronizationContext.cs
    //https://devblogs.microsoft.com/dotnet/await-synchronizationcontext-and-console-apps/
    public sealed class UIThreadSyncCtx : SynchronizationContext
    {
        public const uint WM_SYNCHRONIZATIONCONTEXT_WORK_AVAILABLE = (uint)WinMsg.WM_USER + 1;

        private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object?>> m_queue = new();
        private readonly IntPtr hwnd;

        public UIThreadSyncCtx(IntPtr hwnd) : base()
        {
            this.hwnd = hwnd;
        }

        public override void Post(SendOrPostCallback d, object? state)
        {
            m_queue.Add(new KeyValuePair<SendOrPostCallback, object?>(d, state));
            _ = User32.PostMessage(hwnd, WM_SYNCHRONIZATIONCONTEXT_WORK_AVAILABLE, 0, 0);
        }

        public override void Send(SendOrPostCallback d, object? state)
        {
            m_queue.Add(new KeyValuePair<SendOrPostCallback, object?>(d, state));
            _ = User32.SendMessage(hwnd, WM_SYNCHRONIZATIONCONTEXT_WORK_AVAILABLE, 0, 0);
        }

        public void RunAvailableWorkOnCurrentThread()
        {
            while (m_queue.TryTake(out KeyValuePair<SendOrPostCallback, object?> workItem))
                workItem.Key(workItem.Value);
        }

        public void Complete()
        {
            m_queue.CompleteAdding();
        }
    }
}