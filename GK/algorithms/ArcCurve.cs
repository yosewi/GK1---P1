using GK.enums;
using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.algorithms
{
    public class ArcCurve
    {
        private const double MaxRadius = 10000;

        public static Arc? FindArcShape(Edge edge)
        {
            Vertex start = edge.startVertex;
            Vertex end = edge.endVertex;

            double length = edge.Length;
            if (length < 1)
            {
                return null;
            }

            // G1 może być najwyżej w jednym końcu łuku
            if (start.continuity == Continuity.G1)
            {
                return ArcWithTangent(edge, start, TangentAtG1Vertex(start));
            }
            if (end.continuity == Continuity.G1)
            {
                return ArcWithTangent(edge, end, TangentAtG1Vertex(end));
            }

            return Halfcircle(edge);
        }

        // G0 w obu końcach: półokrąg, którego średnicą jest krawędź
        private static Arc Halfcircle(Edge edge)
        {
            Vertex start = edge.startVertex;
            Vertex end = edge.endVertex;

            (double x, double y) center = Geometry.Midpoint(start.x, start.y, end.x, end.y);
            double radius = edge.Length / 2;

            double startAngle = Math.Atan2(start.y - center.y, start.x - center.x);

            double sweep = Math.PI;

            return new Arc(center.x, center.y, radius, startAngle, sweep);
        }

        // Łuk przez oba końce krawędzi, który w wierzchołku tangentVertex ma kierunek tangent.
        private static Arc? ArcWithTangent(Edge edge, Vertex tangentVertex, (double x, double y) tangent)
        {
            double tangentLength = Geometry.VectorLength(tangent);
            if (tangentLength < 1e-9)
            {
                // sąsiad nie ma kierunku, więc tak jak przy G0
                return Halfcircle(edge);
            }

            // Środek okręgu.
            Vertex otherVertex = edge.OtherVertex(tangentVertex);
            (double x, double y) normal = (-tangent.y / tangentLength, tangent.x / tangentLength);
            (double x, double y) d = Geometry.VectorFromTo(tangentVertex.x, tangentVertex.y, otherVertex.x, otherVertex.y);
            double dAlongNormal = Geometry.LengthAlongDirection(d, normal);

            if (Math.Abs(dAlongNormal) < 1e-9)
            {
                return null;
            }

            double offset = (d.x * d.x + d.y * d.y) / (2 * dAlongNormal);   // po której stronie stycznej jest środek
            double radius = Math.Abs(offset);
            if (radius > MaxRadius)
            {
                return null;
            }

            double centerX = tangentVertex.x + offset * normal.x;
            double centerY = tangentVertex.y + offset * normal.y;

            // Kąty końców i sweep (o ile kątów idziemy od startVertex do endVertex).
            Vertex start = edge.startVertex;
            Vertex end = edge.endVertex;
            double startAngle = Math.Atan2(start.y - centerY, start.x - centerX);
            double endAngle = Math.Atan2(end.y - centerY, end.x - centerX);
            double sweep = endAngle - startAngle;

            if (offset > 0 && sweep < 0)
            {
                sweep += 2 * Math.PI;
            }
            if (offset < 0 && sweep > 0)
            {
                sweep -= 2 * Math.PI;
            }

            return new Arc(centerX, centerY, radius, startAngle, sweep);
        }

        // Kierunek, w którym krzywa ma przechodzić przez wierzchołek v z G1
        private static (double x, double y) TangentAtG1Vertex(Vertex v)
        {
            Edge incoming = v.firstEdge;
            Edge outgoing = v.secondEdge;

            // Dwa łuki
            if (incoming.type == EdgeType.Arc && outgoing.type == EdgeType.Arc)
            {
                Vertex U = incoming.startVertex;
                Vertex W = outgoing.endVertex;
                (double x, double y) a = Geometry.VectorFromTo(v.x, v.y, U.x, U.y);
                (double x, double y) b = Geometry.VectorFromTo(v.x, v.y, W.x, W.y);
                double aLengthSquared = a.x * a.x + a.y * a.y;
                double bLengthSquared = b.x * b.x + b.y * b.y;
                return (aLengthSquared * b.x - bLengthSquared * a.x, aLengthSquared * b.y - bLengthSquared * a.y);
            }

            // łuk wchodzi do v -> bierze kierunek krawędzi wychodzącej
            if (incoming.type == EdgeType.Arc)
            {
                return outgoing.TangentVectorLeavingStart();
            }

            // łuk wychodzi z v -> bierze kierunek krawędzi wchodzącej
            return incoming.TangentVectorArrivingAtEnd();
        }
    }
}
