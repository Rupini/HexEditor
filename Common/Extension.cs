using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HexEditor
{
    public static class Extension
    {
        public static Point Minus(this Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point Minus(this Point p1, double x, double y)
        {
            return new Point(p1.X - x, p1.Y - y);
        }

        public static Point Plus(this Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point Plus(this Point p1, double x, double y)
        {
            return new Point(p1.X + x, p1.Y + y);
        }

        public static Point Inc(this Point p1, double k)
        {
            return new Point(p1.X * k, p1.Y * k);
        }

        public static double Abs(this Point p)
        {
            return Math.Sqrt(p.X * p.X + p.Y * p.Y);
        }

        public static Point Round(this Point p, int digits)
        {
            return new Point(Math.Round(p.X, digits), Math.Round(p.Y, digits));
        }
    }
}
