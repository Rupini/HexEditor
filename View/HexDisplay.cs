using HexEditor.MathModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HexEditor.View
{
    public class HexDisplay
    {
        public static HexDisplay Get(DisplayType type)
        {
            switch(type)
            {
                case DisplayType.FUNC_Selectable:
                    return selectable;
                case DisplayType.FUNC_Selected:
                    return selected;
                default:
                    return unknown;
            }
        }


        //Functional
        private static readonly HexDisplay selectable = new HexDisplay(Colors.Aquamarine, 0.5, FillType.Entire);
        private static readonly HexDisplay selected = new HexDisplay(Colors.Gold, 0.85, FillType.Contour);
        private static readonly HexDisplay unknown = new HexDisplay(Colors.White, 0, FillType.Entire);
        //Game



        private Color color;
        private double opacity;
        private FillType fillType;

        public Color Color { get { return color; } }
        public double Opacity { get { return opacity; } }
        public FillType FillType { get { return fillType; } }

        public HexDisplay(Color color, double opacity, FillType fillType)
        {
            this.color = color;
            this.opacity = opacity;
            this.fillType = fillType;
        }


    }
}
