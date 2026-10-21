using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK.algorithms;

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

        public void DrawArc(Graphics g, Arc arc, Color color)
        {
            float left = (float)(arc.x - arc.radius);
            float top = (float)(arc.y - arc.radius);
            float width = (float)(arc.radius * 2);
            float startDegree = (float)(arc.startAngle * 180 / Math.PI);
            float sweepDegree = (float)(arc.sweep * 180 / Math.PI);

            using (Pen pen = new Pen(color, 1))
            {
                g.DrawArc(pen, left, top, width, width, startDegree, sweepDegree);
            }
        }
    }
}
