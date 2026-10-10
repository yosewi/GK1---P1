using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.algorithms
{
    internal static class Geometry
    {
        public static double Distance(double x1, double y1, double x2, double y2)
        {
            double x = x2 - x1;
            double y = y2 - y1;
            return Math.Sqrt(x * x + y * y);
        }

        public static bool IsNearEdge(double px, double py, double x1, double y1, double x2, double y2, double radius)
        {
            double length = Distance(x1, y1, x2, y2);
            int n = Math.Max(1, (int)(length / radius));

            for (int i = 0; i <= n; i++)
            {
                double t = (double)i / n;

                double x = x1 + t * (x2 - x1);
                double y = y1 + t * (y2 - y1);

                if (Distance(px, py, x, y) <= radius)
                {
                    return true;
                }
            }
            return false;
        }

        public static (double x, double y) Midpoint(double x1, double y1, double x2, double y2)
        {
            return ((x1 + x2) / 2, (y1 + y2) / 2);
        }
    }
}
