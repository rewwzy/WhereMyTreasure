using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace WhereMyTreasure.Utils
{
    public class Utils
    {
        public Point GetMiddleOfImage(Point? point, Bitmap image)
        {
            if (point != null)
                return new Point(point.Value.X + image.Width / 2, point.Value.Y + image.Height / 2);
            else return new Point(0, 0);
        }
    }
}
