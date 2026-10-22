using GK.algorithms;
using GK.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.objects
{
    public class Vertex
    {
        public double x { get; set; }
        public double y { get; set; }
        public Edge firstEdge { get; set; } // krawędź wchodząca do wierzchołka
        public Edge secondEdge { get; set; } // krawędź wychodząca z wierzchołka
        public double oldX { get; set; }
        public double oldY { get; set; }
        public Continuity continuity { get; set; } = Continuity.C1;
        public bool continuitySetByUser { get; set; } = false;
        public bool HasBezierNeighbour => firstEdge.type == EdgeType.Bezier || secondEdge.type == EdgeType.Bezier;

        public Vertex(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public void setLocation(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public bool IsHit(double x, double y, double radius)
        {
            return Geometry.Distance(this.x, this.y, x, y) <= radius;
        }

        public bool CanBeG1()
        {
            // krawędź wchodząca jest łukiem -> jego drugi koniec to jej startVertex
            if (firstEdge.type == EdgeType.Arc && firstEdge.startVertex.continuity == Continuity.G1)
            {
                return false;
            }
            // krawędź wychodząca jest łukiem -> jego drugi koniec to jej endVertex
            if (secondEdge.type == EdgeType.Arc && secondEdge.endVertex.continuity == Continuity.G1)
            {
                return false;
            }
            return true;
        }

        // krawędź po drugiej stronie wierzchołka niż podana
        public Edge EdgeOnOtherSide(Edge edge)
        {
            if (edge == firstEdge)
            {
                return secondEdge;
            }
            return firstEdge;
        }

        public bool HasArcNeighbour => firstEdge.type == EdgeType.Arc || secondEdge.type == EdgeType.Arc;
        public bool HasCurveNeighbour => HasBezierNeighbour || HasArcNeighbour;

        public void SetContinuity(Continuity continuity)
        {
            this.continuity = continuity;
            continuitySetByUser = true;
            ContinuityFixer.FixVertex(this);
        }
    }
}
