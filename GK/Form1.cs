using GK.algorithms;
using GK.constraints;
using GK.drawing;
using GK.enums;
using GK.model;
using GK.objects;

namespace GK
{
    // Główne okno: obsługa myszy i menu kontekstowych
    public partial class Form1 : Form
    {
        Space space; // cała scena
        FullScreen fullScreen;
        Creator creator;
        Drawer drawer = new Drawer();
        public DraggingType draggingType { get; set; } = DraggingType.None; // co aktualnie ciągniemy
        Vertex? draggedVertex = null;
        Polygon? draggedPolygon = null;
        Vertex? menuVertex = null; // na czym otwarto menu kontekstowe
        Polygon? menuPolygon = null;
        Edge? menuEdge = null;
        Point lastMousePosition; // do przesuwania całego wielokąta o różnicę ruchu myszy
        ControlPoint? draggedControlPoint = null;
        Edge? perpendicularFirstEdge = null; // != null -> czekamy na kliknięcie drugiej krawędzi prostopadłości

        public Form1()
        {
            InitializeComponent();
            fullScreen = new FullScreen(this);
            creator = new Creator();
            space = creator.CreateSpace();
            KeyDown += Form1_KeyDown;
        }

        // rysowanie całej sceny przy każdym odświeżeniu panelu
        private void panel_Paint(object sender, PaintEventArgs e)
        {
            drawer.DrawSpace(space, e.Graphics);
        }

        // ruch myszy z wciśniętym przyciskiem: robimy to, co ustawił MouseDown
        private void panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingType == DraggingType.MovingVertex && draggedVertex != null && draggedPolygon != null)
            {
                ConstraintsChecker.MoveVertex(draggedPolygon, draggedVertex, e.X, e.Y);
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
            else if (draggingType == DraggingType.MovingControlPoint && draggedControlPoint != null && draggedPolygon != null)
            {
                ConstraintsChecker.MoveControlPoint(draggedPolygon, draggedControlPoint, e.X, e.Y);
                panel.Invalidate();
            }
        }

        // puszczenie przycisku kończy przeciąganie
        private void panel_MouseUp(object sender, MouseEventArgs e)
        {
            draggingType = DraggingType.None;
            draggedVertex = null;
            draggedPolygon = null;
            draggedControlPoint = null;
        }

        // Kliknięcie w trybie wybierania prostopadłości: lewy to wybór drugiej krawędzi, inny to rezygnacja
        // ppm: menu wierzchołka albo krawędzi (z włączonymi tylko dozwolonymi opcjami)
        // lpm: chwytamy punkt kontrolny, wierzchołek albo cały wielokąt (w tej kolejności)
        // Wielokąty sprawdzamy od ostatniego, bo ten jest narysowany na wierzchu.
        private void panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (perpendicularFirstEdge != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    FinishPerpendicular(space.ClickedEdge(e.X, e.Y, 6));
                }
                else
                {
                    CancelPerpendicular();
                }
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                for (int i = space.polygons.Count - 1; i >= 0; i--)
                {
                    Vertex? vertex = space.polygons[i].ClickedVertex(e.X, e.Y, 8);
                    if (vertex != null)
                    {
                        menuVertex = vertex;
                        menuPolygon = space.polygons[i];
                        g0ToolStripMenuItem.Enabled = vertex.HasCurveNeighbour;
                        g1ToolStripMenuItem.Enabled = vertex.HasCurveNeighbour && vertex.CanBeG1() && !vertex.HasDeletedEdge;
                        C1ToolStripMenuItem.Enabled = vertex.HasBezierNeighbour && !vertex.HasArcNeighbour && !vertex.HasDeletedEdge;
                        g0ToolStripMenuItem.Checked = vertex.continuity == Continuity.G0;
                        g1ToolStripMenuItem.Checked = vertex.continuity == Continuity.G1;
                        C1ToolStripMenuItem.Checked = vertex.continuity == Continuity.C1;
                        VertexContextMenu.Show(panel, e.Location);
                        return;
                    }
                }

