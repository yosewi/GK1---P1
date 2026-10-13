using GK.algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.objects
{
    public class ControlPoint
    {
        public double x { get; set; }
        public double y { get; set; }

        public ControlPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public bool IsHit(double px, double py, double radius)
        {
            return Geometry.Distance(x, y, px, py) <= radius;
        }
    }
}