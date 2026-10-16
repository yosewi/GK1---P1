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
        private static int maxSteps = 1000;

        public static bool MoveVertex(Polygon polygon, Vertex vertex, double x, double y)
        {
            Backup(polygon);

            vertex.x = x;
            vertex.y = y;

            for(int i =0; i < maxSteps; i++)
            {
                bool forwardOk = CheckingPolygon(polygon, vertex, CheckingDirection.Forward);
                bool backwardOk = CheckingPolygon(polygon, vertex, CheckingDirection.Backward);
                if (forwardOk && backwardOk)
                {
                    return true;
                }
            }

            RestoreBackup(polygon);
            polygon.Move(x - vertex.oldX, y - vertex.oldY);
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

        private static void FixEdge(Edge edge, Vertex fixedVertex, Edge nextEdge)
        {
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

                    c.Fix(edge, fixedVertex);
                }
            }
        }
    }
}