                for (int i = space.polygons.Count - 1; i >= 0; i--)
                {
                    Edge? edge = space.polygons[i].ClickedEdge(e.X, e.Y, 6);
                    if (edge != null)
                    {
                        menuEdge = edge;
                        menuPolygon = space.polygons[i];
                        setLengthToolStripMenuItem.Enabled = edge.type == EdgeType.Line;
                        perpendicularToToolStripMenuItem.Enabled = edge.type == EdgeType.Line;
                        deleteEdgeToolStripMenuItem.Enabled = !space.polygons[i].HasDeletedEdge();
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

            for (int i = space.polygons.Count - 1; i >= 0; i--)
            {
                ControlPoint? controlPoint = space.polygons[i].ClickedControlPoint(e.X, e.Y, 8);
                if (controlPoint != null)
                {
                    draggedControlPoint = controlPoint;
                    draggedPolygon = space.polygons[i];
                    draggingType = DraggingType.MovingControlPoint;
                    return;
                }
            }

            for (int i = space.polygons.Count - 1; i >= 0; i--)
            {
                Vertex? vertex = space.polygons[i].ClickedVertex(e.X, e.Y, 8);
                if (vertex != null)
                {
                    draggedVertex = vertex;
                    draggedPolygon = space.polygons[i];
                    draggingType = DraggingType.MovingVertex;
                    return;
                }
            }

            for (int i = space.polygons.Count - 1; i >= 0; i--)
            {
                if (space.polygons[i].IsInside(e.X, e.Y))
                {
                    draggedPolygon = space.polygons[i];
                    lastMousePosition = e.Location;
                    draggingType = DraggingType.MovingPolygon;
                    return;
                }
            }
        }

        // usuwa wierzchołek, ograniczenia jego krawędzi i poprawia ciągłości
        private void deleteVertexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuPolygon != null && menuVertex != null)
            {
                Edge e1 = menuVertex.firstEdge;
                Edge e2 = menuVertex.secondEdge;

                if (menuPolygon.DeleteVertex(menuVertex))
                {
                    space.RemoveConstraintsOfEdge(e1);
                    space.RemoveConstraintsOfEdge(e2);
                    ContinuityFixer.FixPolygon(menuPolygon);
                }
            }
            menuVertex = null;
            menuPolygon = null;
            panel.Invalidate();
        }

        // dodaje wierzchołek w środku krawędzi, ograniczenia tej krawędzi znikają
        private void addVertexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuPolygon != null && menuEdge != null)
            {
                space.RemoveConstraintsOfEdge(menuEdge);
                menuPolygon.AddVertexOnEdge(menuEdge);
                ContinuityFixer.FixPolygon(menuPolygon);
            }
            menuEdge = null;
            menuPolygon = null;
            panel.Invalidate();
        }

        // radio buttony podmieniają sposób rysowania (biblioteka / Bresenham)
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                drawer.lineDrawer = new LibraryLineDrawer();
                panel.Invalidate();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                drawer.lineDrawer = new BresenhamLineDrawer();
                panel.Invalidate();
            }
        }

        // przełączanie między odcinkiem a bezierem. Krzywa nie ma ograniczeń, więc je usuwamy
        private void bezierOnoffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuEdge != null)
            {
                space.RemoveConstraintsOfEdge(menuEdge);
                menuEdge.SelectBezier();
            }
            menuEdge = null;
            menuPolygon = null;
            panel.Invalidate();
        }

        // Zadana długość - stare ograniczenie długości zdejmujemy, próbujemy dodać nowe
        // jak się nie da, przywracamy stare i pokazujemy komunikat
        private void setLengthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuEdge == null)
            {
                return;
            }

            using (LengthDialog dialog = new LengthDialog(menuEdge.Length))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    Constraint? existingConstraint = null;
                    foreach (Constraint c in menuEdge.constraints.ToList())
                    {
                        if (c is LengthConstraint)
                        {
                            existingConstraint = c;
                            space.RemoveConstraint(c);
                        }
                    }

                    if (!ConstraintsChecker.TryAddConstraint(space, new LengthConstraint(menuEdge, dialog.Value)))
                    {
                        if (existingConstraint != null)
                        {
                            space.AddConstraint(existingConstraint);
                        }
                        MessageBox.Show("Nie da się ustawić takiej długości przy obecnych ograniczeniach.", "Ograniczenie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            menuEdge = null;
            menuPolygon = null;
            panel.Invalidate();
        }

        // usuwa wszystkie ograniczenia krawędzi
        private void removeConstraintsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuEdge != null)
            {
                space.RemoveConstraintsOfEdge(menuEdge);
            }
            menuEdge = null;
            menuPolygon = null;
            panel.Invalidate();
        }

        // zapamiętujemy pierwszą krawędź i czekamy na kliknięcie drugiej
        private void perpendicularToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuEdge != null)
            {
                perpendicularFirstEdge = menuEdge;
                panel.Cursor = Cursors.Cross;
            }
            menuEdge = null;
            menuPolygon = null;
            panel.Invalidate();
        }

        // kliknięto drugą krawędź albo i nie. sprawdzamy, czy można, i dodajemy prostopadłość
        private void FinishPerpendicular(Edge? secondEdge)
        {
            Edge firstEdge = perpendicularFirstEdge!;
            CancelPerpendicular();   // wychodzimy z trybu wybierania, niezależnie od wyniku

            if (secondEdge == null)
            {
                return;   // kliknięcie obok krawędzi = rezygnacja
            }

            if (secondEdge == firstEdge)
            {
                MessageBox.Show("Krawędź nie może być prostopadła do samej siebie.", "Ograniczenie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (secondEdge.type != EdgeType.Line)
            {
                MessageBox.Show("Krzywa (Bezier albo łuk) nie może mieć ograniczeń.", "Ograniczenie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (PerpendicularConstraint.Exists(firstEdge, secondEdge))
            {
                MessageBox.Show("Te krawędzie już są prostopadłe.", "Ograniczenie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (PerpendicularConstraint.NeighbourIsPerpendicularTo(firstEdge, secondEdge) || PerpendicularConstraint.NeighbourIsPerpendicularTo(secondEdge, firstEdge))
            {
                MessageBox.Show("Dwie sąsiednie krawędzie nie mogą być prostopadłe do tej samej krawędzi.", "Ograniczenie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PerpendicularConstraint constraint = new PerpendicularConstraint(firstEdge, secondEdge);
            if (!ConstraintsChecker.TryAddConstraint(space, constraint))
            {
                MessageBox.Show("Nie da się ustawić prostopadłości przy obecnych ograniczeniach.", "Ograniczenie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            panel.Invalidate();
        }

        // wyjście z trybu wybierania drugiej krawędzi
        private void CancelPerpendicular()
        {
            perpendicularFirstEdge = null;
            drawer.highlightedEdge = null;
            panel.Cursor = Cursors.Default;
            panel.Invalidate();
        }

        // Esc przerywa wybieranie drugiej krawędzi
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && perpendicularFirstEdge != null)
            {
                CancelPerpendicular();
            }
        }

        // wybór ciągłości z menu wierzchołka
        private void g0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetContinuity(Continuity.G0);
        }

        private void g1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetContinuity(Continuity.G1);
        }

        private void C1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetContinuity(Continuity.C1);
        }

        // ustawia ciągłość jako wybór użytkownika i od razu poprawia punkty kontrolne
        private void SetContinuity(Continuity continuity)
        {
            if (menuVertex != null)
            {
                menuVertex.SetContinuity(continuity);
            }
            menuVertex = null;
            menuPolygon = null;
            panel.Invalidate();
        }

        // przełączanie między odcinkiem a łukiem. Łuk nie ma ograniczeń, więc je usuwamy
        private void arcOnoffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuEdge != null)
            {
                space.RemoveConstraintsOfEdge(menuEdge);
                menuEdge.SelectArc();
            }
            menuEdge = null;
            menuPolygon = null;
            panel.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text =
    "Obsługa: lewy przycisk myszy przeciąga wierzchołek, punkt kontrolny albo cały wielokąt. " +
    "Prawy przycisk otwiera menu wierzchołka (usunięcie, ciągłość G0/G1/C1) albo krawędzi " +
    "(dodanie wierzchołka, Bezier, łuk, długość, prostopadłość, usunięcie ograniczeń). " +
    "Wybranie prostopadłości wymaga kliknięcia drugiej krawędzi, do której pierwsza ma być prostopadła. " +
    "Esc anuluje wybieranie drugiej krawędzi prostopadłości, F11 włącza pełny ekran.\n\n" +
    "Założenia: ograniczenia mają tylko odcinki, przeciągany punkt zawsze podąża za kursorem, " +
    "a gdy ograniczeń nie da się spełnić, przesuwa się cały wielokąt.\n\n" +
    "Model danych: przestrzeń to lista wielokątów złożonych z wierzchołków i krawędzi, " +
    "a każda krawędź ma typ (odcinek, Bezier, łuk) i listę ograniczeń.\n\n" +
    "Algorytm ograniczeń: od przeciąganego wierzchołka poprawiamy kolejne krawędzie w przód i w tył, " +
    "zatrzymując się na pierwszej krawędzi, która spełnia swoje ograniczenia.\n\n" +
    "Ciągłość i rysowanie: punkty kontrolne Beziera dopasowują się do sąsiednich krawędzi (G0/G1/C1), " +
    "a odcinki i łuki rysujemy algorytmami Bresenhama i MidpointCircle.\n\n" +
    "Więcej szczegółów w README i w komentarzach w kodzie.";

            MessageBox.Show(this, text, "Dokumentacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void deleteEdgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuPolygon != null && menuEdge != null && !menuPolygon.HasDeletedEdge())
            {
                space.RemoveConstraintsOfEdge(menuEdge);
                menuPolygon.DeleteEdge(menuEdge);
            }
            menuEdge = null;
            menuPolygon = null;
            panel.Invalidate();
        }

        private void restoreAllEdgesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Polygon polygon in space.polygons)
            {
                polygon.ResurectEdges();
            }
            panel.Invalidate();
        }

        private void DrawBSplyne_CheckedChanged(object sender, EventArgs e)
        {
            drawer.showBSpline = DrawBSplyne.Checked;
            panel.Invalidate();
        }
    }
}
