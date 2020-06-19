using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Core.Map.Flat
{
    public class MapUnit
    {
        #region properties

        private const string MapUnitFormat = "{0},{1}";

        private int _x;

        public int X
        {
            get { return _x; }
            private set { _x = value; }
        }

        private int _y;

        public int Y
        {
            get { return _y; }
            private set { _y = value; }
        }

        #endregion properties

        internal MapUnit(int x, int y)
        {
            X = x;
            Y = y;
        }

        internal MapUnit(Vector2 vector)
        {
            X = Convert.ToInt32(Math.Floor(vector.X));
            Y = Convert.ToInt32(Math.Floor(vector.Y));
        }

        public override string ToString()
        {
            return string.Format(MapUnitFormat, X.ToString(), Y.ToString());
        }

        public static implicit operator Vector2(MapUnit unit)
        {
            return new Vector2(
                unit.X,
                unit.Y
                );
        }

        public static MapUnit operator +(MapUnit val, MapUnit target)
        {
            val.X += target.X;
            val.Y += target.Y;
            return val;
        }

        public static MapUnit operator -(MapUnit val, MapUnit target)
        {
            val.X -= target.X;
            val.Y -= target.Y;
            return val;
        }

        public static MapUnit operator *(MapUnit val, MapUnit target)
        {
            val.X *= target.X;
            val.Y *= target.Y;
            return val;
        }

        public static MapUnit operator /(MapUnit val, MapUnit target)
        {
            val.X /= target.X;
            val.Y /= target.Y;
            return val;
        }
    }
}