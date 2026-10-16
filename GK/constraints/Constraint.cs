using GK.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.constraints
{
    public abstract class Constraint
    {
        public abstract IEnumerable<Edge> Edges { get; }
        public abstract string Label { get; }
        public abstract bool IsDone();
        public abstract void Fix(Edge edge, Vertex fixedVertex);
    }
}
