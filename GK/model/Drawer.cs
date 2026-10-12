using GK.algorithms;
using GK.drawing;
using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using GK.constraints;

namespace GK.model
{
    internal class Drawer
    {
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
                lineDrawer.DrawLine(graphics, edge.startVertex.x, edge.startVertex.y, edge.endVertex.x, edge.endVertex.y, Color.Black);
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
    }
}
