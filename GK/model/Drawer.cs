using GK.algorithms;
using GK.constraints;
using GK.drawing;
using GK.enums;
using GK.objects;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace GK.model
{
    // Rysuje całą scenę przy każdym odświeżeniu
    // Odcinki i łuki rysuje przez lineDrawer (biblioteka albo Bresenham)
    internal class Drawer
    {
        public Edge? highlightedEdge { get; set; }
        private Pen dashedPen = new Pen(Color.Gray, 1) { DashStyle = DashStyle.Dash };
        public ILineDrawer lineDrawer { get; set; } = new LibraryLineDrawer();
        public bool showBSpline { get; set; } = false;
        public bool showBSplineHelpers { get; set; } = false;
        private Pen helperPen = new Pen(Color.CornflowerBlue, 1) { DashStyle = DashStyle.Dash };
        private static readonly Color[] helperColors = { Color.Red, Color.LimeGreen, Color.Magenta, Color.Orange };

        // najpierw wszystkie wielokąty, potem napisy, żeby żadna krawędź ich nie zasłoniła
        public void DrawSpace(Space space, Graphics graphics)
        {
            foreach (Polygon polygon in space.polygons)
            {
                DrawPolygon(polygon, graphics);
            }

            if (showBSpline)
            {
                foreach (Polygon polygon in space.polygons)
                {
                    DrawBSpline(polygon, graphics);
                }
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

        // krawędzie według typu, potem wierzchołki jako kropki
        public void DrawPolygon(Polygon polygon, Graphics graphics)
        {
            foreach (Edge edge in polygon.edges)
            {
                if (edge.deleted)
                {
                    continue;
                }
                if (edge.type == EdgeType.Bezier)
                {
                    DrawBezierEdge(edge, graphics);
                }
                else if (edge.type == EdgeType.Arc)
                {
                    DrawArcEdge(edge, graphics);
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

        // napisy ograniczeń krawędzi obok środka krawędzi
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

        // rysuje krawędź Beziera jako krzywą, a do niej linie przerywane do punktów kontrolnych i same punkty kontrolne
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

        // punkty z BezierCurve łączy krótkimi odcinkami
        private void DrawBezierCurve(Graphics graphics, Vertex v0, ControlPoint v1, ControlPoint v2, Vertex v3, Color color)
        {
            List<(double x, double y)> points = BezierCurve.FindPoints(v0, v1, v2, v3);

            for (int i = 0; i < points.Count - 1; i++)
            {
                lineDrawer.DrawLine(graphics, points[i].x, points[i].y, points[i + 1].x, points[i + 1].y, color);
            }
        }

        // napis G0/G1/C1 przy wierzchołku - tylko gdy obok jest krzywa, bo tylko wtedy ciągłość ma znaczenie
        private void DrawContinuityLabel(Vertex vertex, Graphics graphics)
        {
            if (!vertex.HasCurveNeighbour)
            {
                return;
            }
            graphics.DrawString(vertex.continuity.ToString(), Label.DefaultFont, Brushes.DarkGreen, (float)vertex.x + 8, (float)vertex.y - 20);
        }

        // łuk liczony od nowa z wierzchołków
        private void DrawArcEdge(Edge edge, Graphics graphics)
        {
            Arc? arc = ArcCurve.FindArcShape(edge);
            if (arc == null)
            {
                lineDrawer.DrawLine(graphics, edge.startVertex.x, edge.startVertex.y, edge.endVertex.x, edge.endVertex.y, Color.Black);
                return;
            }

            lineDrawer.DrawArc(graphics, arc, Color.Black);
        }

        private void DrawBSpline(Polygon polygon, Graphics graphics)
        {
            if (polygon.HasDeletedEdge())
            {
                return;
            }

            List<(double x, double y)> controlPoints = BSplineCurve.ControlPoints(polygon.vertices[0]);
            List<(double x, double y)[]> segments = BSplineCurve.ClosedPolygonToBezier(controlPoints);

            foreach ((double x, double y)[] segment in segments)
            {
                // znalazłem punkty do segmentu Beziera, więc rysuje tworze z segmentu krzywą Beziera i ją rysuje istniejącym już algorytmem
                List<(double x, double y)> points = BezierCurve.FindPoints(segment[0], segment[1], segment[2], segment[3]);
                for (int i = 0; i < points.Count - 1; i++)
                {
                    lineDrawer.DrawLine(graphics, points[i].x, points[i].y, points[i + 1].x, points[i + 1].y, Color.Blue);
                }
            }

            if (showBSplineHelpers)
            {
                DrawBSplineHelpers(segments, graphics);
            }
        }

        private void DrawBSplineHelpers(List<(double x, double y)[]> segments, Graphics graphics)
        {
            for (int k = 0; k < segments.Count; k++)
            {
                (double x, double y)[] segment = segments[k];

                for (int i = 0; i < 3; i++)
                {
                    // kolorowanie i rysowanie odcinków segmentów
                    Color color = helperColors[(3 * k + i) % helperColors.Length];
                    using (Pen pen = new Pen(color, 2))
                    {
                        graphics.DrawLine(pen, (float)segment[i].x, (float)segment[i].y, (float)segment[i + 1].x, (float)segment[i + 1].y);
                    }
                }

                // punkty podziału boku: A i B
                graphics.FillRectangle(Brushes.CornflowerBlue, (float)segment[1].x - 3, (float)segment[1].y - 3, 6, 6);
                graphics.FillRectangle(Brushes.CornflowerBlue, (float)segment[2].x - 3, (float)segment[2].y - 3, 6, 6);

                // punkt sklejenia
                graphics.FillEllipse(Brushes.Blue, (float)segment[0].x - 4, (float)segment[0].y - 4, 8, 8);
            }
        }
    }
}
