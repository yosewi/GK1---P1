using GK.drawing;
using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GK.algorithms
{
    internal class BezierCurve
    {
        private const int n = 100;

        public static List<(double x, double y)> ComputePoints(Vertex v0, ControlPoint v1, ControlPoint v2, Vertex v3)
        {
            double a0x = v0.x;
            double a0y = v0.y;
            double a1x = 3 * (v1.x - v0.x);
            double a1y = 3 * (v1.y - v0.y);
            double a2x = 3 * (v2.x - 2 * v1.x + v0.x);
            double a2y = 3 * (v2.y - 2 * v1.y + v0.y);
            double a3x = v3.x - 3 * v2.x + 3 * v1.x - v0.x;
            double a3y = v3.y - 3 * v2.y + 3 * v1.y - v0.y;

            double d = 1.0 / n;
            double d2 = d * d;
            double d3 = d * d * d;

            double px = a0x;
            double py = a0y;
            double dpx = a3x * d3 + a2x * d2 + a1x * d;
            double dpy = a3y * d3 + a2y * d2 + a1y * d;
            double d2px = 6 * a3x * d3 + 2 * a2x * d2;
            double d2py = 6 * a3y * d3 + 2 * a2y * d2;
            double d3px = 6 * a3x * d3;
            double d3py = 6 * a3y * d3;

            List<(double x, double y)> points = new List<(double x, double y)>();
            points.Add((px, py));

            for (int i = 0; i < n; i++)
            {
                double nextX = px + dpx;
                double nextY = py + dpy;

                px = nextX;
                py = nextY;
                dpx += d2px;
                dpy += d2py;
                d2px += d3px;
                d2py += d3py;

                points.Add((px, py));
            }

            return points;
        }
    }
}
