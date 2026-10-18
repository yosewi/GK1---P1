using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK.enums;
using GK.objects;

namespace GK.algorithms
{
    internal static class ContinuityFixer
    {
        // przywraca ciągłość w wierzchołku, ruszając punkt kontrolny
        public static void FixVertex(Vertex v)
        {
            if (v.secondEdge.type == EdgeType.Bezier)
            {
                FitOutgoingControlPoint(v);
            }
            else if (v.firstEdge.type == EdgeType.Bezier)
            {
                FitIncomingControlPoint(v);
            }
        }

        // przywraca ciągłość we wszystkich wierzchołkach wielokąta (ruszając tylko punkty kontrolne)
        public static void FixPolygon(Polygon polygon)
        {
            foreach (Vertex v in polygon.vertices)
            {
                FixVertex(v);
            }
        }

        // ustawia punkt kontrolny wychodzącego Béziera tak, żeby pasował do krawędzi wchodzącej
        public static void FitOutgoingControlPoint(Vertex v)
        {
            if (v.continuity == Continuity.G0 || v.secondEdge.type != EdgeType.Bezier)
            {
                return;
            }

            (double x, double y) target = OutgoingControlPointTarget(v);
            v.secondEdge.controlPoint1!.x = target.x;
            v.secondEdge.controlPoint1!.y = target.y;
        }

        // ustawia punkt kontrolny wchodzącego Béziera tak, żeby pasował do krawędzi wychodzącej
        public static void FitIncomingControlPoint(Vertex v)
        {
            if (v.continuity == Continuity.G0 || v.firstEdge.type != EdgeType.Bezier)
            {
                return;
            }

            (double x, double y) target = IncomingControlPointTarget(v);
            v.firstEdge.controlPoint2!.x = target.x;
            v.firstEdge.controlPoint2!.y = target.y;
        }

        // gdzie powinien stać uchwyt krawędzi wychodzącej, żeby zgadzał się z wchodzącą
        private static (double x, double y) OutgoingControlPointTarget(Vertex v)
        {
            (double x, double y) tangent = NewTangent(v.continuity, IncomingTangent(v), OutgoingTangent(v));
            double k = TangentFactor(v.secondEdge);

            return (v.x + tangent.x / k, v.y + tangent.y / k);
        }

        // gdzie powinien stać uchwyt krawędzi wchodzącej, żeby zgadzał się z wychodzącą
        private static (double x, double y) IncomingControlPointTarget(Vertex v)
        {
            (double x, double y) tangent = NewTangent(v.continuity, OutgoingTangent(v), IncomingTangent(v));
            double k = TangentFactor(v.firstEdge);

            return (v.x - tangent.x / k, v.y - tangent.y / k);
        }

        // jaka ma być styczna, żeby pasowała do wzorca (stycznej z drugiej strony wierzchołka)
        //  C1 - dokładnie taka jak wzorzec
        //  G1 - kierunek wzorca, ale długość dotychczasowa (β dowolne, dodatnie)
        private static (double x, double y) NewTangent(Continuity continuity, (double x, double y) pattern, (double x, double y) current)
        {
            if (continuity == Continuity.C1)
            {
                return pattern;
            }

            double patternLength = Math.Sqrt(pattern.x * pattern.x + pattern.y * pattern.y);
            double currentLength = Math.Sqrt(current.x * current.x + current.y * current.y);

            if (patternLength < 1e-9)
            {
                return current;
            }

            // wektor o dł 1 ze wzorcowej stycznej, pomnożony przez długość dotychczasowej stycznej
            return (pattern.x / patternLength * currentLength, pattern.y / patternLength * currentLength);
        }

        // styczna krawędzi wchodzącej w wierzchołku v (krawędź kończy się w v)
        private static (double x, double y) IncomingTangent(Vertex v)
        {
            Edge edge = v.firstEdge;
            if (edge.type == EdgeType.Bezier)
            {
                return (3 * (v.x - edge.controlPoint2!.x), 3 * (v.y - edge.controlPoint2!.y));
            }
            return (v.x - edge.startVertex.x, v.y - edge.startVertex.y);
        }

        // styczna krawędzi wychodzącej w wierzchołku v (krawędź zaczyna się w v)
        private static (double x, double y) OutgoingTangent(Vertex v)
        {
            Edge edge = v.secondEdge;
            if (edge.type == EdgeType.Bezier)
            {
                return (3 * (edge.controlPoint1!.x - v.x), 3 * (edge.controlPoint1!.y - v.y));
            }
            return (edge.endVertex.x - v.x, edge.endVertex.y - v.y);
        }

        // ile razy styczna jest dłuższa: 3 dla Béziera, 1 dla odcinka
        private static double TangentFactor(Edge edge)
        {
            if (edge.type == EdgeType.Bezier)
            {
                return 3;
            }
            return 1;
        }
    }
}