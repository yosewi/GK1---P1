using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.algorithms
{
    public class Arc
    {
        public double x { get; }
        public double y { get; }
        public double radius { get; }
        public double startAngle { get; }
        // sweep to kąt który idziemy od start angle w kierunku przeciwnym do ruchu wskazówek zegara
        public double sweep { get; }

        public Arc(double x, double y, double radius, double startAngle, double sweep)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
            this.startAngle = startAngle;
            this.sweep = sweep;
        }

        public bool ContainsArcWithAngle(double angle)
        {
            double fromStart;
            if (sweep >= 0)
            {
                fromStart = angle - startAngle;

            }
            else
            {
                fromStart = startAngle - angle;
            }

            fromStart %= 2 * Math.PI;
            if (fromStart < 0)
            {
                fromStart += 2 * Math.PI;
            }

            if(fromStart <= Math.Abs(sweep))
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
