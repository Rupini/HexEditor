using HexEditor.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HexEditor.MathModel
{
    public class Map
    {
        #region Definition

        private const double X_CENTRE_DISTANCE_FACTOR = 1.5d;

        //Variables have names according geometry definitions.
        private double a; //a = R
        private double r;

        private Dictionary<int, Hex> hexMap;

        #endregion
        #region Events

        private Action<bool, List<HexRender>> mapChangedEvent;
        #endregion
        #region Constructors
        public Map(Action<bool, List<HexRender>> mapChangedEvent)
        {
            this.r = GlobalConst.DEFAULT_r;
            a = 2d / Math.Sqrt(3) * r;
            hexMap = new Dictionary<int, Hex>();
            this.mapChangedEvent = mapChangedEvent;
        }

        #endregion
        #region Functional

        public Hex GetHexByPoint(Point hexCenterPoint)
        {
            var x = Convert.ToInt32(hexCenterPoint.X / (X_CENTRE_DISTANCE_FACTOR * a));
            var y = Convert.ToInt32(hexCenterPoint.Y / r);
            var inx = new Index2D(x, y);
            if (hexMap.ContainsKey(inx))
                return hexMap[inx];
            else
                return new Hex(inx);
        }

        public Point GetCoordinateByIndex(Index2D inx)
        {
            return new Point((X_CENTRE_DISTANCE_FACTOR * a) * inx.X, r * inx.Y);
        }

        public void AddHexRange(List<Hex> hexes)
        {
            for (int i = 0; i < hexes.Count; i++)
            {
                if (hexes[i].Type == HexType.Empty)
                {
                    var newHex = new Hex(hexes[i].Index, GetCoordinateByIndex(hexes[i].Index), HexType.SimpleGround, 1);
                    hexes[i] = newHex;
                    hexMap[newHex.Index] = newHex;
                }
                else
                {
                    hexes.RemoveAt(i);
                    i--;
                }
            }

            mapChangedEvent(true, hexes.ConvertAll((hex) => hex.GetRenderHex()));
        }
        #endregion
    }
}
