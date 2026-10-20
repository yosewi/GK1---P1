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

        public static (double x, double y) VectorFromTo(double fromX, double fromY, double toX, double toY)
        {
            return (toX - fromX, toY - fromY);
        }

        public static double VectorLength((double x, double y) vector)
        {
            return Math.Sqrt(vector.x * vector.x + vector.y * vector.y);
        }

        public static double LengthAlongDirection((double x, double y) vector, (double x, double y) unitDirection)
        {
            return vector.x * unitDirection.x + vector.y * unitDirection.y;
        }

        // wektor o tym samym kierunku co vector, ale o podanej długości (vector nie może mieć długości 0)
        public static (double x, double y) SameDirectionWithLength((double x, double y) vector, double newLength)
        {
            double currentLength = VectorLength(vector);
            return (vector.x / currentLength * newLength, vector.y / currentLength * newLength);
        }

        public static (double x, double y)? CrossingOfLines((double x, double y) point1, (double x, double y) direction1, (double x, double y) point2, (double x, double y) direction2)
        {
            double sinOfAngle = direction1.x * direction2.y - direction1.y * direction2.x;
            if (Math.Abs(sinOfAngle) < 0.2)
            {
                return null;
            }

            (double x, double y) fromPoint1ToPoint2 = VectorFromTo(point1.x, point1.y, point2.x, point2.y);
            double a = (fromPoint1ToPoint2.x * direction2.y - fromPoint1ToPoint2.y * direction2.x) / sinOfAngle;

            return (point1.x + a * direction1.x, point1.y + a * direction1.y);
        }
    }
}
