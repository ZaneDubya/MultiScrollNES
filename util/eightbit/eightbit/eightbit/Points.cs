using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit
{
    public sealed class Point2D
    {
        private int _x, _y;
        public Point2D(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public Point2D()
        {
            _x = 0;
            _y = 0;
        }

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /*public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Point2D))
            {
                Point2D p = (Point2D)obj;
                if (p._x == _x && p._y == _y)
                    return true;
            }
            return false;
        }*/
    }
}
