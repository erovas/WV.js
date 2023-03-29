using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WV.WebView;

namespace WV.Windows.Webview
{
    public class Area : IArea
    {
        public int Top { get; }
        public int Bottom { get; }
        public int Left { get; }
        public int Right { get; }
        public int Width { get; }
        public int Height { get; }

        public Area(Rectangle rt)
        {
            Top = rt.Top;
            Bottom = rt.Bottom;
            Left = rt.Left;
            Right = rt.Right;
            Width = rt.Width;
            Height = rt.Height;
        }
    }
}