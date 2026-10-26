using GK.algorithms;
using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.constraints
{
    // Prostopadłość poprawiamy obrotem krawędzi wokół stałego końca, do bliższego z dwóch prostopadłych położeń. 
    // Gdy następna krawędź ma ustalony kierunek, wierzchołek może zamiast tego zjechać wzdłuż niej; wybieramy krótszy ruch.
    public class PerpendicularConstraint : Constraint
    {
        // tolerancja dla consiusa
        private const double Epsilon = 0.0001;
        // licznik/numerator dla prostopadlosci
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

        // prostopadle krawedzie
        public override IEnumerable<Edge> Edges => new[] { first, second };

        // napis wyswietlany na ekranie
        public override string Label => $"⊥{id}";

        // liczy cos = a·b / (|a|·|b|) i uznaje ograniczenie za spełnione, gdy |cos| < Epsilon (bo cos 90 stopni = 0)
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

        // edge to krawędź do poprawienia, fixedVertex jej koniec, który zostaje, vertexToMove drugi koniec
        // referenceEdge to wzorzec, czyli druga krawędź pary; ona się nie rusza
        public override void Fix(Edge edge, Vertex fixedVertex)
        {
            // wzorzec - druga krawędź z pary
            Edge referenceEdge = OtherEdge(edge);
            Vertex vertexToMove = edge.OtherVertex(fixedVertex);

            double referenceLength = referenceEdge.Length;
            // Wzorzec długości prawie 0 -> koniec, bo nie ma kierunku, do którego można być prostopadłym
            if (referenceLength < 1e-9)
            {
                return;
            }

            // wektor wzorca
            double referenceXdif = referenceEdge.endVertex.x - referenceEdge.startVertex.x;
            double referenceYdif = referenceEdge.endVertex.y - referenceEdge.startVertex.y;

            // kierunek prostopadły do wzorca
            double px = -referenceYdif / referenceLength;
            double py = referenceXdif / referenceLength;

            // drugi pomysł na nowe miejsce (zamiast obracac, mozna przesunac)
            (double x, double y) startPosition = (vertexToMove.x, vertexToMove.y);
            (double x, double y)? slidePosition = PositionAfterSlideAlongNextEdge(edge, fixedVertex, vertexToMove, (px, py));

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

            // jeśli da się przesunąć wierzchołki i to przesuniecie jest mniejsze niz obrócenie, to przesuwamy wierzcholk zamiast obracac
            if (slidePosition != null)
            {
                double distanceAfterRotation = Geometry.Distance(startPosition.x, startPosition.y, vertexToMove.x, vertexToMove.y);
                double distanceAfterSlide = Geometry.Distance(startPosition.x, startPosition.y, slidePosition.Value.x, slidePosition.Value.y);

                if (distanceAfterSlide <= distanceAfterRotation)
                {
                    vertexToMove.x = slidePosition.Value.x;
                    vertexToMove.y = slidePosition.Value.y;
                }
            }
        }

        // druga krawędź prostopadlej pary - wzorzec
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

        // obrót przesuwa vertexToMove w bok, więc następna krawędź (po drugiej stronie vertexToMove) zmienia kierunek. 
        // jeśli też ma prostopadłość, psuje się, a algorytm sprawddzajacy ograniczenia musi iść dalej i dłużej
        // dlatego przesuwamy vertexToMove po prostej następnej krawędzi. Ona zmienia wtedy tylko długość, a jej kierunek i prostopadłość zostają,
        // więc algorytm zatrzymuje się wcześniej
        private static (double x, double y)? PositionAfterSlideAlongNextEdge(Edge edge, Vertex fixedVertex, Vertex vertexToMove, (double x, double y) perpendicularDirection)
        {
            // robimy to jak nastepna krawedz ma ograniczenie prostopadlosci, wiec jej kierunek jest zależny, ale nie ma ograniczenia dlugosci
            Edge nextEdge = vertexToMove.EdgeOnOtherSide(edge);
            if (!HasFixedDirectionButFreeLength(nextEdge))
            {
                return null;
            }

            Vertex nextEdgeFarEnd = nextEdge.OtherVertex(vertexToMove);
            (double x, double y) nextEdgeVector = Geometry.VectorFromTo(nextEdgeFarEnd.x, nextEdgeFarEnd.y, vertexToMove.x, vertexToMove.y);
            if (Geometry.VectorLength(nextEdgeVector) < 1e-9)
            {
                return null;
            }
            (double x, double y) nextEdgeDirection = Geometry.SameDirectionWithLength(nextEdgeVector, 1);

            // punkt wspólny dwóch prostych
            (double x, double y)? crossing = Geometry.CrossingOfLines((fixedVertex.x, fixedVertex.y), perpendicularDirection, (nextEdgeFarEnd.x, nextEdgeFarEnd.y), nextEdgeDirection);
            if (crossing == null)
            {
                return null;
            }

            // nowy wektor krawedzi ma ten sam zwrot co stary, wiec krawedz nie przeskakuje nagle
            if (!KeepsDirection(fixedVertex, vertexToMove, crossing.Value) || !KeepsDirection(nextEdgeFarEnd, vertexToMove, crossing.Value))
            {
                return null;
            }

            return crossing;
        }

        // czy krawędź ma ustalony kierunek, ale wolną długość (brak zadanej długości)
        private static bool HasFixedDirectionButFreeLength(Edge edge)
        {
            bool hasPerpendicular = false;
            foreach (Constraint c in edge.constraints)
            {
                if (c is LengthConstraint)
                {
                    return false;
                }
                if (c is PerpendicularConstraint)
                {
                    hasPerpendicular = true;
                }
            }
            return hasPerpendicular;
        }

        // czy krawędź od "end" do nowego punktu ma ten sam zwrot co od "end" do vertexToMove
        private static bool KeepsDirection(Vertex end, Vertex vertexToMove, (double x, double y) newPoint)
        {
            (double x, double y) oldVector = Geometry.VectorFromTo(end.x, end.y, vertexToMove.x, vertexToMove.y);
            (double x, double y) newVector = Geometry.VectorFromTo(end.x, end.y, newPoint.x, newPoint.y);
            bool sameSide = oldVector.x * newVector.x + oldVector.y * newVector.y > 0;
            return sameSide && Geometry.VectorLength(newVector) >= 1;
        }
    }
}
