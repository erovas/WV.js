namespace WV.NotificationIcon.Windows
{
    internal class CursorUtil
    {
        public int X { get; }
        public int Y { get; }
        public int Button { get; } = -1;

        public CursorUtil(EventArgs e)
        {
            try
            {
                MouseEventArgs mouse = (MouseEventArgs)e;

                switch (mouse.Button)
                {
                    case MouseButtons.Left:
                        Button = 0;
                        break;
                    case MouseButtons.Right:
                        Button = 2;
                        break;
                    case MouseButtons.Middle:
                        Button = 1;
                        break;
                    case MouseButtons.XButton1:
                        Button = 3;
                        break;
                    case MouseButtons.XButton2:
                        Button = 4;
                        break;
                }
            }
            catch (Exception) { }

            this.X = Cursor.Position.X;
            this.Y = Cursor.Position.Y;
        }
    }
}
