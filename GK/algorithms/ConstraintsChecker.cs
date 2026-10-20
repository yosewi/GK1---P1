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
        private static int maxAttempts = 1000;

        // true - ograniczenie spełnione
        // false - nie i przesuwamy cały wielokąt
        public static bool MoveVertex(Polygon polygon, Vertex vertex, double x, double y)
        {
            if (TryMoveVertex(polygon, vertex, x, y, null))
            {
                return true;
            }

            polygon.Move(x - vertex.x, y - vertex.y);
            return false;
        }

        // Próbuje przesunąć wierzchołek, poprawiając resztę wielokąta.
        public static bool TryMoveVertex(Polygon polygon, Vertex vertex, double x, double y, Vertex? cantMoveVertex)
        {
            Backup(polygon);

            vertex.x = x;
            vertex.y = y;

            for (int i = 0; i < maxAttempts; i++)
            {
                bool forwardOk = CheckingPolygon(polygon, vertex, cantMoveVertex, CheckingDirection.Forward);
                bool backwardOk = CheckingPolygon(polygon, vertex, cantMoveVertex, CheckingDirection.Backward);
                if (forwardOk && backwardOk && AllEdgesGood(polygon))
                {
                    MoveControlPointsWithVertices(polygon);
                    ContinuityFixer.FixPolygon(polygon);
                    return true;
                }
            }

            RestoreBackup(polygon);
            return false;
        }

        public static void MoveControlPoint(Polygon polygon, ControlPoint controlPoint, double x, double y)
        {
            controlPoint.x = x;
            controlPoint.y = y;

            Vertex vertex = controlPoint.vertex;
            if (vertex.continuity == Continuity.G0)
            {
                return;
            }

            bool isOutgoing = vertex.secondEdge.controlPoint1 == controlPoint;

            Edge otherEdge;
            if (isOutgoing)
            {
                otherEdge = vertex.firstEdge;
            }
            else
            {
                otherEdge = vertex.secondEdge;
            }

            // po drugiej stronie też Bézier - dopasowujemy jego punkt kontrolny
            if (otherEdge.type == EdgeType.Bezier)
            {
                if (isOutgoing)
                {
                    ContinuityFixer.FitIncomingControlPoint(vertex);
                }
                else
                {
                    ContinuityFixer.FitOutgoingControlPoint(vertex);
                }
                return;
            }

            // po drugiej stronie odcinek - ustawiamy go po przeciwnej stronie wierzchołka niż punkt kontrolny
            double hx = vertex.x - controlPoint.x;
            double hy = vertex.y - controlPoint.y;
            double handleLength = Math.Sqrt(hx * hx + hy * hy);
            if (handleLength < 1e-9)
            {
                return;
            }
            double directionX = hx / handleLength;
            double directionY = hy / handleLength;

            Vertex farVertex = otherEdge.OtherVertex(vertex);
            double oldLength = otherEdge.Length;

            double newLength = oldLength;
            if (vertex.continuity == Continuity.C1)
            {
                newLength = 3 * handleLength;
            }
            if (TryMoveVertex(polygon, farVertex, vertex.x + directionX * newLength, vertex.y + directionY * newLength, vertex))
            {
                return;
            }

            if (vertex.continuity == Continuity.C1 && TryMoveVertex(polygon, farVertex, vertex.x + directionX * oldLength, vertex.y + directionY * oldLength, vertex))
            {
                return;
            }

            // odcinka nie da się obrócić - punkt kontrolny jeździ po jego przedłużeniu
            SlideAlongLine(polygon, vertex, controlPoint, otherEdge, farVertex);
        }

        // Dodaje ograniczenie i od razu dopasowuje do niego kształt
        // true  - ograniczenie dodane i spełnione
        // false - nie da się. ograniczenie usunięte, kształty jak przed próbą
        public static bool TryAddConstraint(Space space, Constraint constraint)
        {
            space.AddConstraint(constraint);

            foreach (Edge edge in constraint.Edges)
            {
                Polygon? polygon = space.PolygonOf(edge);
                if (polygon == null)
                {
                    continue;
                }

                // początek krawędzi w to samo miejsce
                Vertex start = edge.startVertex;
                if (MoveVertex(polygon, start, start.x, start.y))
                {
                    return true;
                }
            }

            space.RemoveConstraint(constraint);
            return false;
        }

        private static void Backup(Polygon polygon)
        {
            foreach (Vertex v in polygon.vertices)
            {
                v.oldX = v.x;
                v.oldY = v.y;
            }
        }

        private static void RestoreBackup(Polygon polygon)
        {
            foreach(Vertex v in polygon.vertices)
            {
                v.x = v.oldX;
                v.y = v.oldY;
            }
        }

        // sprawdzamy wielokąt w danym kierunku
        // true - udało się znaleźć dobrą krawędź
        // false - nie i trzeba przesunąć cały wielokąt
        private static bool CheckingPolygon(Polygon polygon, Vertex draggedVertex, Vertex? cantMoveVertex, CheckingDirection direction)
        {
            Vertex fixedVertex = draggedVertex; // wierzchołek który już stoi dobrze
            int stepsLeft = polygon.vertices.Count;

            while (stepsLeft > 0)
            {
                Edge edge = NextEdge(fixedVertex, direction);

                if (IsEdgeGood(edge))
                {
                    return true;
                }

                Vertex vertexToMove = edge.OtherVertex(fixedVertex);

                if (vertexToMove == draggedVertex || vertexToMove == cantMoveVertex)
                {
                    return false;
                }

                FixEdge(edge, fixedVertex, NextEdge(vertexToMove, direction));

                fixedVertex = vertexToMove;
                stepsLeft--;
            }

            return false;
        }

        private static Edge NextEdge(Vertex vertex, CheckingDirection direction)
        {
            if (direction == CheckingDirection.Forward)
            {
                return vertex.secondEdge;
            }
            return vertex.firstEdge;
        }

        private static bool IsEdgeGood(Edge edge)
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

        // nextEdge - krawędź ktora jest kolejna do sprawdzania
        private static void FixEdge(Edge edge, Vertex fixedVertex, Edge nextEdge)
        {
            // dwa razy bo poprawienie jednego ograniczenia może zepsuć inne na krawędzi
            for (int round = 0; round < 2; round++)
            {
                foreach (Constraint c in edge.constraints)
                {
                    if (c.IsDone())
                    {
                        continue;
                    }

                    // poprawi się dopiero na następnej krawędzi
                    if (c.CanWaitForFix(nextEdge))
                    {
                        continue;
                    }

                    c.Fix(edge, fixedVertex);
                }
            }
        }

        // czy wszystkie krawędzie wielokąta mają spełnione ograniczenia
        private static bool AllEdgesGood(Polygon polygon)
        {
            foreach (Edge edge in polygon.edges)
            {
                if (!IsEdgeGood(edge))
                {
                    return false;
                }
            }
            return true;
        }

        // punkty kontrolne jadą razem ze swoimi wierzchołkami
        private static void MoveControlPointsWithVertices(Polygon polygon)
        {
            foreach (Edge edge in polygon.edges)
            {
                if (edge.type == EdgeType.Bezier)
                {
                    MoveWithItsVertex(edge.controlPoint1!);
                    MoveWithItsVertex(edge.controlPoint2!);
                }
            }
        }

        // przesuwa punkt kontrolny o tyle, o ile jego wierzchołek przesunął się od początku ruchu
        private static void MoveWithItsVertex(ControlPoint controlPoint)
        {
            controlPoint.x += controlPoint.vertex.x - controlPoint.vertex.oldX;
            controlPoint.y += controlPoint.vertex.y - controlPoint.vertex.oldY;
        }

        // Punkt kontrolny przy odcinku, którego nie da się obrócić.
        private static void SlideAlongLine(Polygon polygon, Vertex vertex, ControlPoint controlPoint, Edge line, Vertex otherVertex)
        {
            // kierunek przedłużenia odcinka za wierzchołkiem
            double dx = vertex.x - otherVertex.x;
            double dy = vertex.y - otherVertex.y;
            double lineLength = Math.Sqrt(dx * dx + dy * dy);
            if (lineLength < 1e-9)
            {
                return;
            }
            dx /= lineLength;
            dy /= lineLength;

            // rzut kursora na ten kierunek = nowa długość przerywanej linii od wierzchołka do punktu kontrolnego
            double controlPointLineLength = (controlPoint.x - vertex.x) * dx + (controlPoint.y - vertex.y) * dy;
            controlPointLineLength = Math.Max(1, controlPointLineLength);

            if (vertex.continuity == Continuity.C1)
            {
                double newEndX = vertex.x - dx * 3 * controlPointLineLength;
                double newEndY = vertex.y - dy * 3 * controlPointLineLength;
                if (!TryMoveVertex(polygon, otherVertex, newEndX, newEndY, vertex))
                {
                    ContinuityFixer.FixVertex(vertex);
                }
                return;
            }

            controlPoint.x = vertex.x + dx * controlPointLineLength;
            controlPoint.y = vertex.y + dy * controlPointLineLength;
        }
    }
}