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

        public override void Fix(Edge edge, Vertex fixedVertex)
        {
            // wzorzec - druga krawędź z pary
            Edge referenceEdge = OtherEdge(edge);
            Vertex vertexToMove = edge.OtherVertex(fixedVertex);

            double referenceLength = referenceEdge.Length;
            if (referenceLength < 1e-9)
            {
                return;
            }

            double referenceXdif = referenceEdge.endVertex.x - referenceEdge.startVertex.x;
            double referenceYdif = referenceEdge.endVertex.y - referenceEdge.startVertex.y;

            // kierunek prostopadły do wzorca
            double px = -referenceYdif / referenceLength;
            double py = referenceXdif / referenceLength;

            double length = NewLength(fixedVertex, vertexToMove);

            // liczy 2 możliwe punkty do przesunięcia vertexToMove, które są prostopadłe do wzorca
            // wybiera ten, który jest bliżej aktualnej pozycji vertexToMove
            double ax = fixedVertex.x + px * length;
            double ay = fixedVertex.y + py * length;
            double bx = fixedVertex.x - px * length;
            double by = fixedVertex.y - py * length;

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

        // nextEdge - krawędź, którą będziemy sprawdzać zaraz po obecnej
        // jeśli to nasza para, prostopadłość poprawi się dopiero tam (róg zostaje na miejscu)
        public override bool CanWaitForFix(Edge nextEdge)
        {
            return nextEdge == first || nextEdge == second;
        }

        private Edge OtherEdge(Edge edge)
        {
            if (edge == first)
            {
                return second;
            }
            return first;
        }

        // jak długa ma być krawędź po obrocie
        private static double NewLength(Vertex fixedVertex, Vertex vertexToMove)
        {
            // czy stały wierzchołek przesunął się od początku ruchu (porównujemy go z nim samym sprzed ruchu)
            bool fixedVertexMoved = fixedVertex.x != fixedVertex.oldX || fixedVertex.y != fixedVertex.oldY;

            if (fixedVertexMoved)
            {
                // przesunął się -> krawędź jedzie razem z nim i zachowuje długość sprzed ruchu
                double oldLength = Geometry.Distance(fixedVertex.oldX, fixedVertex.oldY, vertexToMove.oldX, vertexToMove.oldY);
                if (oldLength > 1e-9)
                {
                    return oldLength;
                }
            }

            // stoi -> krawędź tylko się obraca, długość bez zmian
            return Geometry.Distance(fixedVertex.x, fixedVertex.y, vertexToMove.x, vertexToMove.y);
        }

        // czy te dwie krawędzie mają już między sobą prostopadłość
        public static bool Exists(Edge first, Edge second)
        {
            foreach (Constraint c in first.constraints)
            {
                if (c is PerpendicularConstraint && c.Edges.Contains(second))
                {
                    return true;
                }
            }
            return false;
        }
    }
}