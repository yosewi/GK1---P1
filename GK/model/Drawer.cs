using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.model
{
    internal class Drawer
    {
        public void DrawPolygon(Polygon polygon, Graphics graphics)   
        {
            foreach(Edge edge in polygon.edges)
            {
                using (Pen pen = new Pen(Color.Black, 4.0f))
                {
                    graphics.DrawLine(pen, (int)edge.startVertex.x, (int)edge.startVertex.y, (int)edge.endVertex.x, (int)edge.endVertex.y);
                }
            }
            foreach(Vertex vertex in polygon.vertices)
            {
                graphics.FillEllipse(Brushes.Black, (int)vertex.x - 5, (int)vertex.y - 5, 10, 10);
            }
        }
    }
}
