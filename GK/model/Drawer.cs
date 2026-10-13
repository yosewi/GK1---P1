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
                    lineDrawer.DrawLine(graphics, edge.startVertex.x, edge.startVertex.y, edge.endVertex.x, edge.endVertex.y, Color.Black);
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
            double a0x = v0.x;
            double a0y = v0.y;
            double a1x = 3 * (v1.x - v0.x);
            double a1y = 3 * (v1.y - v0.y);
            double a2x = 3 * (v2.x - 2 * v1.x + v0.x);
            double a2y = 3 * (v2.y - 2 * v1.y + v0.y);
            double a3x = v3.x - 3 * v2.x + 3 * v1.x - v0.x;
            double a3y = v3.y - 3 * v2.y + 3 * v1.y - v0.y;

            int n = 100;
            double d = 1.0 / n;
            double d2 = d * d;
            double d3 = d * d * d;

            double px = a0x;                                            
            double py = a0y;
            double dpx = a3x * d3 + a2x * d2 + a1x * d;       
            double dpy = a3y * d3 + a2y * d2 + a1y * d;
            double d2px = 6 * a3x * d3 + 2 * a2x * d2;        
            double d2py = 6 * a3y * d3 + 2 * a2y * d2;
            double d3px = 6 * a3x * d3;                        
            double d3py = 6 * a3y * d3;

            for (int i = 0; i < n; i++)
            {
                double nextX = px + dpx;
                double nextY = py + dpy;

                lineDrawer.DrawLine(graphics, px, py, nextX, nextY, color);

                px = nextX;
                py = nextY;
                dpx += d2px;
                dpy += d2py;
                d2px += d3px;
                d2py += d3py;
            }
        }
    }
}
