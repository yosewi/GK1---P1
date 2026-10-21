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

        public double Length => Geometry.Distance(startVertex.x, startVertex.y, endVertex.x, endVertex.y);

        public Vertex OtherVertex(Vertex v)
        {
            return v == startVertex ? endVertex : startVertex;
        }

        public bool IsHit(double x, double y, double radius)
        {
            if (type == EdgeType.Arc)
            {
                Arc? arc = ArcCurve.FindArcShape(this);
                if (arc != null)
                {
                    double distanceToCenter = Geometry.Distance(x, y, arc.x, arc.y);
                    double angle = Math.Atan2(y - arc.y, x - arc.x);
                    return Math.Abs(distanceToCenter - arc.radius) <= radius && arc.ContainsArcWithAngle(angle);
                }
            }
            if (type == EdgeType.Bezier)
            {
                List<(double x, double y)> points = BezierCurve.FindPoints(startVertex, controlPoint1!, controlPoint2!, endVertex);
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

                ContinuityFixer.FixVertex(startVertex);
                ContinuityFixer.FixVertex(endVertex);
                return;
            }

            type = EdgeType.Bezier;

            double dx = endVertex.x - startVertex.x;
            double dy = endVertex.y - startVertex.y;

            double px = -dy;
            double py = dx;

            controlPoint1 = new ControlPoint(startVertex.x + dx / 3 + px / 4, startVertex.y + dy / 3 + py / 4, startVertex);
            controlPoint2 = new ControlPoint(startVertex.x + 2 * dx / 3 + px / 4, startVertex.y + 2 * dy / 3 + py / 4, endVertex);

            if (!startVertex.continuitySetByUser)
            {
                startVertex.continuity = DefaultContinuityForBezier(startVertex);
            }
            if (!endVertex.continuitySetByUser)
            {
                endVertex.continuity = DefaultContinuityForBezier(endVertex);
            }

            ContinuityFixer.FitOutgoingControlPoint(startVertex); // controlPoint1
            ContinuityFixer.FitIncomingControlPoint(endVertex); // controlPoint2
        }

        private static Continuity DefaultContinuityForBezier(Vertex v)
        {
            if (v.HasArcNeighbour)
            {
                return Continuity.G0;
            }
            return Continuity.C1;
        }

        // Wektor styczny w początku krawędzi
        public (double x, double y) TangentVectorLeavingStart()
        {
            if (type == EdgeType.Bezier)
            {
                // P'(0) = 3 (V1 - V0)
                return (3 * (controlPoint1!.x - startVertex.x), 3 * (controlPoint1!.y - startVertex.y));
            }
            return (endVertex.x - startVertex.x, endVertex.y - startVertex.y);
        }

        // Wektor styczny w końcu krawędzi
        public (double x, double y) TangentVectorArrivingAtEnd()
        {
            if (type == EdgeType.Bezier)
            {
                // P'(1) = 3 (V3 - V2)
                return (3 * (endVertex.x - controlPoint2!.x), 3 * (endVertex.y - controlPoint2!.y));
            }
            return (endVertex.x - startVertex.x, endVertex.y - startVertex.y);
        }

        // przełącza krawędź: odcinek -> łuk albo łuk -> odcinek
        public void SelectArc()
        {
            if (type == EdgeType.Arc)
            {
                type = EdgeType.Line;

                // sąsiednie Béziery dopasowują się do odcinka
                ContinuityFixer.FixVertex(startVertex);
                ContinuityFixer.FixVertex(endVertex);
                return;
            }

            type = EdgeType.Arc;
            controlPoint1 = null;
            controlPoint2 = null;

            // nowy łuk zaczyna jako półokrąg: G0 w obu końcach (G1 można ustawić w jednym z nich)
            startVertex.continuity = Continuity.G0;
            startVertex.continuitySetByUser = false;
            endVertex.continuity = Continuity.G0;
            endVertex.continuitySetByUser = false;
        }
    }
}
