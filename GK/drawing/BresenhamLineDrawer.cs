using GK.algorithms;
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
            List<(int x, int y)> pixels = BrezenhamLine.FindPixels(x1, y1, x2, y2);

            using (SolidBrush brush = new SolidBrush(color))
            {
                foreach ((int x, int y) in pixels)
                {
                    g.FillRectangle(brush, x, y, 1, 1);
                }
            }
        }
    }
}
