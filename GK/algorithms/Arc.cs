using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.algorithms
{
    // kształt łuku, czyli kawałek okręgu. Liczy go ArcCurve.FindArcShape, używają go rysowanie i IsHit.
    public class Arc
    {
        public double x { get; }
        public double y { get; }
        public double radius { get; }
        // pod jakim kątem ze środka widać startVertex
        public double startAngle { get; }
        // sweep - o ile kątów idziemy od startAngle do endVertex. Dodatni to zgodnie z ruchem wskazówek zegara, ujemny - przeciwnie
        public double sweep { get; }

        public Arc(double x, double y, double radius, double startAngle, double sweep)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
            this.startAngle = startAngle;
            this.sweep = sweep;
        }

        // czy punkt okręgu widziany ze środka pod kątem angle należy do łuku
        public bool ContainsArcWithAngle(double angle)
        {
            // jak daleko od startu jest ten kąt, idąc w stronę, w którą idzie łuk
            double fromStart;
            if (sweep >= 0)
            {
                fromStart = angle - startAngle;

            }
            else
            {
                fromStart = startAngle - angle;
            }

            // sprowadzamy do zakresu [0, 2π)
            fromStart %= 2 * Math.PI;
            if (fromStart < 0)
            {
                fromStart += 2 * Math.PI;
            }

            // na łuku, jeśli nie wyszliśmy dalej niż długość łuku w kątach
            if (fromStart <= Math.Abs(sweep))
            {
                return true;

            }
            else
            {
                return false;
            }
        }
    }
}
