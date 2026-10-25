using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.algorithms
{
    // Piksele łuku algorytmem MidpointCircle
    // Algorytm liczy jedną ósemkę okręgu o środku (0, 0): od punktu (0, R) w prawo, dopóki y > x.
    // Pozostałe 7 ósemek dostajemy z symetrii, przesuwamy o środek łuku i zostawiamy tylko piksele, które leżą na łuku.
    public class MidpointCircle
    {
        public static List<(int x, int y)> FindPixels(Arc arc)
        {
            List<(int x, int y)> pixels = new List<(int x, int y)>();

            int centerX = (int)Math.Round(arc.x);
            int centerY = (int)Math.Round(arc.y);
            int R = (int)Math.Round(arc.radius);

            // d - zmienna decyzyjna: znak mówi, czy środek między kandydatami jest w środku okręgu czy poza nim
            // deltaE, deltaSE - o ile zmieni się d po kroku E albo SE (też zmieniają się przy każdym kroku)
            int deltaE = 3;
            int deltaSE = 5 - 2 * R;
            int d = 1 - R;
            int x = 0;
            int y = R;

            AddPixelsOnArc(pixels, arc, centerX, centerY, x, y);
            while(y > x)
            {
                if(d < 0)
                {
                    d+= deltaE;
                    deltaE += 2;
                    deltaSE += 2;
                }
                else
                {
                    d += deltaSE;
                    deltaE += 2;
                    deltaSE += 4;
                    y--;
                }
                x++;
                AddPixelsOnArc(pixels, arc, centerX, centerY, x, y);
            }
            return pixels;
        }

        // Dodaje piksele z jednej ósemki okręgu, jeśli leżą na łuku
        private static void AddPixelsOnArc(List<(int x, int y)> pixels, Arc arc, int centerX, int centerY, int x, int y)
        {
            AddPixelIfOnArc(pixels, centerX, centerY, arc, x, y);
            AddPixelIfOnArc(pixels, centerX, centerY, arc, x, -y);
            AddPixelIfOnArc(pixels, centerX, centerY, arc, -x, y);
            AddPixelIfOnArc(pixels, centerX, centerY, arc, -x, -y);
            AddPixelIfOnArc(pixels, centerX, centerY, arc, y, x);
            AddPixelIfOnArc(pixels, centerX, centerY, arc, y, -x);
            AddPixelIfOnArc(pixels, centerX, centerY, arc, -y, x);
            AddPixelIfOnArc(pixels, centerX, centerY, arc, -y, -x);
        }

        // (seenFromAngleX, seenFromAngleY) - położenie piksela względem środka okręgu;
        // dodajemy go (przesuniętego o środek) tylko wtedy, gdy jego kąt należy do łuku
        private static void AddPixelIfOnArc(List<(int x, int y)> pixels, int x, int y, Arc arc, int seenFromAngleX, int seenFromAngleY)
        {
            double angle = Math.Atan2(seenFromAngleY, seenFromAngleX);
            if (arc.ContainsArcWithAngle(angle))
            {
                pixels.Add((x + seenFromAngleX, y + seenFromAngleY));
            }
        }
    }
}
