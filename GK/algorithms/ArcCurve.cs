using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.algorithms
{
    // dane o łuku
    public class ArcCurve
    {
        public static Arc? FindArcShape(Edge edge)
        {
            Vertex start = edge.startVertex;
            Vertex end = edge.endVertex;

            double length = edge.Length;
            if(length < 1)
            {
                return null;
            }

            (double x, double y)center = Geometry.Midpoint(start.x, start.y, end.x, end.y);
            double radius = length / 2;

            double startAngle = Math.Atan2(start.y - center.y, start.x - center.x);

            double sweep = Math.PI;

            return new Arc(center.x, center.y, radius, startAngle, sweep);
        }
    }
}
