using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GK.objects
{
    internal class Polygon
    {

        public List<Vertex> vertices { get; set; }
        public List<Edge> edges { get; set; } = new List<Edge>();

        public Polygon(List<Vertex> vertices)
        {
            this.vertices = vertices;
            BuildEdges();
        }

        private void BuildEdges()
        {
            int n = vertices.Count;

            for(int i = 0; i < n; i++) {
                Vertex start = vertices[i];
                Vertex end = vertices[(i + 1) % n];
                edges.Add(new Edge(start, end));
            }

            for(int i = 0; i < n; i++)
            {
                vertices[i].firstEdge = edges[(edges.Count - 1 + i) % edges.Count];
                vertices[i].secondEdge = edges[i];
            }
        }

        public Vertex? ClickedVertex(double x, double y, double radius)
        {
            foreach(Vertex v in vertices)
            {
                double x2 = v.x - x;
                double y2 = v.y - y;
                if(x2 * x2 + y2 * y2 <= radius * radius)
                {
                    return v;
                }
            }
            return null;
        }

        public bool IsInside(double x, double y)
        {
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;

            foreach(Vertex v in vertices)
            {
                if (v.x > maxX) maxX = v.x;
                if (v.x < minX) minX = v.x;
                if (v.y > maxY) maxY = v.y;
                if (v.y < minY) minY = v.y;
            }

            return x <= maxX && x >= minX && y <= maxY && y >= minY;
        }

        public void Move(double x, double y)
        {
            foreach(Vertex v in vertices)
            {
                v.x += x;
                v.y += y;
            }
        }

        public bool DeleteVertex(Vertex v)
        {
            if (vertices.Count <= 3)
            {
                return false;
            }

            vertices.Remove(v);
            EdgesAfterDeletingVertex(v.firstEdge, v.secondEdge);
            return true;
        }

        public void EdgesAfterDeletingVertex(Edge e1, Edge e2)
        {
            int id = edges.IndexOf(e1);
            edges.Remove(e1);
            if (edges.IndexOf(e2) < id)
            {
                id--;
            }
            edges.Remove(e2);
            Edge e = new Edge(e1.startVertex, e2.endVertex);
            edges.Insert(id, e);
            e.startVertex.secondEdge = e;
            e.endVertex.firstEdge = e;
        }
    }
}
