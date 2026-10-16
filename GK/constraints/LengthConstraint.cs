using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.constraints
{
    public class LengthConstraint : Constraint
    {
        private const double Epsilon = 0.01;

        public Edge edge { get; }
        public double length { get; set; }

        public LengthConstraint(Edge edge, double length)
        {
            this.edge = edge;
            this.length = length;
        }

        public override IEnumerable<Edge> Edges => new[] { edge };

        public override string Label => $"{length:0}";

        public override bool IsDone()
        {
            return Math.Abs(edge.Length - length) < Epsilon;
        }

        // przesuwa drugi wierzchołek krawędzi tak, aby zachować długość krawędzi
        public override void Fix(Edge edge, Vertex movedVertex)
        {
            Vertex other = edge.OtherVertex(movedVertex);

            double dx = other.x - movedVertex.x;
            double dy = other.y - movedVertex.y;
            double currentLength = Math.Sqrt(dx * dx + dy * dy);

            if (currentLength < 1e-9)
            {
                dx = 1;
                dy = 0;
                currentLength = 1;
            }

            other.x = movedVertex.x + dx / currentLength * length;
            other.y = movedVertex.y + dy / currentLength * length;
        }
    }
}