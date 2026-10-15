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
        public static void MoveVertex(Polygon polygon, Vertex vertex, double x, double y)
        {
            vertex.x = x;
            vertex.y = y;

            CheckingPolygon(polygon, vertex, CheckingDirection.Forward);
            CheckingPolygon(polygon, vertex, CheckingDirection.Backward);
        }

        private static bool CheckingPolygon(Polygon polygon, Vertex movedVertex, CheckingDirection direction)
        {
            int stepsLeft = polygon.vertices.Count;

            while (stepsLeft > 0)
            {
                Edge edge = NextEdge(movedVertex, direction);

                if (IsEdgeGood(edge))
                {
                    return true;
                }

                Vertex vertexToMove = edge.OtherVertex(movedVertex);

                if (vertexToMove == movedVertex)
                {
                    return false;
                }

                FixEdge(edge, movedVertex);

                movedVertex = vertexToMove;
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