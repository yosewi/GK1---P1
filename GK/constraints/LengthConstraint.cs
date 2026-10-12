using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.constraints
{
    public class LengthConstraint : Constraint
    {
        public Edge edge { get; }
        public double length { get; set; }

        public LengthConstraint(Edge edge, double length)
        {
            this.edge = edge;
            this.length = length;
        }

        public override IEnumerable<Edge> Edges => new[] { edge };

        public override string Label => $"{length:0}";
    }
}