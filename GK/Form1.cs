using GK.enums;
using GK.model;
using GK.objects;

namespace GK
{
    public partial class Form1 : Form
    {
        List<Polygon> polygons = new List<Polygon>();
        FullScreen fullScreen;
        Creator creator;
        public DraggingType draggingType { get; set; } = DraggingType.None;
        Vertex? draggedVertex = null;
        Polygon? draggedPolygon = null;
        Vertex? menuVertex = null;
        Polygon? menuPolygon = null;
        Edge? menuEdge = null;
        Point lastMousePosition;

        public Form1()
        {
            InitializeComponent();
            fullScreen = new FullScreen(this);
            creator = new Creator();
            polygons.Add(creator.createTriangle());
            polygons.Add(creator.createpoli());
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            Drawer drawer = new Drawer();
            foreach (Polygon p in polygons)
            {
                drawer.DrawPolygon(p, e.Graphics);
            }
        }

        private void panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingType == DraggingType.MovingVertex && draggedVertex != null)
            {
                draggedVertex.x = e.X;
                draggedVertex.y = e.Y;
                panel.Invalidate();
            }
            else if (draggingType == DraggingType.MovingPolygon && draggedPolygon != null)
            {
                int x = e.X - lastMousePosition.X;
                int y = e.Y - lastMousePosition.Y;
                draggedPolygon.Move(x, y);
                lastMousePosition = e.Location;
                panel.Invalidate();
            }
        }

        private void panel_MouseUp(object sender, MouseEventArgs e)
        {
            draggingType = DraggingType.None;
            draggedVertex = null;
            draggedPolygon = null;
        }

        private void panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = polygons.Count - 1; i >= 0; i--)
                {
                    Vertex? vertex = polygons[i].ClickedVertex(e.X, e.Y, 8);
                    if (vertex != null)
                    {
                        menuVertex = vertex;
                        menuPolygon = polygons[i];
                        VertexContextMenu.Show(panel, e.Location);
                        return;
                    }
                }

                for (int i = polygons.Count - 1; i >= 0; i--)
                {
                    Edge? edge = polygons[i].ClickedEdge(e.X, e.Y, 6);
                    if (edge != null)
                    {
                        menuEdge = edge;
                        menuPolygon = polygons[i];
                        EdgeContextMenu.Show(panel, e.Location);
                        return;
                    }
                }
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            for (int i = polygons.Count - 1; i >= 0; i--)
            {
                Vertex? vertex = polygons[i].ClickedVertex(e.X, e.Y, 8);
                if (vertex != null)
                {
                    draggedVertex = vertex;
                    draggingType = DraggingType.MovingVertex;
                    return;
                }
            }

            for (int i = polygons.Count - 1; i >= 0; i--)
            {
                if (polygons[i].IsInside(e.X, e.Y))
                {
                    draggedPolygon = polygons[i];
                    lastMousePosition = e.Location;
                    draggingType = DraggingType.MovingPolygon;
                    return;
                }
            }
        }

        private void deleteVertexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuPolygon != null && menuVertex != null)
            {
                menuPolygon.DeleteVertex(menuVertex);
            }
            menuVertex = null;
            menuPolygon = null;
            panel.Invalidate();
        }

        private void addVertexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuPolygon != null && menuEdge != null)
            {
                menuPolygon.AddVertexOnEdge(menuEdge);
            }
            menuEdge = null;
            menuPolygon = null;
            panel.Invalidate();
        }
    }
}
