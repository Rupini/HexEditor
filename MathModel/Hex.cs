using HexEditor.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HexEditor.MathModel
{
    public class Hex
    {
        #region Definition

        private const int DEFAULT_HEIGHT = 1;

        private Index2D index;

        private double r;
        private double a;
        private int height;

        private HexType type;

        private HexRender render;
        #endregion
        #region Properies
        public Index2D Index { get { return index; } }

        public HexType Type { get { return type; } set { type = value; } }

        #endregion
        #region Constructor
        public Hex(Index2D inx) :
            this(inx, new Point(), HexType.Empty, DEFAULT_HEIGHT)
        {
        }

        public Hex(Index2D inx, Point center,  HexType type, int height)
        {
            r = GlobalConst.DEFAULT_r;
            a = 2d / Math.Sqrt(3) * r;
            index = inx;
            this.type = type;
            this.height = height;

            if (type != HexType.Empty)
                render = new HexRender(DisplayType.GAME_SimpleGround, height, center);
        }

        #endregion
        #region Methods

        public HexRender GetRenderHex()
        {
            return render;
        }

        #region Object_Override
        public override bool Equals(object obj)
        {
            return index.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return index.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
        #endregion
        #endregion

    }
}
