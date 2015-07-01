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
    public class HexRender
    {
        #region Static
        public static implicit operator Polygon(HexRender hex) { return hex.polygon; }

        private const double DEFAULT_OFFSET = 1;

        private const double CONTOUR_RATIO_THICKNESS = 0.2;

        #endregion
        #region Definition
        private HexGrid hexGrid;
        private Polygon polygon;

        private DisplayType _dType;
        private FillType fillType;

        private double _r;
        private double _a;
        private double scale;
        
        private int _height;

        private Point worldCenter;

        private Point offset;
        private double _offsetValue;

        private double r { get { return _r * scale - offset.X; } }
        private double a { get { return _a * scale - offset.Y; } }

        private double offsetValue
        {
            get
            {
                return _offsetValue;
            }
            set
            {
                _offsetValue = value;
                offset = new Point(Math.Sqrt(3.0) / 2.0 * offsetValue, offsetValue);
            }
        }

        private int height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        #endregion
        #region Properties

        public Point WorldCenter
        {
            get
            {
                return worldCenter;
            }
            set
            {
                worldCenter = value;
            }
        }

        public DisplayType DisplayType
        {
            get
            {
                return _dType;
            }
            set
            {
                _dType = value;

                if ((byte)_dType < 10)
                {

                    var display = HexDisplay.Get(_dType);


                    this.fillType = display.FillType;
                    switch (this.fillType)
                    {
                        case FillType.Entire:
                            polygon.Fill = new SolidColorBrush(display.Color);
                            polygon.Fill.Opacity = display.Opacity;
                            offsetValue = DEFAULT_OFFSET;
                            polygon.StrokeThickness = 0;
                            break;
                        case FillType.Contour:
                            polygon.Fill = new SolidColorBrush(Colors.White);
                            polygon.Fill.Opacity = 0;

                            polygon.Stroke = new SolidColorBrush(display.Color);
                            polygon.Stroke.Opacity = display.Opacity;

                            Refresh();
                            break;
                    }
                }
                else
                {
                    switch (_dType)
                    {
                        case DisplayType.GAME_SimpleGround:
                            polygon.Fill = new SolidColorBrush(Colors.Green);
                            polygon.Fill.Opacity = 1;
                            offsetValue = DEFAULT_OFFSET;
                            polygon.StrokeThickness = 0;
                            break;
                    }
                }
            }
        }

       

        #endregion
        #region Constructor

        private HexRender()
        {
            _r = GlobalConst.DEFAULT_r;
            _a = 2.0 / Math.Sqrt(3.0) * _r;
            hexGrid = HexGrid.I;
            polygon = new Polygon() { IsEnabled = false };
            for (int i = 0; i < 6; i++) polygon.Points.Add(new Point());
        }

        public HexRender(DisplayType displayType)
            : this()
        {
            this.DisplayType = displayType;
        }

        public HexRender(DisplayType displayType, int height, Point worldCenter)
            : this()
        {
            this.worldCenter = worldCenter;
            this.DisplayType = displayType;
            this.height = height;
        }
        #endregion
        #region Functional
        public void Refresh(Point newWorldCenter)
        {
            worldCenter = newWorldCenter;
            Refresh();

        }

        public void Refresh()
        {
            var localCenter = hexGrid.ToScreanPoint(worldCenter);
            this.scale = hexGrid.Scale;

            if (fillType == FillType.Contour)
            {
                polygon.StrokeThickness = CONTOUR_RATIO_THICKNESS * _r * scale;
                offsetValue = 0.5 * polygon.StrokeThickness + DEFAULT_OFFSET * scale;
            }

            polygon.Points[0] = localCenter.Plus(-a / 2.0, r);
            polygon.Points[1] = localCenter.Plus(a / 2.0, r);
            polygon.Points[2] = localCenter.Plus(a, 0);
            polygon.Points[3] = localCenter.Plus(a / 2.0, -r);
            polygon.Points[4] = localCenter.Plus(-a / 2.0, -r);
            polygon.Points[5] = localCenter.Plus(-a, 0);
        }


        #endregion
    }
}
