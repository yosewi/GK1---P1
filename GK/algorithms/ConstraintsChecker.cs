using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK.enums;
using GK.constraints;

namespace GK.algorithms
{
    // Algorytm sprawdzający ograniczenia.
    // Zapamiętujemy stare pozycje wierzchołków -> stawiamy przeciągany wierzchołek pod kursorem ->
    // od przeciąganego wierzchołka idziemy w jedną stronę i poprawiamy kolejne krawędzie, aż trafimy na krawędź,
    // która spełnia swoje ograniczenia (tam przestajemy) -> to samo w drugą stronę.
    // Jak coś dalej nie gra, powtarzamy (najwyżej MaxCheckRepetitions razy, ale przerywamy, gdy nic się już nie rusza).
    // Jak się nie da -> cofamy wszystko i przesuwamy cały wielokąt, żeby wierzchołek był pod kursorem.
    // Druga część klasy to przeciąganie punktu kontrolnego (MoveControlPoint i metody pod nią).
    internal static class ConstraintsChecker
    {
        private const int MaxCheckRepetitions = 1000;

        // Przeciąganie wierzchołka myszą
        // true - ograniczenia udało się spełnić
        // false - nie dało się, więc przesunęliśmy cały wielokąt
        public static bool MoveVertex(Polygon polygon, Vertex vertex, double x, double y)
        {
            if (TryMoveVertex(polygon, vertex, x, y, null))
            {
                return true;
            }

            polygon.Move(x - vertex.x, y - vertex.y);
            return false;
        }

        // Próba przesunięcia wierzchołka z poprawieniem reszty wielokąta
        // cantMoveVertex - wierzchołek, którego nie wolno ruszyć (używa tego przeciąganie punktu kontrolnego)
        // true - udało się
        // false - nie da się, wszystko wraca na stare miejsca
        public static bool TryMoveVertex(Polygon polygon, Vertex vertex, double x, double y, Vertex? cantMoveVertex)
        {
            // zapamiętujemy pozycje wszystkich wierzchołków
            RememberVertexPositions(polygon);

            // przesuwamy trzymany kursorem wierzchołek
            vertex.x = x;
            vertex.y = y;

            // sprawdzamy w przód i w tył. Powtarzamy, bo poprawki w tył mogą zepsuć to, co poprawiliśmy w przód
            for (int repetition = 0; repetition < MaxCheckRepetitions; repetition++)
            {
                List<(double x, double y)> positionsBefore = CurrentPositions(polygon);

                bool forwardOk = CheckInOneDirection(polygon, vertex, cantMoveVertex, CheckingDirection.Forward);
                bool backwardOk = CheckInOneDirection(polygon, vertex, cantMoveVertex, CheckingDirection.Backward);

                // w obu kierunkach zatrzymaliśmy się na dobrej krawędzi i cały wielokąt jest ok
                // punkty kontrolne jadą za swoimi wierzchołkami, poprawiamy ciągłości i zwracamy true
                if (forwardOk && backwardOk && PolygonSatisfiesConstraints(polygon))
                {
                    MoveControlPointsWithVertices(polygon);
                    ContinuityFixer.FixPolygon(polygon);
                    return true;
                }

                // w całym powtórzeniu nic się nie ruszyło -> kolejne dadzą to samo, więc kończymy od razu
                if (NothingMoved(polygon, positionsBefore))
                {
                    break;
                }
            }

            RestoreVertexPositions(polygon);
            return false;
        }

        // Dopisuje ograniczenie do krawędzi, chyba że nie da się go spełnić.
        public static bool TryAddConstraint(Space space, Constraint constraint)
        {
            space.AddConstraint(constraint);

            // próbujemy po kolei z każdej krawędzi ograniczenia
            foreach (Edge edge in constraint.Edges)
            {
                Polygon? polygon = space.PolygonOf(edge);
                if (polygon == null)
                {
                    continue;
                }

                // przeciągamy początek krawędzi w to samo miejsce -> algorytm rusza od niego,
                // poprawia nowe ograniczenie i pilnuje wszystkich starych
                Vertex start = edge.startVertex;
                if (TryMoveVertex(polygon, start, start.x, start.y, null))
                {
                    return true;
                }
            }

            space.RemoveConstraint(constraint);
            return false;
        }

