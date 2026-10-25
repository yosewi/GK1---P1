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
        // tolerancja 0,01 pixela
        private const double Epsilon = 0.01;

        public Edge edge { get; }
        public double length { get; set; }

        public LengthConstraint(Edge edge, double length)
        {
            this.edge = edge;
            this.length = length;
        }

        // krawędź z ograniczeniem
        public override IEnumerable<Edge> Edges => new[] { edge };

        // napis wyswieltany na ekranie
        public override string Label => $"{length:0}";

        // czy ograniczenie jest spelnione
        public override bool IsDone()
        {
            return Math.Abs(edge.Length - length) < Epsilon;
        }

        // przesuwa drugi wierzchołek krawędzi tak, aby zachować długość krawędzi
        // przesuwa drugi koniec wzdłuż prostej przez oba końce, na odległość length od stałego końca.
        // Drugi koniec musi leżeć na okręgu o promieniu length wokół stałego końca. Najbliższy punkt 
        // okręgu do jego obecnego położenia leży na półprostej od środka przez to położenie.
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
