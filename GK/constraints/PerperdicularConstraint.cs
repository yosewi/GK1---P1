using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.constraints
{
    public class PerpendicularConstraint : Constraint
    {
        private static int nextId = 1;

        public int id { get; }
        public Edge first { get; }
        public Edge second { get; }

        public PerpendicularConstraint(Edge first, Edge second)
        {
            this.first = first;
            this.second = second;
            id = nextId++;
        }

        public override IEnumerable<Edge> Edges => new[] { first, second };

        public override string Label => $"⊥{id}";
    }
}