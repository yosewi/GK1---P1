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
        // wierzchołek, przy którym leży ten punkt kontrolny
        public Vertex vertex { get; }
        
        public ControlPoint(double x, double y, Vertex vertex)
        {
            this.x = x;
            this.y = y;
            this.vertex = vertex;
        }

        // czy trafiliśmy punkt kontrolny
        public bool IsHit(double px, double py, double radius)
        {
            return Geometry.Distance(x, y, px, py) <= radius;
        }

        // do której krawędzi należy punkt kontrolny
        public Edge EdgeItBelongsTo
        {
            get
            {
                if (vertex.secondEdge.controlPoint1 == this)
                {
                    return vertex.secondEdge;
                }
                return vertex.firstEdge;
            }
        }
    }
}
