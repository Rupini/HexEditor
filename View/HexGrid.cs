using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HexEditor.View
{
    public class HexGrid
    {
        #region Static

        public static HexGrid I { get { return _i; } }

        private static HexGrid _i;

        #endregion
        #region Definition

        private const double MIN_SCALE = 1;
        private const double MAX_SCALE = 5;
        private const double SCALE_INCREMENT = 1.15d;

        private double real_a;
        private double real_r;

        private double scale = 1;

        private double a { get { return real_a * scale; } }
        private double r { get { return real_r * scale; } }

        private double renderWidth;
        private double renderHeight;

        private Point localOrigin;
        private Point worldOrigin;
        private Pen hexPen;

        private HexRender selectableHex;
        private List<HexRender> selectableArea = new List<HexRender>();
        private List<HexRender> selectedArea = new List<HexRender>();

        private SelectionState selectionState;

        private Point selectStart;
        private Point selectCurrent;

        private Pen selectionPen = new Pen(Brushes.DarkGreen, 2);

        UIElementCollection ui;
        #endregion
        #region Properites

        public double Scale { get { return scale; } }

        #endregion
        #region Events
        private Action<List<Point>> onSelected;
        #endregion
        #region Constructor
        public HexGrid(UIElementCollection ui, Action<List<Point>> onSelected)
        {
            _i = this;

            this.real_r = GlobalConst.DEFAULT_r;
            this.real_a = 2.0 / Math.Sqrt(3) * real_r;
            this.worldOrigin = new Point(0, 0);
            this.ui = ui;

            selectableHex = new HexRender(DisplayType.FUNC_Selectable);

            ui.Add(selectableHex);

            this.onSelected = onSelected;

            hexPen = new Pen
            {
                Brush = Brushes.BlueViolet,
                Thickness = 2,
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };
        }
        #endregion
        #region Methods
        #region Coordinates

        public Point ToWorldPoint(Point screenPoint)
        {
            double x = (screenPoint.X / scale + localOrigin.X);
            double y = (screenPoint.Y / scale + localOrigin.Y);
            return new Point(x, y);
        }

        public Point ToScreanPoint(Point worldPoint)
        {
            double x = (worldPoint.X - localOrigin.X) * scale;
            double y = (worldPoint.Y - localOrigin.Y) * scale;
            return new Point(x, y);
        }

        private Point GetHexOnScreenFromScreen(Point pointOnScreen)
        {
            return ToScreanPoint(GetHexInWorldFromScreen(pointOnScreen));
        }

        private Point GetHexInWorldFromScreen(Point pointOnScreen)//Update It!
        {
            var worldPoint = ToWorldPoint(pointOnScreen);
            var x = Math.Ceiling(worldPoint.X / (3.0 * real_a)) * 3 * real_a;
            var y = Math.Ceiling(worldPoint.Y / (2.0 * real_r)) * 2 * real_r;
            if (Math.Abs(x - worldPoint.X) > real_a) x -= 3 * real_a;
            if (Math.Abs(y - worldPoint.Y) > real_r) y -= 2 * real_r;
            var hexCenter = new Point(x, y);

            double minR = worldPoint.Minus(hexCenter).Abs();
            if (minR > real_r * 1.025)
            {
                double minAlfa = 0;
                for (double alfa = 0; alfa <= Math.PI * 2; alfa += Math.PI / 3.0)
                {

                    double tryR = hexCenter.Plus(2 * real_r * Math.Sin(alfa), 2 * real_r * Math.Cos(alfa)).Minus(worldPoint).Abs();
                    if (tryR < minR)
                    {
                        minR = tryR;
                        minAlfa = alfa;
                    }
                }
                hexCenter = hexCenter.Plus(2 * real_r * Math.Sin(minAlfa), 2 * real_r * Math.Cos(minAlfa));
            }

            return hexCenter;
        }

        private List<Point> GetHexInWorldAreaFromScreen(Point startScreenPoint, double width, double height, out int[] size, bool isMediate)
        {
            List<Point> points = new List<Point>();

            if (width < 0)
            {
                startScreenPoint.X += width;
                width *= -1;
            }

            if (height < 0)
            {
                startScreenPoint.Y += height;
                height *= -1;
            }

            var p0 = GetHexInWorldFromScreen(startScreenPoint);
            var p1 = GetHexInWorldFromScreen(new Point(startScreenPoint.X + width, startScreenPoint.Y + height));

            int nX = Convert.ToInt32((p1.X - p0.X) / (3.0 * real_a));
            int nY = Convert.ToInt32((p1.Y - p0.Y) / (2.0 * real_r));

            for (int i = 0; i <= nY; i++)
                for (int j = 0; j <= nX; j++)
                {
                    points.Add(p0.Plus(3 * real_a * j, 2 * real_r * i));
                }

            if (isMediate)
            {
                int nX_M = nX - 1;
                int nY_M = nY - 1;

                var offsetPoint = p0.Plus(1.5 * real_a, real_r);

                for (int i = 0; i <= nY_M; i++)
                    for (int j = 0; j <= nX_M; j++)
                    {
                        points.Add(offsetPoint.Plus(3 * real_a * j, 2 * real_r * i));
                    }
            }

            size = new int[] { nX, nY };

            return points;
        }
        #endregion
        #region Render

        public void OnRender(DrawingContext dc)
        {
            RenderGrid(dc);

            if (selectionState == SelectionState.During) RenderSelectionFrame(dc);
        }

        private void RenderGrid(DrawingContext dc)
        {
            int[] size;
            var points = GetHexInWorldAreaFromScreen(new Point(0, 0), renderWidth, renderHeight, out size, false);

            int x = 0;
            int y = 0;
            foreach (var worldPoint in points)
            {
                var sp = ToScreanPoint(worldPoint);
                RenderHex(dc, sp, y == 0);
                x++;
                if (x > size[0])
                {
                    x = 0;
                    y++;
                    if (renderWidth - sp.X > a / 2.0)
                        dc.DrawLine(hexPen, new Point(sp.X + a, sp.Y), new Point(sp.X + 2 * a, sp.Y));
                }
                else
                    dc.DrawLine(hexPen, new Point(sp.X + a, sp.Y), new Point(sp.X + 2 * a, sp.Y));
            }


        }

        private void RenderHex(DrawingContext dc, Point c, bool firstLine)
        {
            dc.DrawLine(hexPen, c.Plus(-a / 2.0, r), c.Plus(a / 2.0, r));
            dc.DrawLine(hexPen, c.Plus(a / 2.0, r), c.Plus(a, 0));
            dc.DrawLine(hexPen, c.Plus(a, 0), c.Plus(a / 2.0, -r));

            if (firstLine) dc.DrawLine(hexPen, c.Plus(a / 2.0, -r), c.Plus(-a / 2.0, -r));
            dc.DrawLine(hexPen, c.Plus(-a / 2.0, -r), c.Plus(-a, 0));
            dc.DrawLine(hexPen, c.Plus(-a, 0), c.Plus(-a / 2.0, r));
        }

        private void RenderSelectionFrame(DrawingContext dc)
        {
            dc.DrawLine(selectionPen, selectStart, new Point(selectCurrent.X, selectStart.Y));
            dc.DrawLine(selectionPen, new Point(selectCurrent.X, selectStart.Y), selectCurrent);
            dc.DrawLine(selectionPen, selectCurrent, new Point(selectStart.X, selectCurrent.Y));
            dc.DrawLine(selectionPen, new Point(selectStart.X, selectCurrent.Y), selectStart);
        }

        public void RefreshSelectableHex(Point cursor)
        {
            selectableHex.Refresh(GetHexInWorldFromScreen(cursor));
        }

        private void RefreshSelectedArea()
        {
            selectedArea.ForEach((hex) => { hex.Refresh(); });
        }

        #endregion
        #region Movement

        public void OnSizeChanged(double newWidth, double newHeight)
        {
            renderWidth = newWidth;
            renderHeight = newHeight;
            localOrigin = localOrigin.Minus((newWidth - renderWidth) / (scale * 2), (newHeight - renderHeight) / (scale * 2));

            EndSelect(false);
        }

        public void OnLocalOriginChanged(Point dp)
        {
            localOrigin = localOrigin.Plus(dp);
            RefreshSelectedArea();
        }

        public void OnScaleChanged(Point cursor, bool increase)
        {
            if (selectionState == SelectionState.During) return;

            var worldPoint = ToWorldPoint(cursor);
            if (increase)
            {
                scale *= SCALE_INCREMENT;
                if (scale > MAX_SCALE) scale = MAX_SCALE;
            }
            else
            {
                scale /= SCALE_INCREMENT;
                if (scale < MIN_SCALE) scale = MIN_SCALE;
            }
            localOrigin = localOrigin.Plus(worldPoint.Minus(ToWorldPoint(cursor)));

            RefreshSelectableHex(cursor);
            RefreshSelectedArea();
        }

        public void ToCentre()
        {
            localOrigin.X = -renderWidth / (scale * 2d);
            localOrigin.Y = -renderHeight / (scale * 2d);
        }
        #endregion
        #region Select

        public void OnSelectionChanged(SelectionState state, Point point)
        {
            selectionState = state;
            switch (state)
            {
                case SelectionState.Start:
                    selectStart = point;
                    ui.Remove(selectableHex);
                    break;
                case SelectionState.During:
                    DuringSelect(point);
                    break;
                case SelectionState.End:
                    EndSelect(true);
                    break;
            }
        }

        public void OnSelect()
        {
            selectedArea.RemoveAll((hex) => { ui.Remove(hex); return true; });

            if (selectableArea.Count == 0)
            {
                var selectedHex = new HexRender(DisplayType.FUNC_Selected);
                selectedHex.Refresh(selectableHex.WorldCenter);
                ui.Add(selectedHex);
                selectedArea.Add(selectedHex);
            }
            else
                selectedArea = new List<HexRender>(selectableArea);

            selectedArea.ForEach((hex) => { hex.DisplayType = DisplayType.FUNC_Selected; });
            onSelected(selectedArea.ConvertAll((hex) => hex.WorldCenter));
        }

        private void DuringSelect(Point point)
        {
            selectCurrent = point;
            int[] size;
            var points = GetHexInWorldAreaFromScreen(selectStart, selectCurrent.X - selectStart.X, selectCurrent.Y - selectStart.Y, out size, true);

            selectableArea.RemoveAll((hex) => { ui.Remove(hex); return true; });
            points.ForEach((p) => { var hex = new HexRender(DisplayType.FUNC_Selectable); hex.WorldCenter = p; selectableArea.Add(hex); });
            selectableArea.ForEach((hex) => { hex.Refresh(); ui.Add(hex); });
        }

        private void EndSelect(bool success)
        {
            if (success)
            {
                ui.Add(selectableHex);
                OnSelect();
                selectableArea.Clear();
            }
            else
            {
                selectionState = SelectionState.End;
                selectedArea.RemoveAll((hex) => { ui.Remove(hex); return true; });
            }
        }

        #endregion
        #endregion
    }
}
