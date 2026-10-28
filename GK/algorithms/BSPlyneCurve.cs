using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.algorithms
{
    internal static class BSplineCurve
    {
        public static List<(double x, double y)> ControlPoints(Vertex start)
        {
            List<(double x, double y)> points = new List<(double x, double y)>();
            Vertex vertex = start;
            while (true)
            {
                points.Add((vertex.x, vertex.y));
                if (vertex.secondEdge.deleted)
                {
                    break;
                }
                vertex = vertex.secondEdge.endVertex;
                if (vertex == start)
                {
                    break;
                }
            }
            return points;
        }

        // wersja dla zamkniętego wielokąta
        public static List<(double x, double y)[]> ClosedPolygonToBezier(List<(double x, double y)> P)
        {
            int n = P.Count;

            // dzielimy boki na 3 części
            // A to te w 1/3, B w 2/3 odcinka
            (double x, double y)[] A = new (double x, double y)[n];
            (double x, double y)[] B = new (double x, double y)[n];
            for (int i = 0; i < n; i++)
            {
                (double x, double y) next = P[(i + 1) % n];
                A[i] = PointOnSide(P[i], next, 1.0 / 3);
                B[i] = PointOnSide(P[i], next, 2.0 / 3);
            }

            // środki odcinków łączących punkty B[i-1] i A[i]
            (double x, double y)[] S = new (double x, double y)[n];
            for (int i = 0; i < n; i++)
            {
                S[i] = PointOnSide(B[(i - 1 + n) % n], A[i], 0.5);
            }

            // segmenty krzywych Beziera
            List<(double x, double y)[]> segments = new List<(double x, double y)[]>();
            for (int i = 0; i < n; i++)
            {
                segments.Add(new[] { S[i], A[i], B[i], S[(i + 1) % n] });
            }
            return segments;
        }

        private static (double x, double y) PointOnSide((double x, double y) from, (double x, double y) to, double t)
        {
            return (from.x + t * (to.x - from.x), from.y + t * (to.y - from.y));
        }
    }
}
