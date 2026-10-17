using GK.algorithms;
using GK.constraints;
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
            Polygon triangle = createTriangle();
            Polygon poli = createpoli();
            space.polygons.Add(triangle);
            space.polygons.Add(poli);
            ConstraintsChecker.TryAddConstraint(space, new LengthConstraint(triangle.edges[0], 300));
            ConstraintsChecker.TryAddConstraint(space, new PerpendicularConstraint(poli.edges[0], poli.edges[1]));

            return space;
        }

        private Polygon createTriangle()
        {
            List<Vertex> vertices = new List<Vertex>
            {
                new Vertex(100, 100),
                new Vertex(300, 100),
                new Vertex(200, 250)
            };
            return new Polygon(vertices);
        }

        private Polygon createpoli()
        {
            List<Vertex> vertices = new List<Vertex>
            {
                new Vertex(600, 100),
                new Vertex(700, 100),
                new Vertex(900, 300),
                new Vertex(800, 300),
                new Vertex(900, 350),
                new Vertex(950, 300),
                new Vertex(1000, 300)
            };
            return new Polygon(vertices);
        }

        private void AddConstraint(Space space, Polygon polygon, Constraint constraint)
        {
            space.AddConstraint(constraint);
            Vertex start = constraint.Edges.First().startVertex;
            ConstraintsChecker.MoveVertex(polygon, start, start.x, start.y);
        }
    }
}
