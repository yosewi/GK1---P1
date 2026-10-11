using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.drawing
{
    internal class LibraryLineDrawer : ILineDrawer
    {
        public void DrawLine(Graphics g, double x1, double y1, double x2, double y2, Color color)
        {
            using (Pen pen = new Pen(color, 1))
            {
                g.DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
            }
        }
    }
}
