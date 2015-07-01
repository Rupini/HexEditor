using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Shapes;
using HexEditor.View;
using HexEditor.MathModel;

namespace HexEditor
{
    class MainBlackBoard : Canvas
    {
        #region Definition
        private HexGrid hexGrid;
        private Map map;

        private bool postInitialized;

        private Point repulsionPoint;
        private Point selectionStart;

        private bool isSelection;
        private bool isAreaSelection;
        private bool isMove;

        private Stopwatch sw = new Stopwatch();
        private string fpsStats = "0";

        private Pen framePen = new Pen(Brushes.Black, 3);

        private List<Hex> selectedHexes = new List<Hex>();
        private List<HexRender> mapHexes = new List<HexRender>();
        #endregion
        #region Constructor
        public MainBlackBoard()
        {
            this.ClipToBounds = true;

            //Binding to KeyBoardEvents
            this.Focusable = true;
            Loaded += (sender, e) => Keyboard.Focus(this);

            SizeChanged += OnSizeChanged;

            hexGrid = new HexGrid(Children, OnSelected);
            map = new Map(OnMapChanged);
        }
        #endregion
        #region Render

        protected new void InvalidateVisual()
        {
            sw.Restart();
            base.InvalidateVisual();

            this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                sw.Stop();
                fpsStats = sw.ElapsedMilliseconds.ToString();
            }));
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            hexGrid.OnRender(dc);
            RenderFrame(dc);
            RenderStats(dc);
        }

        protected void RenderStats(DrawingContext dc)
        {
            string text =
                "Position: " + hexGrid.ToWorldPoint(Mouse.GetPosition(this)).Round(1).ToString() + Environment.NewLine +
                "Ms For Frame: " + fpsStats + Environment.NewLine;

            FormattedText fText = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 14, Brushes.Lime);

            fText.SetFontWeight(FontWeights.DemiBold);
            dc.DrawText(fText, new Point());
        }

        private void RenderFrame(DrawingContext dc)
        {
            dc.DrawLine(framePen, new Point(), new Point(ActualWidth, 0));
            dc.DrawLine(framePen, new Point(ActualWidth, 0), new Point(ActualWidth, ActualHeight));
            dc.DrawLine(framePen, new Point(ActualWidth, ActualHeight), new Point(0, ActualHeight));
            dc.DrawLine(framePen, new Point(0, ActualHeight), new Point(0, 0));
        }

        #endregion
        #region Canvas_CallBacks
        #region Mouse
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var cursor = e.GetPosition(this);

            if (isMove && e.MiddleButton == MouseButtonState.Pressed)
            {
                hexGrid.OnLocalOriginChanged(repulsionPoint.Minus(cursor));
                mapHexes.ForEach((hex) => { hex.Refresh(); });
                repulsionPoint = cursor;
            }

            if (!isAreaSelection && e.LeftButton == MouseButtonState.Pressed && selectionStart.Minus(cursor).Abs() > 2)
            {
                isAreaSelection = true;
                hexGrid.OnSelectionChanged(SelectionState.Start, selectionStart);
            }

            if (isAreaSelection) hexGrid.OnSelectionChanged(SelectionState.During, cursor);


            hexGrid.RefreshSelectableHex(cursor);

            InvalidateVisual();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.LeftButton == MouseButtonState.Pressed && !isAreaSelection)
            {
                isSelection = true;
                selectionStart = e.GetPosition(this);
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                repulsionPoint = e.GetPosition(this);
                isMove = true;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (isMove && e.MiddleButton == MouseButtonState.Released) isMove = false;

            if (isSelection && e.LeftButton == MouseButtonState.Released)
            {
                isSelection = false;
                if (isAreaSelection)
                {
                    isAreaSelection = false;
                    hexGrid.OnSelectionChanged(SelectionState.End, e.GetPosition(this));
                    InvalidateVisual();
                }
                else
                    hexGrid.OnSelect();
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (isMove)
            {
                isMove = false;
            }
            if (isAreaSelection)
            {
                isAreaSelection = false;
                hexGrid.OnSelectionChanged(SelectionState.End, e.GetPosition(this));
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            hexGrid.OnScaleChanged(e.GetPosition(this), e.Delta > 0);
            InvalidateVisual();
        }
        #endregion
        #region KeyBoard

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Return)
            {
                map.AddHexRange(selectedHexes);
            }
        }

        #endregion
        #region Other
        protected void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!postInitialized)
            {
                postInitialized = true;
                hexGrid.OnLocalOriginChanged(new Point(-ActualWidth / 2, -ActualHeight / 2));
            }

            hexGrid.OnSizeChanged(e.NewSize.Width, e.NewSize.Height);
            InvalidateVisual();
        }

        public void ToCentre()
        {
            hexGrid.ToCentre();
            InvalidateVisual();
        }
        #endregion
        #endregion
        #region HexGrid_CallBacks

        private void OnSelected(List<Point> hexCenters)
        {
            selectedHexes.Clear();
            selectedHexes = hexCenters.ConvertAll((center) => map.GetHexByPoint(center));
        }

        #endregion
        #region Map_CallBacks

        private void OnMapChanged(bool add, List<HexRender> hexes)
        {
            if (add)
            {
                hexes.ForEach((hex) => { Children.Add(hex); hex.Refresh(); });
                mapHexes.AddRange(hexes);
            }
            else
                hexes.ForEach((hex) => { Children.Remove(hex); mapHexes.Remove(hex); });

        }

        #endregion
    }
}
