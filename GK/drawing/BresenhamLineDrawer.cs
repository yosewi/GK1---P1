using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.drawing
{
    internal class BresenhamLineDrawer : ILineDrawer
    {
        public void DrawLine(Graphics g, double x1, double y1, double x2, double y2, Color color)
        {
            int ax = (int)Math.Round(x1);
            int ay = (int)Math.Round(y1);
            int bx = (int)Math.Round(x2);
            int by = (int)Math.Round(y2);

            bool inverse = Math.Abs(bx - ax) < Math.Abs(by - ay);

            using (SolidBrush bruh = new SolidBrush(color))
            {
                if (inverse)
                {
                    (ax, ay) = (ay, ax);
                    (bx, by) = (by, bx);
                }

                if (ax > bx)
                {
                    (ax, bx) = (bx, ax);
                    (ay, by) = (by, ay);
                }

                int dx = bx - ax;
                int dy = by - ay;

                int direction = -1;
                if(dy >= 0)
                {
                    direction = 1;
                }
                dy *= direction;

                int distanceFromTheMiddleToTheLine = 2 * dy - dx;
                int incrE = 2 * dy;
                int incrNE = 2 * (dy-dx);
                int x = ax;
                int y = ay;
                DrawPixel(g, bruh, x, y, inverse);
                while (x < bx)
                {
                    if (distanceFromTheMiddleToTheLine < 0)
                    {
                        distanceFromTheMiddleToTheLine += incrE;
                        x++;
                    }
                    else
                    {
                        distanceFromTheMiddleToTheLine += incrNE;
                        x++;
                        y+=direction;
                    }
                    DrawPixel(g, bruh, x, y, inverse);
                }
            }
        }

        private void DrawPixel(Graphics g, Brush brush, int x, int y, bool inverse)
        {
            if (inverse)
            {
                g.FillRectangle(brush, y, x, 1, 1);
            }
            else
            {
                g.FillRectangle(brush, x, y, 1, 1);
            }
        }
    }
}
