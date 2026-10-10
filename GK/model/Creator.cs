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

        public Polygon createTriangle()
        {
            List<Vertex> vertices = new List<Vertex>
            {
                new Vertex(100, 100),
                new Vertex(300, 100),
                new Vertex(200, 250)
            };
            return new Polygon(vertices);
        }

        public Polygon createpoli()
        {
            List<Vertex> vertices = new List<Vertex>
            {
                new Vertex(600, 100),
                new Vertex(700, 100),
                new Vertex(900, 300),
                new Vertex(800, 300)
            };
            return new Polygon(vertices);
        }

    }
}
