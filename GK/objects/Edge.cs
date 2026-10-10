using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK.algorithms;

namespace GK.objects
{
    public class Edge
    {
        public Vertex startVertex { get; set; }
        public Vertex endVertex { get; set; }

        public Edge(Vertex startVertex, Vertex endVertex)
        {
            this.startVertex = startVertex;
            this.endVertex = endVertex;
        }

        public bool IsHit(double x, double y, double radius)
        {
            return Geometry.IsNearEdge(x, y, startVertex.x, startVertex.y, endVertex.x, endVertex.y, radius);
        }
    }
}
