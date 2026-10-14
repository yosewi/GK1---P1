using GK.algorithms;
using GK.constraints;
using GK.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.objects
{
    public class Edge
    {
        public Vertex startVertex { get; set; }
        public Vertex endVertex { get; set; }
        public List<Constraint> constraints { get; set; } = new List<Constraint>();
        public EdgeType type { get; set; } = EdgeType.Line;
        public ControlPoint? controlPoint1 { get; set; }
        public ControlPoint? controlPoint2 { get; set; }

        public Edge(Vertex startVertex, Vertex endVertex)
        {
            this.startVertex = startVertex;
            this.endVertex = endVertex;
        }

        public bool IsHit(double x, double y, double radius)
        {
            if (type == EdgeType.Bezier)
            {
                List<(double x, double y)> points = BezierCurve.ComputePoints(startVertex, controlPoint1!, controlPoint2!, endVertex);
                for (int i = 0; i < points.Count - 1; i++)
                {
                    if (Geometry.IsNearEdge(x, y, points[i].x, points[i].y, points[i + 1].x, points[i + 1].y, radius))
                    {
                        return true;
                    }
                }
                return false;
            }

            return Geometry.IsNearEdge(x, y, startVertex.x, startVertex.y, endVertex.x, endVertex.y, radius);
        }

        public void SelectBezier()
        {
            if (type == EdgeType.Bezier)
            {
                type = EdgeType.Line;
                controlPoint1 = null;
                controlPoint2 = null;
                return;
            }

            type = EdgeType.Bezier;

            double dx = endVertex.x - startVertex.x;
            double dy = endVertex.y - startVertex.y;

            double px = -dy;
            double py = dx;

            controlPoint1 = new ControlPoint(startVertex.x + dx / 3 + px / 4, startVertex.y + dy / 3 + py / 4);
            controlPoint2 = new ControlPoint(startVertex.x + 2 * dx / 3 + px / 4, startVertex.y + 2 * dy / 3 + py / 4);
        }
    }
}
