using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HexEditor.MathModel
{
    public struct Index2D
    {
        private const int KEY_MULTIPLIER = 1000;

        public static implicit operator int(Index2D index2D)
        {
            return index2D.GetHashCode();
        }

        private int x;
        private int y;

        public int X { get { return x; } }
        public int Y { get { return y; } }

        public Index2D(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

       

        public override int GetHashCode()
        {
            return x * KEY_MULTIPLIER + y;
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override string ToString()
        {
            return "X = " + x + " Y = " + y;
        }
    }
}
