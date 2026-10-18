using GK.algorithms;
using GK.constraints;
using GK.drawing;
using GK.enums;
using GK.objects;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace GK.model
{
    internal class Drawer
    {
        public Edge? highlightedEdge { get; set; }
        private Pen dashedPen = new Pen(Color.Gray, 1) { DashStyle = DashStyle.Dash };
        public ILineDrawer lineDrawer { get; set; } = new LibraryLineDrawer();
        
        public void DrawSpace(Space space, Graphics graphics)
        {
            foreach (Polygon polygon in space.polygons)
            {
                DrawPolygon(polygon, graphics);
            }

            foreach (Polygon polygon in space.polygons)
            {
                foreach (Edge edge in polygon.edges)
                {
                    DrawConstraintLabels(edge, graphics);
                }
            }

            foreach (Polygon polygon in space.polygons)
            {
                foreach (Vertex vertex in polygon.vertices)
                {
                    DrawContinuityLabel(vertex, graphics);
                }
            }
        }

        public void DrawPolygon(Polygon polygon, Graphics graphics)   
        {
            foreach (Edge edge in polygon.edges)
            {
                if (edge.type == EdgeType.Bezier)
                {
                    DrawBezierEdge(edge, graphics);
                }
                else
                {
                    Color color = Color.Black;
                    if (edge == highlightedEdge)
                    {
                        color = Color.Red;
                    }
                    lineDrawer.DrawLine(graphics, edge.startVertex.x, edge.startVertex.y, edge.endVertex.x, edge.endVertex.y, color);
                }
            }

            foreach (Vertex vertex in polygon.vertices)
            {
                graphics.FillEllipse(Brushes.Black, (int)vertex.x - 5, (int)vertex.y - 5, 10, 10);
            }
        }

        private void DrawConstraintLabels(Edge edge, Graphics graphics)
        {
            if (edge.constraints.Count == 0)
            {
                return;
            }

            List<string> labels = new List<string>();
            foreach (Constraint c in edge.constraints)
            {
                labels.Add(c.Label);
            }
            string text = string.Join(" ", labels);

            (double mx, double my) = Geometry.Midpoint(edge.startVertex.x, edge.startVertex.y, edge.endVertex.x, edge.endVertex.y);
            graphics.DrawString(text, Label.DefaultFont, Brushes.Blue, (float)mx + 6, (float)my + 6);
        }

        private void DrawBezierEdge(Edge edge, Graphics graphics)
        {
            Vertex v0 = edge.startVertex;
            ControlPoint c1 = edge.controlPoint1!;
            ControlPoint c2 = edge.controlPoint2!;
            Vertex v3 = edge.endVertex;

            // linia przerywana
            graphics.DrawLine(dashedPen, (float)v0.x, (float)v0.y, (float)c1.x, (float)c1.y);
            graphics.DrawLine(dashedPen, (float)c1.x, (float)c1.y, (float)c2.x, (float)c2.y);
            graphics.DrawLine(dashedPen, (float)c2.x, (float)c2.y, (float)v3.x, (float)v3.y);

            DrawBezierCurve(graphics, v0, c1, c2, v3, Color.Black);

            // punkty kontrolne
            graphics.FillRectangle(Brushes.Orange, (float)c1.x - 4, (float)c1.y - 4, 8, 8);
            graphics.FillRectangle(Brushes.Orange, (float)c2.x - 4, (float)c2.y - 4, 8, 8);
        }

        private void DrawBezierCurve(Graphics graphics, Vertex v0, ControlPoint v1, ControlPoint v2, Vertex v3, Color color)
        {
            List<(double x, double y)> points = BezierCurve.FindPoints(v0, v1, v2, v3);

            for (int i = 0; i < points.Count - 1; i++)
            {
                lineDrawer.DrawLine(graphics, points[i].x, points[i].y, points[i + 1].x, points[i + 1].y, color);
            }
        }

        private void DrawContinuityLabel(Vertex vertex, Graphics graphics)
        {
            if (!vertex.HasBezierNeighbour)
            {
                return;
            }
            graphics.DrawString(vertex.continuity.ToString(), Label.DefaultFont, Brushes.DarkGreen, (float)vertex.x + 8, (float)vertex.y - 20);
        }
    }
}
