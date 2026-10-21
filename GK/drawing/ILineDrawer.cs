using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK.algorithms;

namespace GK.drawing
{
    internal interface ILineDrawer
    {
        void DrawLine(Graphics g, double x1, double y1, double x2, double y2, Color color);
        void DrawArc(Graphics g, Arc arc, Color color);
    }
}
