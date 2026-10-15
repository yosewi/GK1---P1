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
    internal static class ConstraintsChecker
    {
        public static bool MoveVertex(Polygon polygon, Vertex vertex, double x, double y)
        {
            List<(double x, double y)> backup = Backup(polygon);
            double oldX = vertex.x;
            double oldY = vertex.y;

            vertex.x = x;
            vertex.y = y;

            bool forwardOk = CheckingPolygon(polygon, vertex, CheckingDirection.Forward);
            bool backwardOk = CheckingPolygon(polygon, vertex, CheckingDirection.Backward);

            if (forwardOk && backwardOk)
            {
                return true;
            }

            RestoreBackup(polygon, backup);
            polygon.Move(x - oldX, y - oldY);
            return false;
        }

        private static List<(double x, double y)> Backup(Polygon polygon)
        {
            List<(double x, double y)> backup = new List<(double x, double y)>();
            foreach (Vertex v in polygon.vertices)
            {
                backup.Add((v.x, v.y));
            }
            return backup;
        }

        private static void RestoreBackup(Polygon polygon, List<(double x, double y)> backup)
        {
            for (int i = 0; i < polygon.vertices.Count; i++)
            {
                polygon.vertices[i].x = backup[i].x;
                polygon.vertices[i].y = backup[i].y;
            }
        }

        private static bool CheckingPolygon(Polygon polygon, Vertex movedVertex, CheckingDirection direction)
        {
            Vertex fixedVertex = movedVertex;
            int stepsLeft = polygon.vertices.Count;

            while (stepsLeft > 0)
            {
                Edge edge = NextEdge(fixedVertex, direction);

                if (IsEdgeGood(edge))
                {
                    return true;
                }

                Vertex vertexToMove = edge.OtherVertex(fixedVertex);

                if (vertexToMove == movedVertex)
                {
                    return false;
                }

                FixEdge(edge, fixedVertex);

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

        private static void FixEdge(Edge edge, Vertex fixedVertex)
        {
            foreach (Constraint c in edge.constraints)
            {
                if (!c.IsDone())
                {
                    c.Fix(fixedVertex);
                }
            }
        }
    }
}