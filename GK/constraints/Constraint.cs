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
        // poprawia krawędź edge: fixedVertex zostaje w miejscu, rusza się drugi koniec krawędzi
        public abstract void Fix(Edge edge, Vertex fixedVertex);
        // czy poprawianie może poczekać, aż dojdziemy do nextEdge (domyślnie nie)
        public virtual bool CanWaitForFix(Edge nextEdge)
        {
            return false;
        }
    }
}
