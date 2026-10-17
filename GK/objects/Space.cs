using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK.constraints;

namespace GK.objects
{
    public class Space
    {
        public List<Polygon> polygons { get; } = new List<Polygon>();

        public void AddConstraint(Constraint constraint)
        {
            foreach (Edge edge in constraint.Edges)
            {
                edge.constraints.Add(constraint);
            }
        }

        public void RemoveConstraint(Constraint constraint)
        {
            foreach (Edge edge in constraint.Edges)
            {
                edge.constraints.Remove(constraint);
            }
        }

        public void RemoveConstraintsOfEdge(Edge edge)
        {
            foreach (Constraint constraint in edge.constraints.ToList())
            {
                RemoveConstraint(constraint);
            }
        }

        public HashSet<Constraint> AllConstraints()
        {
            HashSet<Constraint> result = new HashSet<Constraint>();
            foreach (Polygon polygon in polygons)
            {
                foreach (Edge edge in polygon.edges)
                {
                    foreach (Constraint constraint in edge.constraints)
                    {
                        result.Add(constraint);
                    }
                }
            }
            return result;
        }

        // w którym wielokącie leży krawędź
        public Polygon? PolygonOf(Edge edge)
        {
            foreach (Polygon polygon in polygons)
            {
                if (polygon.edges.Contains(edge))
                {
                    return polygon;
                }
            }
            return null;
        }

        // krawędź pod kursorem w dowolnym wielokącie
        public Edge? ClickedEdge(double x, double y, double radius)
        {
            for (int i = polygons.Count - 1; i >= 0; i--)
            {
                Edge? edge = polygons[i].ClickedEdge(x, y, radius);
                if (edge != null)
                {
                    return edge;
                }
            }
            return null;
        }
    }
}