        // Przeciąganie punktu kontrolnego: punkt kontrolny zawsze staje pod kursorem,
        // a potem dopasowuje się otherEdge - krawędź po drugiej stronie jego wierzchołka:
        // G0 -> nic, punkty są niezależne, łuk -> nic, łuk przeliczy się sam przy rysowaniu
        // Bezier -> ustawiamy jego punkt kontrolny, wierzchołki stoją
        // odcinek -> obracamy go. wierzchołek ruszamy dopiero, gdy odcinek nie da rady
        public static void MoveControlPoint(Polygon polygon, ControlPoint controlPoint, double x, double y)
        {
            // punkt kontrolny pod kursorem
            controlPoint.x = x;
            controlPoint.y = y;

            // G0 -> koniec, bo nic nie jest powiązane
            Vertex vertex = controlPoint.vertex;
            if (vertex.continuity == Continuity.G0)
            {
                return;
            }

            Edge otherEdge = vertex.EdgeOnOtherSide(controlPoint.EdgeItBelongsTo);

            // łuk -> koniec, łuk sam się dopasuje
            if (otherEdge.type == EdgeType.Arc)
            {
                return;
            }

            // Bezier -> ustawiamy jego punkt kontrolny przy tym wierzchołku, wierzchołków nie ruszamy
            if (otherEdge.type == EdgeType.Bezier)
            {
                ContinuityFixer.FitControlPointOfEdgeAtVertex(otherEdge, vertex);
                return;
            }

            // odcinek -> to on musi się dopasować do punktu kontrolnego
            AdjustOtherEdgeToControlPoint(polygon, vertex, controlPoint, otherEdge);

            // odcinek nie dał rady i punkt odjechał spod kursora -> ruszamy wierzchołek
            if (!IsUnderCursor(controlPoint, x, y))
            {
                PullVertexAfterControlPoint(polygon, vertex, controlPoint, otherEdge, x, y);
            }
        }

        // Punkt kontrolny ma stać pod kursorem, a otherEdge nie mogła się dopasować:
        // przesuwamy wierzchołek o brakujący kawałek, a jak i to się nie da to cały wielokąt
        private static void PullVertexAfterControlPoint(Polygon polygon, Vertex vertex, ControlPoint controlPoint, Edge otherEdge, double x, double y)
        {
            // ile brakuje punktowi kontrolnemu do kursora
            double missingX = x - controlPoint.x;
            double missingY = y - controlPoint.y;

            // wierzchołek o tyle samo (punkt kontrolny jedzie razem z nim)
            if (TryMoveVertex(polygon, vertex, vertex.x + missingX, vertex.y + missingY, null))
            {
                // punkt dokładnie pod kursor i jeszcze raz dopasowujemy otherEdge
                controlPoint.x = x;
                controlPoint.y = y;
                AdjustOtherEdgeToControlPoint(polygon, vertex, controlPoint, otherEdge);

                if (IsUnderCursor(controlPoint, x, y))
                {
                    return;
                }

                missingX = x - controlPoint.x;
                missingY = y - controlPoint.y;
            }

            polygon.Move(missingX, missingY);
        }

        // czy punkt kontrolny stoi pod kursorem
        private static bool IsUnderCursor(ControlPoint controlPoint, double x, double y)
        {
            return Geometry.Distance(controlPoint.x, controlPoint.y, x, y) < 0.01;
        }

        // otherEdge (odcinek) ma przedłużać styczną Beziera za wierzchołkiem. Jej drugi koniec (otherEdgeEnd) leży na prostej
        // od punktu kontrolnego przez wierzchołek, po drugiej stronie wierzchołka.
        // G1 -> tylko obrót (długość zostaje), C1 -> dodatkowo długość = 3 * controlPointDistance (bo P'(0) = 3(V1 - V0)).
        private static void AdjustOtherEdgeToControlPoint(Polygon polygon, Vertex vertex, ControlPoint controlPoint, Edge otherEdge)
        {
            // kierunek od punktu kontrolnego do wierzchołka i długość controlPointDistance (odległość punktu kontrolnego od wierzchołka)
            (double x, double y) fromControlPointToVertex = Geometry.VectorFromTo(controlPoint.x, controlPoint.y, vertex.x, vertex.y);
            double controlPointDistance = Geometry.VectorLength(fromControlPointToVertex);
            if (controlPointDistance < 1e-9)
            {
                return;
            }

            (double x, double y) edgeDirection = Geometry.SameDirectionWithLength(fromControlPointToVertex, 1);

            Vertex otherEdgeEnd = otherEdge.OtherVertex(vertex);
            double oldEdgeLength = otherEdge.Length;

            double newEdgeLength = oldEdgeLength;
            if (vertex.continuity == Continuity.C1)
            {
                newEdgeLength = 3 * controlPointDistance;
            }
            if (TryMoveOtherEdgeEnd(polygon, vertex, otherEdgeEnd, edgeDirection, newEdgeLength))
            {
                return;
            }

            // C1, ale długości nie da się zmienić (np. zadana długość), więc tylko obracamy krawędź, a
            // potem FixPolygon ustawi controlPointDistance na 1/3 krawędzi, a resztę zrobi PullVertexAfterControlPoint
            if (vertex.continuity == Continuity.C1 && TryMoveOtherEdgeEnd(polygon, vertex, otherEdgeEnd, edgeDirection, oldEdgeLength))
            {
                return;
            }

            // otherEdge nie może się obrócić -> punkt kontrolny wraca na jej przedłużenie
            PutControlPointOnEdgeExtension(polygon, vertex, controlPoint, otherEdgeEnd);
        }

