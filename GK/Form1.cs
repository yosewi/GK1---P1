using GK.model;
using GK.objects;

namespace GK
{
    public partial class Form1 : Form
    {
        Polygon p1;
        FullScreen fullScreen;
        public Form1()
        {
            InitializeComponent();
            fullScreen = new FullScreen(this);
            Vertex v1 = new Vertex(100, 100);
            Vertex v2 = new Vertex(300, 100);
            Vertex v3 = new Vertex(200, 250);
            Edge e1 = new Edge(v1, v2);
            Edge e2 = new Edge(v2, v3);
            Edge e3 = new Edge(v3, v1);
            List<Vertex> verticesP1 = new List<Vertex> { v1, v2, v3 };
            List<Edge> edgesP1 = new List<Edge> { e1, e2, e3 };
            p1 = new Polygon(verticesP1, edgesP1);


        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            Drawer drawer = new Drawer();
            drawer.DrawPolygon(p1, e.Graphics);
        }
    }
}
