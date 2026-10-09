using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.objects
{
    internal class Polygon
    {
        public List<Vertex> vertices { get; set; } = new List<Vertex>();
        public List<Edge> edges { get; set; } = new List<Edge>();

        public Polygon(List<Vertex> vertices, List<Edge> edges)
        {
            this.vertices = vertices;
            this.edges = edges;
        }
    }
}
