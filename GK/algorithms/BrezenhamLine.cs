using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.algorithms
{
    internal static class BrezenhamLine
    {
        public static List<(int x, int y)> FindPixels(double x1, double y1, double x2, double y2)
        {
            List<(int x, int y)> pixels = new List<(int x, int y)>();

            int ax = (int)Math.Round(x1);
            int ay = (int)Math.Round(y1);
            int bx = (int)Math.Round(x2);
            int by = (int)Math.Round(y2);

            // zamieniamy osie x i y, jak jest stroma linia
            bool inverse = Math.Abs(bx - ax) < Math.Abs(by - ay);

            if (inverse)
            {
                (ax, ay) = (ay, ax);
                (bx, by) = (by, bx);
            }

            // pętla algorytmu idzie zawsze w prawo, więc jeśli punkt A jest po prawej stronie punktu B, to je zamieniamy
            if (ax > bx)
            {
                (ax, bx) = (bx, ax);
                (ay, by) = (by, ay);
            }

            int dx = bx - ax;
            int dy = by - ay;

            // kierunek kroku w osi y
            int direction = -1;
            if (dy >= 0)
            {
                direction = 1;
            }
            dy *= direction;

            int distanceFromTheMiddleToTheLine = 2 * dy - dx;
            int incrE = 2 * dy;
            int incrNE = 2 * (dy - dx);
            int x = ax;
            int y = ay;
            AddPixel(pixels, x, y, inverse);
            while (x < bx)
            {
                if (distanceFromTheMiddleToTheLine < 0)
                {
                    distanceFromTheMiddleToTheLine += incrE;
                    x++;
                }
                else
                {
                    distanceFromTheMiddleToTheLine += incrNE;
                    x++;
                    y += direction;
                }
                AddPixel(pixels, x, y, inverse);
            }

            return pixels;
        }

        private static void AddPixel(List<(int x, int y)> pixels, int x, int y, bool inverse)
        {
            if (inverse)
            {
                pixels.Add((y, x));
            }
            else
            {
                pixels.Add((x, y));
            }
        }
    }
}