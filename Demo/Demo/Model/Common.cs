using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo.Model
{
    class Common
    {
    }
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public Rect(int l, int t, int r, int b)
        {
            Left = l;
            Top = t;
            Right = r;
            Bottom = b;
        }
        public string getRectStr()
        {
            return Left.ToString() + "|" + Top.ToString() + "|" + Right.ToString() + "|" + Bottom.ToString() + "|";
        }
    }
}