        // Stawia otherEdgeEnd (drugi koniec otherEdge) w odległości edgeLength od wierzchołka, w kierunku edgeDirection.
        private static bool TryMoveOtherEdgeEnd(Polygon polygon, Vertex vertex, Vertex otherEdgeEnd, (double x, double y) edgeDirection, double edgeLength)
        {
            double targetX = vertex.x + edgeDirection.x * edgeLength;
            double targetY = vertex.y + edgeDirection.y * edgeLength;
            return TryMoveVertex(polygon, otherEdgeEnd, targetX, targetY, vertex);
        }

        // otherEdge nie może się obrócić -> punkt kontrolny ustawiamy na jej przedłużeniu za wierzchołkiem,
        // tam gdzie wypada rzut kursora na to przedłużenie
        private static void PutControlPointOnEdgeExtension(Polygon polygon, Vertex vertex, ControlPoint controlPoint, Vertex otherEdgeEnd)
        {
            // kierunek przedłużenia od otherEdgeEnd przez wierzchołek
            (double x, double y) extension = Geometry.VectorFromTo(otherEdgeEnd.x, otherEdgeEnd.y, vertex.x, vertex.y);
            if (Geometry.VectorLength(extension) < 1e-9)
            {
                return;
            }
            (double x, double y) extensionDirection = Geometry.SameDirectionWithLength(extension, 1);

            // punkt kontrolny jest teraz pod kursorem, więc to rzut kursora na przedłużenie = nowa długość controlPointDistance
            (double x, double y) fromVertexToControlPoint = Geometry.VectorFromTo(vertex.x, vertex.y, controlPoint.x, controlPoint.y);
            double controlPointDistance = Math.Max(1, Geometry.LengthAlongDirection(fromVertexToControlPoint, extensionDirection));

            // C1 -> krawędź musi mieć 3 * controlPointDistance: próbujemy zmienić tylko jej długość
            // jak się nie da, ustawiamy controlPointDistance na 1/3 obecnej krawędzi
            if (vertex.continuity == Continuity.C1)
            {
                double newFarX = vertex.x - extensionDirection.x * 3 * controlPointDistance;
                double newFarY = vertex.y - extensionDirection.y * 3 * controlPointDistance;
                if (!TryMoveVertex(polygon, otherEdgeEnd, newFarX, newFarY, vertex))
                {
                    ContinuityFixer.FixVertex(vertex);
                }
                return;
            }

            // G1 -> długość controlPointDistance dowolna, tylko kierunek wzdłuż krawędzi
            controlPoint.x = vertex.x + extensionDirection.x * controlPointDistance;
            controlPoint.y = vertex.y + extensionDirection.y * controlPointDistance;
        }

        // Sprawdzanie w jednym kierunku: od przeciąganego wierzchołka poprawiamy kolejne krawędzie.
        // true - zatrzymaliśmy się na krawędzi, która spełnia swoje ograniczenia
        // false - obeszliśmy cały wielokąt albo trzeba by ruszyć przeciągany wierzchołek / cantMoveVertex
        private static bool CheckInOneDirection(Polygon polygon, Vertex draggedVertex, Vertex? cantMoveVertex, CheckingDirection direction)
        {
            Vertex fixedVertex = draggedVertex; // wierzchołek już ustawiony
            int stepsLeft = polygon.vertices.Count; // najwyżej jedno okrążenie

            while (stepsLeft > 0)
            {
                Edge edge = NextEdgeInDirection(fixedVertex, direction);

                // krawędź jest ok, dalej nie trzeba sprawdzać
                if (EdgeSatisfiesConstraints(edge))
                {
                    return true;
                }

                Vertex vertexToMove = edge.OtherVertex(fixedVertex);

                // doszliśmy dookoła do przeciąganego albo do wierzchołka, którego nie wolno ruszyć
                if (vertexToMove == draggedVertex || vertexToMove == cantMoveVertex)
                {
                    return false;
                }

                // poprawiamy krawędź, fixedVertex stoi, rusza się vertexToMove
                FixEdgeConstraints(edge, fixedVertex, NextEdgeInDirection(vertexToMove, direction));

                fixedVertex = vertexToMove;
                stepsLeft--;
            }

            return false;
        }

