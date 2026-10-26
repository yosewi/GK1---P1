using GK.algorithms;
using GK.constraints;
using GK.enums;
using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.model
{
    // Scena startowa.
    // Każdy wielokąt ma swoją metodę Create...Polygon - nowy wielokąt to nowa metoda i jedna linia w CreateSpace.
    // Wierzchołki podajemy zgodnie z ruchem wskazówek zegara (na ekranie), wtedy półokręgi łuków wychodzą na zewnątrz.
    internal class Creator
    {
        public Creator() { }

        public Space CreateSpace()
        {
            Space space = new Space();
            Polygon teacherPolygon = CreateTeacherPolygon(space);
            CreateConstraintsPolygon(space);
            Polygon curvesPolygon = CreateCurvesPolygon(space);

            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(curvesPolygon.edges[6], teacherPolygon.edges[7]));

            return space;
        }

        private Polygon CreateTeacherPolygon(Space space)
        {
            Polygon polygon = AddPolygon(space, (200, 50), (300, 100), (450, 190), (1000, 200), (1000, 500),
                                                (900, 400), (800, 450), (650, 425), (450, 425), (75, 250));

            polygon.edges[2].SelectBezier();
            polygon.edges[5].SelectArc();
            polygon.edges[6].SelectArc();
            polygon.vertices[5].SetContinuity(Continuity.G1);
            polygon.edges[8].SelectBezier();
            polygon.vertices[8].SetContinuity(Continuity.G1);

            ConstraintsChecker.TryAddConstraint(space, new LengthConstraint(polygon.edges[0], 112));
            ConstraintsChecker.TryAddConstraint(space, new LengthConstraint(polygon.edges[1], 175));
            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(polygon.edges[3], polygon.edges[7]));

            return polygon;
        }

        private Polygon CreateConstraintsPolygon(Space space)
        {
            Polygon polygon = AddPolygon(space, (1030, 70), (1150, 70), (1150, 190), (1090, 250), (1150, 310), (1030, 310));

            ConstraintsChecker.TryAddConstraint(space, new LengthConstraint(polygon.edges[0], 120));
            ConstraintsChecker.TryAddConstraint(space, new LengthConstraint(polygon.edges[1], 120));
            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(polygon.edges[0], polygon.edges[1]));
            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(polygon.edges[5], polygon.edges[0]));
            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(polygon.edges[2], polygon.edges[3]));

            return polygon;
        }

        private Polygon CreateCurvesPolygon(Space space)
        {
            Polygon polygon = AddPolygon(space, (240, 565), (480, 550), (720, 550), (930, 550), (930, 670), (600, 685), (240, 685));

            polygon.edges[0].SelectBezier();
            polygon.edges[3].SelectArc(); 
            polygon.edges[2].SelectBezier();               
            polygon.vertices[2].SetContinuity(Continuity.G1);

            ConstraintsChecker.TryAddConstraint(space, new LengthConstraint(polygon.edges[1], 240));
            ConstraintsChecker.TryAddConstraint(space, new LengthConstraint(polygon.edges[5], 360));
            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(polygon.edges[5], polygon.edges[6]));

            return polygon;
        }

        // dodaje do sceny wielokąt o podanych wierzchołkach
        private static Polygon AddPolygon(Space space, params (double x, double y)[] points)
        {
            List<Vertex> vertices = new List<Vertex>();
            foreach ((double x, double y) in points)
            {
                vertices.Add(new Vertex(x, y));
            }

            Polygon polygon = new Polygon(vertices);
            space.polygons.Add(polygon);
            return polygon;
        }
    }
}