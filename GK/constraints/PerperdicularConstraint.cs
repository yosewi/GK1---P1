using GK.algorithms;
using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.constraints
{
    public class PerpendicularConstraint : Constraint
    {
        private const double Epsilon = 0.0001;
        private static int nextId = 1;

        public int id { get; }
        public Edge first { get; }
        public Edge second { get; }

        public PerpendicularConstraint(Edge first, Edge second)
        {
            this.first = first;
            this.second = second;
            id = nextId++;
        }

        public override IEnumerable<Edge> Edges => new[] { first, second };

        public override string Label => $"⊥{id}";

        public override bool IsDone()
        {
            if (first.Length < 1e-9 || second.Length < 1e-9)
            {
                return true;
            }

            double ax = first.endVertex.x - first.startVertex.x;
            double ay = first.endVertex.y - first.startVertex.y;
            double bx = second.endVertex.x - second.startVertex.x;
            double by = second.endVertex.y - second.startVertex.y;

            // kąt między wektorami a i b jest równy 90 stopni, jeśli ich iloczyn skalarny jest równy zero
            double cos = (ax * bx + ay * by) / (first.Length * second.Length);

            return Math.Abs(cos) < Epsilon;
        }

        public override void Fix(Edge edge, Vertex controlledVertex)
        {
            Edge edgeToFix = OtherEdge(edge);
            Vertex vertexToMove = edge.OtherVertex(controlledVertex);

            double edgeToFixLength = edgeToFix.Length;
            if (edgeToFixLength < 1e-9)
            {
                return;
            }

            double edgeToFixXdif = edgeToFix.endVertex.x - edgeToFix.startVertex.x;
            double edgeToFixYdif = edgeToFix.endVertex.y - edgeToFix.startVertex.y;

            // prostopadły wektor do edgeToFix
            double px = -edgeToFixYdif / edgeToFixLength;
            double py = edgeToFixXdif / edgeToFixLength;

            double length = edge.Length;

            // liczy 2 możliwe punkty do przesunięcia vertexToMove, które są prostopadłe do edgeToFix
            // wybiera ten, który jest bliżej aktualnej pozycji vertexToMove
            double ax = controlledVertex.x + px * length;
            double ay = controlledVertex.y + py * length;
            double bx = controlledVertex.x - px * length;
            double by = controlledVertex.y - py * length;

            double distanceToA = Geometry.Distance(vertexToMove.x, vertexToMove.y, ax, ay);
            double distanceToB = Geometry.Distance(vertexToMove.x, vertexToMove.y, bx, by);

            if (distanceToA <= distanceToB)
            {
                vertexToMove.x = ax;
                vertexToMove.y = ay;
            }
            else
            {
                vertexToMove.x = bx;
                vertexToMove.y = by;
            }
        }

        private Edge OtherEdge(Edge edge)
        {
            if (edge == first)
            {
                return second;
            }
            return first;
        }
    }
}