using GK.drawing;
using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace GK.model
{
    internal class Drawer
    {
        public ILineDrawer lineDrawer { get; set; } = new LibraryLineDrawer();
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
    }
}
