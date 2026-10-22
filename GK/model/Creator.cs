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
    internal class Creator
    {
        public Creator() { }

        public Space CreateSpace()
        {
            Space space = new Space();
            Polygon constraintsPolygon = CreateConstraintsPolygon(space);
            Polygon bezierPolygon = CreateBezierPolygon(space);
            CreateArcPolygon(space);

            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(constraintsPolygon.edges[2], bezierPolygon.edges[4]));

            return space;
        }

        private Polygon CreateConstraintsPolygon(Space space)
        {
            Polygon polygon = AddPolygon(space, (230, 210), (490, 210), (490, 370), (410, 490), (230, 370));

            ConstraintsChecker.TryAddConstraint(space, new LengthConstraint(polygon.edges[0], 260));
            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(polygon.edges[0], polygon.edges[1]));
            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(polygon.edges[4], polygon.edges[0]));

            return polygon;
        }

        private Polygon CreateBezierPolygon(Space space)
        {
            Polygon polygon = AddPolygon(space, (590, 460), (560, 260), (780, 260), (810, 460), (710, 540));

            polygon.edges[1].SelectBezier();
            polygon.edges[3].SelectBezier();
            polygon.vertices[3].SetContinuity(Continuity.G1);
            polygon.vertices[4].SetContinuity(Continuity.G0);

            return polygon;
        }

        private Polygon CreateArcPolygon(Space space)
        {
            Polygon polygon = AddPolygon(space, (930, 240), (1060, 240), (1140, 340), (1080, 470), (950, 470), (900, 360));

            polygon.edges[0].SelectArc();
            polygon.edges[2].SelectArc();
            polygon.vertices[2].SetContinuity(Continuity.G1);
            polygon.edges[4].SelectArc();
            polygon.edges[3].SelectBezier();
            polygon.vertices[4].SetContinuity(Continuity.G1);

            return polygon;
        }

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