using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK.enums;
using GK.objects;

namespace GK.algorithms
{
    internal static class ContinuityFixer
    {
        private const double TangentVectorToHandleRatio = 3; // bo P′(0) = 3(V1 − V0)

        // jak bezier wychodzi z v (czyli jest jego secondEdge) to dopasowuje sie do krawedzi wchodzacej
        // wpw dopasowuje sie do krawedzi wychodzącej
        // przy dwoch bezierach dopasowuje się wchodzący
        public static void FixVertex(Vertex v)
        {
            if (v.secondEdge.type == EdgeType.Bezier)
            {
                FitOutgoingControlPoint(v);
            }
            else if (v.firstEdge.type == EdgeType.Bezier)
            {
                FitIncomingControlPoint(v);
            }
        }

        // wywoluje FixVertex w kazdym wierzcholku. Wywolywane po udanym algorytmie sprawdzajacym
        // ograniczenia i po dodaniu/usunieciu wierzcholka
        public static void FixPolygon(Polygon polygon)
        {
            foreach (Vertex v in polygon.vertices)
            {
                FixVertex(v);
            }
        }

        // dopasowuje punkt kontrolny krawędzi edge, który leży przy wierzchołku v
        public static void FitControlPointOfEdgeAtVertex(Edge edge, Vertex v)
        {
            if (edge == v.secondEdge)
            {
                FitOutgoingControlPoint(v);
            }
            else
            {
                FitIncomingControlPoint(v);
            }
        }

        // ustawia controlPoint1 wychodzącego Béziera tak, żeby pasował do krawędzi wchodzącej
        public static void FitOutgoingControlPoint(Vertex v)
        {
            // nic nie robimy przy G0, gdy krawędź wychodząca nie jest Bézierem albo gdy obok jest łuk (wtedy to łuk dopasowuje się do Béziera)
            if (v.continuity == Continuity.G0 || v.secondEdge.type != EdgeType.Bezier || v.HasArcNeighbour)
            {
                return;
            }

            // oblicza styczną i nową pozycję na podstawie stycznej
            (double x, double y) requiredTangent = RequiredTangentVector(v.continuity, TangentVectorOfIncomingEdge(v), TangentVectorOfOutgoingEdge(v));
            (double x, double y) newPosition = PositionOfOutgoingControlPoint(v, requiredTangent);

            v.secondEdge.controlPoint1!.x = newPosition.x;
            v.secondEdge.controlPoint1!.y = newPosition.y;
        }

        // ustawia controlPoint2 wchodzącego Béziera tak, żeby pasował do krawędzi wychodzącej
        public static void FitIncomingControlPoint(Vertex v)
        {
            // nic nie robimy przy G0, gdy krawędź wychodząca nie jest Bézierem albo gdy obok jest łuk (wtedy to łuk dopasowuje się do Béziera)
            if (v.continuity == Continuity.G0 || v.firstEdge.type != EdgeType.Bezier || v.HasArcNeighbour)
            {
                return;
            }

            // oblicza styczną i nową pozycję na podstawie stycznej
            (double x, double y) requiredTangent = RequiredTangentVector(v.continuity, TangentVectorOfOutgoingEdge(v), TangentVectorOfIncomingEdge(v));
            (double x, double y) newPosition = PositionOfIncomingControlPoint(v, requiredTangent);

            v.firstEdge.controlPoint2!.x = newPosition.x;
            v.firstEdge.controlPoint2!.y = newPosition.y;
        }

        // jaka ma byc styczna po naszej stronie
        private static (double x, double y) RequiredTangentVector(Continuity continuity, (double x, double y) tangentOnOtherSide, (double x, double y) currentTangent)
        {
            // dokladnie styczna z drugiej strony
            if (continuity == Continuity.C1)
            {
                return tangentOnOtherSide;
            }

            // G1: kierunek z drugiej strony, dlugosc obecnej stycznej. Punkt kontrolny
            // obraca sie wokol wierzzcholka zachowujac odleglosc
            if (Geometry.VectorLength(tangentOnOtherSide) < 1e-9)
            {
                return currentTangent;
            }

            return Geometry.SameDirectionWithLength(tangentOnOtherSide, Geometry.VectorLength(currentTangent));
        }

        // przekazuje pytanie o styczna do krawedzi
        private static (double x, double y) TangentVectorOfIncomingEdge(Vertex v)
        {
            return v.firstEdge.TangentVectorArrivingAtEnd();
        }

        // przekazuje pytanie o styczna do krawedzi
        private static (double x, double y) TangentVectorOfOutgoingEdge(Vertex v)
        {
            return v.secondEdge.TangentVectorLeavingStart();
        }

        // pozycja punktu kontrolnego w zaleznosci od ciaglosci. Dla C1 stosunek 1/3
        // dla G1 odleglosc zostaje, zmienia sie tylko kierunek
        private static (double x, double y) PositionOfOutgoingControlPoint(Vertex v, (double x, double y) requiredTangent)
        {
            return (v.x + requiredTangent.x / TangentVectorToHandleRatio, v.y + requiredTangent.y / TangentVectorToHandleRatio);
        }

        // pozycja punktu kontrolnego w zaleznosci od ciaglosci. Dla C1 stosunek 1/3
        // dla G1 odleglosc zostaje, zmienia sie tylko kierunek
        private static (double x, double y) PositionOfIncomingControlPoint(Vertex v, (double x, double y) requiredTangent)
        {
            return (v.x - requiredTangent.x / TangentVectorToHandleRatio, v.y - requiredTangent.y / TangentVectorToHandleRatio);
        }
    }
}
