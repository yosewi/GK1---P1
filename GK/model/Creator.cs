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
            Polygon Polygon1 = CreatePolygon1(space);
            CreatePolygon2(space);
            Polygon Polygon3 = CreatePolygon3(space);

            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(Polygon3.edges[6], Polygon1.edges[7]));

            return space;
        }

        private Polygon CreatePolygon1(Space space)
        {
            Polygon polygon = AddPolygon(space, (260, 50), (360, 100), (510, 190), (1060, 200), (1060, 500),
                                    (960, 400), (860, 450), (710, 425), (510, 425), (135, 250));

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

        private Polygon CreatePolygon2(Space space)
        {
            Polygon polygon = AddPolygon(space, (1090, 70), (1210, 70), (1210, 190), (1150, 250), (1210, 310), (1090, 310));

            ConstraintsChecker.TryAddConstraint(space, new LengthConstraint(polygon.edges[0], 120));
            ConstraintsChecker.TryAddConstraint(space, new LengthConstraint(polygon.edges[1], 120));
            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(polygon.edges[0], polygon.edges[1]));
            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(polygon.edges[5], polygon.edges[0]));
            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(polygon.edges[2], polygon.edges[3]));

            return polygon;
        }

        private Polygon CreatePolygon3(Space space)
        {
            Polygon polygon = AddPolygon(space, (300, 565), (540, 550), (780, 550), (990, 550), (990, 670), (660, 685), (300, 685));

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