        // Poprawia ograniczenia krawędzi. FixedVertex zostaje, rusza się jej drugi koniec.
        // nextEdge - krawędź, którą sprawdzimy zaraz po tej.
        private static void FixEdgeConstraints(Edge edge, Vertex fixedVertex, Edge nextEdge)
        {
            // dwie rundy, bo krawędź może mieć kilka ograniczeń i poprawienie jednego czasem psuje inne
            // druga runda poprawia to, co zepsuła pierwsza
            for (int i = 0; i < 2; i++)
            {
                foreach (Constraint c in edge.constraints)
                {
                    if (c.IsDone())
                    {
                        continue;
                    }

                    if (i > 0 && c.Edges.Contains(nextEdge))
                    {
                        continue;
                    }

                    c.Fix(edge, fixedVertex);
                }
            }
        }

        // następna krawędź w danym kierunku: w przód secondEdge, w tył firstEdge
        private static Edge NextEdgeInDirection(Vertex vertex, CheckingDirection direction)
        {
            if (direction == CheckingDirection.Forward)
            {
                return vertex.secondEdge;
            }
            return vertex.firstEdge;
        }

        // czy wszystkie ograniczenia krawędzi są spełnione
        private static bool EdgeSatisfiesConstraints(Edge edge)
        {
            foreach (Constraint c in edge.constraints)
            {
                if (!c.IsDone())
                {
                    return false;
                }
            }
            return true;
        }

        // czy wszystkie ograniczenia wielokąta są spełnione
        private static bool PolygonSatisfiesConstraints(Polygon polygon)
        {
            foreach (Edge edge in polygon.edges)
            {
                if (!EdgeSatisfiesConstraints(edge))
                {
                    return false;
                }
            }
            return true;
        }

        // obecne położenia wierzchołków
        private static List<(double x, double y)> CurrentPositions(Polygon polygon)
        {
            List<(double x, double y)> positions = new List<(double x, double y)>();
            foreach (Vertex v in polygon.vertices)
            {
                positions.Add((v.x, v.y));
            }
            return positions;
        }

        // czy żaden wierzchołek nie zmienił pozycji od zapamiętanych
        private static bool NothingMoved(Polygon polygon, List<(double x, double y)> positionsBefore)
        {
            for (int i = 0; i < polygon.vertices.Count; i++)
            {
                if (polygon.vertices[i].x != positionsBefore[i].x || polygon.vertices[i].y != positionsBefore[i].y)
                {
                    return false;
                }
            }
            return true;
        }

        // zapis pozycji sprzed ruchu do oldX, oldY
        private static void RememberVertexPositions(Polygon polygon)
        {
            foreach (Vertex v in polygon.vertices)
            {
                v.oldX = v.x;
                v.oldY = v.y;
            }
        }

        // cofnięcie do pozycji sprzed ruchu
        private static void RestoreVertexPositions(Polygon polygon)
        {
            foreach (Vertex v in polygon.vertices)
            {
                v.x = v.oldX;
                v.y = v.oldY;
            }
        }

        // punkty kontrolne wszystkich Bezierów jadą za swoimi wierzchołkami
        private static void MoveControlPointsWithVertices(Polygon polygon)
        {
            foreach (Edge edge in polygon.edges)
            {
                if (edge.type == EdgeType.Bezier)
                {
                    MoveControlPointWithItsVertex(edge.controlPoint1!);
                    MoveControlPointWithItsVertex(edge.controlPoint2!);
                }
            }
        }

        // punkt kontrolny przesuwa się o tyle, o ile przesunął się jego wierzchołek
        private static void MoveControlPointWithItsVertex(ControlPoint controlPoint)
        {
            controlPoint.x += controlPoint.vertex.x - controlPoint.vertex.oldX;
            controlPoint.y += controlPoint.vertex.y - controlPoint.vertex.oldY;
        }
    }
}