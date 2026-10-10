using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK.algorithms;

namespace GK.objects
{
    public class Vertex
    {
        public double x { get; set; }
        public double y { get; set; }
        public Edge firstEdge { get; set; }
        public Edge secondEdge { get; set; }

        public Vertex(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public void setLocation(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public bool IsHit(double x, double y, double radius)
        {
            return Geometry.Distance(this.x, this.y, x, y) <= radius;
        }
    }
}
