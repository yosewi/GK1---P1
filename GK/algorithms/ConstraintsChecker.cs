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
            Backup(polygon);

            vertex.x = x;
            vertex.y = y;

            for(int i =0; i < maxAttempts; i++)
            {
                bool forwardOk = CheckingPolygon(polygon, vertex, CheckingDirection.Forward);
                bool backwardOk = CheckingPolygon(polygon, vertex, CheckingDirection.Backward);
                if (forwardOk && backwardOk && AllEdgesGood(polygon))
                {
                    return true;
                }
            }

            RestoreBackup(polygon);
            polygon.Move(x - vertex.oldX, y - vertex.oldY);
            return false;
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
        private static bool CheckingPolygon(Polygon polygon, Vertex draggedVertex, CheckingDirection direction)
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

                if (vertexToMove == draggedVertex)
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
    }
}