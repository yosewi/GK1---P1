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
    // Zapamiętujemy stare pozycje wierzchołków -> stawiamy przeciągany wierzchołek pod kursorem ->
    // idziemy w jedną stronę i od przeciąganego wierzchołka poprawiamy kolejne krawędzie, aż trafimy na dobrą krawędź lub
    // wrócimy do przeciąganego wierzchołka -> to samo w drugą stronę. Jeśli w obu kierunkach udało się znaleźć dobrą krawędź, to ruch jest dozwolony.
    // jak nie to w pętli maxAttempts razy powtarzamy. Jak dalej źle to cofamy i przesuwamy cały wielokąt.
    internal static class ConstraintsChecker
    {
        private const int MaxWaveRepetitions = 1000;

        // Przeciąganie wierzchołka myszą.
        // true - ruch się udał
        // false - nie dało się
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
        // true - udało się
        // false - nie da się, wszystko wraca na stare miejsca
        public static bool TryMoveVertex(Polygon polygon, Vertex vertex, double x, double y, Vertex? cantMoveVertex)
        {
            // zapamietujemy pozycje wierzcholkow
            RememberVertexPositions(polygon);

            // przesuwamy trzymany kursorem wierzcholek
            vertex.x = x;
            vertex.y = y;

            // w pętli zapamietujemy pozycje -> sprawdzamy wielokąt w obie strony od przesuwanego wierzcholka
            // jak po obu stronach zatrzymalismy sie na wierzcholkach ktorych nie trzeba juz poprawiac
            // to wielokat spelnia ograniczenia -> punkty kontrolne jadą za swoimi wierzcholkami, poprawiamy ciąglości i zawracamy true
            for (int repetition = 0; repetition < MaxWaveRepetitions; repetition++)
            {
                List<(double x, double y)> positionsBefore = CurrentPositions(polygon);

                bool forwardOk = RunWaveInOneDirection(polygon, vertex, cantMoveVertex, CheckingDirection.Forward);
                bool backwardOk = RunWaveInOneDirection(polygon, vertex, cantMoveVertex, CheckingDirection.Backward);

                if (forwardOk && backwardOk && PolygonSatisfiesConstraints(polygon))
                {
                    MoveControlPointsWithVertices(polygon);
                    ContinuityFixer.FixPolygon(polygon);
                    return true;
                }

                if (NothingMoved(polygon, positionsBefore))
                {
                    break;
                }
            }

            RestoreVertexPositions(polygon);
            return false;
        }

        // dopsiuje ograniczenie do krawędzi, chyba że to niemożliwe
        public static bool TryAddConstraint(Space space, Constraint constraint)
        {
            space.AddConstraint(constraint);

            // znajdujemy krawedz tego ograniczenia 
            foreach (Edge edge in constraint.Edges)
            {
                Polygon? polygon = space.PolygonOf(edge);
                if (polygon == null)
                {
                    continue;
                }

                // robimy TryMoveVertex w to samo miejsce, żeby algorytm przeszedl przez
                // caly wielokat i sprawdzil ograniczenia
                Vertex start = edge.startVertex;
                if (TryMoveVertex(polygon, start, start.x, start.y, null))
                {
                    return true;
                }
            }

            space.RemoveConstraint(constraint);
            return false;
        }

        // 
        public static void MoveControlPoint(Polygon polygon, ControlPoint controlPoint, double x, double y)
        {
            controlPoint.x = x;
            controlPoint.y = y;

            Vertex vertex = controlPoint.vertex;
            if (vertex.continuity == Continuity.G0)
            {
                return;
            }

            Edge otherEdge = vertex.EdgeOnOtherSide(controlPoint.EdgeItBelongsTo);

            if (otherEdge.type == EdgeType.Arc)
            {
                return;
            }

            if (otherEdge.type == EdgeType.Bezier)
            {
                ContinuityFixer.FitControlPointOfEdgeAtVertex(otherEdge, vertex);
                return;
            }

            // po drugiej stronie odcinek - to on musi się dopasować do punktu kontrolnego
            AdjustLineToControlPoint(polygon, vertex, controlPoint, otherEdge);

            if (!IsUnderCursor(controlPoint, x, y))
            {
                PullVertexAfterControlPoint(polygon, vertex, controlPoint, otherEdge, x, y);
            }
        }

        // Punkt kontrolny ma stać pod kursorem
        private static void PullVertexAfterControlPoint(Polygon polygon, Vertex vertex, ControlPoint controlPoint, Edge line, double x, double y)
        {
            double missingX = x - controlPoint.x;
            double missingY = y - controlPoint.y;

            if (TryMoveVertex(polygon, vertex, vertex.x + missingX, vertex.y + missingY, null))
            {
                controlPoint.x = x;
                controlPoint.y = y;
                AdjustLineToControlPoint(polygon, vertex, controlPoint, line);

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

        private static void AdjustLineToControlPoint(Polygon polygon, Vertex vertex, ControlPoint controlPoint, Edge line)
        {
            (double x, double y) fromControlPointToVertex = Geometry.VectorFromTo(controlPoint.x, controlPoint.y, vertex.x, vertex.y);
            double controlPointLineLength = Geometry.VectorLength(fromControlPointToVertex);
            if (controlPointLineLength < 1e-9)
            {
                return;
            }

            (double x, double y) lineDirection = Geometry.SameDirectionWithLength(fromControlPointToVertex, 1);

            Vertex lineFarEnd = line.OtherVertex(vertex);
            double oldLineLength = line.Length;

            double newLineLength = oldLineLength;
            if (vertex.continuity == Continuity.C1)
            {
                newLineLength = 3 * controlPointLineLength;
            }
            if (TryPutLineFarEndAt(polygon, vertex, lineFarEnd, lineDirection, newLineLength))
            {
                return;
            }

            if (vertex.continuity == Continuity.C1 && TryPutLineFarEndAt(polygon, vertex, lineFarEnd, lineDirection, oldLineLength))
            {
                return;
            }

            SlideControlPointAlongLine(polygon, vertex, controlPoint, lineFarEnd);
        }

        private static bool TryPutLineFarEndAt(Polygon polygon, Vertex vertex, Vertex lineFarEnd, (double x, double y) lineDirection, double lineLength)
        {
            double targetX = vertex.x + lineDirection.x * lineLength;
            double targetY = vertex.y + lineDirection.y * lineLength;
            return TryMoveVertex(polygon, lineFarEnd, targetX, targetY, vertex);
        }

        private static void SlideControlPointAlongLine(Polygon polygon, Vertex vertex, ControlPoint controlPoint, Vertex lineFarEnd)
        {
            (double x, double y) lineExtension = Geometry.VectorFromTo(lineFarEnd.x, lineFarEnd.y, vertex.x, vertex.y);
            if (Geometry.VectorLength(lineExtension) < 1e-9)
            {
                return;
            }
            (double x, double y) extensionDirection = Geometry.SameDirectionWithLength(lineExtension, 1);

            (double x, double y) fromVertexToCursor = Geometry.VectorFromTo(vertex.x, vertex.y, controlPoint.x, controlPoint.y);
            double controlPointLineLength = Math.Max(1, Geometry.LengthAlongDirection(fromVertexToCursor, extensionDirection));

            if (vertex.continuity == Continuity.C1)
            {
                double newEndX = vertex.x - extensionDirection.x * 3 * controlPointLineLength;
                double newEndY = vertex.y - extensionDirection.y * 3 * controlPointLineLength;
                if (!TryMoveVertex(polygon, lineFarEnd, newEndX, newEndY, vertex))
                {
                    ContinuityFixer.FixVertex(vertex);
                }
                return;
            }

            controlPoint.x = vertex.x + extensionDirection.x * controlPointLineLength;
            controlPoint.y = vertex.y + extensionDirection.y * controlPointLineLength;
        }

        // sprawdza czy ograniczenia są zachowane przechodząc po krawędziach wielokątu 
        private static bool RunWaveInOneDirection(Polygon polygon, Vertex draggedVertex, Vertex? cantMoveVertex, CheckingDirection direction)
        {
            Vertex fixedVertex = draggedVertex; // przeciągany wierzchołek
            int stepsLeft = polygon.vertices.Count;

            while (stepsLeft > 0)
            {
                Edge edge = NextEdgeInDirection(fixedVertex, direction);

                if (EdgeSatisfiesConstraints(edge))
                {
                    return true;
                }

                Vertex vertexToMove = edge.OtherVertex(fixedVertex);

                if (vertexToMove == draggedVertex || vertexToMove == cantMoveVertex)
                {
                    return false;
                }

                FixEdgeConstraints(edge, fixedVertex, NextEdgeInDirection(vertexToMove, direction));

                fixedVertex = vertexToMove;
                stepsLeft--;
            }

            return false;
        }

        // Poprawia ograniczenia krawędzi: fixedVertex zostaje, rusza się jej drugi koniec.
        // nextEdge - krawędź, którą fala sprawdzi zaraz po tej
        // ograniczenie jest pomijane gdy jest juz spelnione, moze poczekac na poprawe
        // lub w drugim powtorzeniu jest wspolne z nastepna krawedzia ktora zostanie sprawdzona
        // nastepny krok algorytmu i tak sie nią zajmie
        private static void FixEdgeConstraints(Edge edge, Vertex fixedVertex, Edge nextEdge)
        {
            // dwa powtorzenia bo krawędź może mieć kilka ograniczeń i poprawienie jednego
            // czasami psuje inne. Drugie powtorzenie poprawia to co zepsulo pierwsze
            for (int round = 0; round < 2; round++)
            {
                foreach (Constraint c in edge.constraints)
                {
                    if (c.IsDone())
                    {
                        continue;
                    }

                    if (c.CanWaitForFix(nextEdge))
                    {
                        continue;
                    }

                    if (round > 0 && c.Edges.Contains(nextEdge))
                    {
                        continue;
                    }

                    c.Fix(edge, fixedVertex);
                }
            }
        }

        // nastepna krawedz w okreslonym kierunku
        private static Edge NextEdgeInDirection(Vertex vertex, CheckingDirection direction)
        {
            if (direction == CheckingDirection.Forward)
            {
                return vertex.secondEdge;
            }
            return vertex.firstEdge;
        }

        // czy wszystkie ograniczenia krawedzi są spelnione
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

        // czy wszystkie ograniczenia wielokąta są spelnione
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

        // polozenie wierzcholkow wielokata
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

        // zapis pozycji wierzcholkow
        private static void RememberVertexPositions(Polygon polygon)
        {
            foreach (Vertex v in polygon.vertices)
            {
                v.oldX = v.x;
                v.oldY = v.y;
            }
        }

        // odtworzenie pozycji wierzcholkow
        private static void RestoreVertexPositions(Polygon polygon)
        {
            foreach (Vertex v in polygon.vertices)
            {
                v.x = v.oldX;
                v.y = v.oldY;
            }
        }

        // kazdy punkt kontrolny przesuwa sie o tyle o ile jego wierzcholek
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

        // punkt kontrolny przesuwa sie o tyle o ile jego wierzcholek
        private static void MoveControlPointWithItsVertex(ControlPoint controlPoint)
        {
            controlPoint.x += controlPoint.vertex.x - controlPoint.vertex.oldX;
            controlPoint.y += controlPoint.vertex.y - controlPoint.vertex.oldY;
        }
    }
}
