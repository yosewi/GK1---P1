using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK.enums;
using GK.objects;

namespace GK.algorithms
{
    // Pilnuje ciągłości (G1, C1) w wierzchołkach obok Beziera: ustawia punkt kontrolny przy wierzchołku
    // tak, żeby wektor styczny Beziera pasował do wektora stycznego krawędzi po drugiej stronie.
    // G1 - ten sam kierunek stycznych, C1 - ten sam wektor (kierunek i długość). Przy G0 nic nie robimy.
    // ogólnie rusza tylko punkty kontrolne Bezierów, nie rusza wierzchołków.
    internal static class ContinuityFixer
    {
        private const double TangentVectorToHandleRatio = 3; // bo P′(0) = 3(V1 − V0)

        // Przywraca ciągłość w wierzchołku v - dopasowuje się zawsze tylko jedna strona:
        // jak Bezier wychodzi z v (czyli jest jego secondEdge), to dopasowuje się do krawędzi wchodzącej,
        // wpw Bezier wchodzący dopasowuje się do krawędzi wychodzącej.
        // Przy dwóch Bezierach dopasowuje się wychodzący.
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

        // ustawia controlPoint1 wychodzącego Beziera tak, żeby pasował do krawędzi wchodzącej
        public static void FitOutgoingControlPoint(Vertex v)
        {
            // nic nie robimy przy G0, gdy krawędź wychodząca nie jest Bezierem albo gdy obok jest łuk (wtedy to łuk dopasowuje się do Beziera)
            if (v.continuity == Continuity.G0 || v.secondEdge.type != EdgeType.Bezier || v.HasArcNeighbour)
            {
                return;
            }

            // jaka ma być styczna Beziera w v (na podstawie stycznej krawędzi wchodzącej) -> gdzie postawić controlPoint1
            (double x, double y) requiredTangent = RequiredTangentVector(v.continuity, TangentVectorOfIncomingEdge(v), TangentVectorOfOutgoingEdge(v));
            (double x, double y) newPosition = PositionOfOutgoingControlPoint(v, requiredTangent);

            v.secondEdge.controlPoint1!.x = newPosition.x;
            v.secondEdge.controlPoint1!.y = newPosition.y;
        }

        // ustawia controlPoint2 wchodzącego Beziera tak, żeby pasował do krawędzi wychodzącej
        public static void FitIncomingControlPoint(Vertex v)
        {
            // nic nie robimy przy G0, gdy krawędź wychodząca nie jest Bezierem albo gdy obok jest łuk (wtedy to łuk dopasowuje się do Beziera)
            if (v.continuity == Continuity.G0 || v.firstEdge.type != EdgeType.Bezier || v.HasArcNeighbour)
            {
                return;
            }

            // jaka ma być styczna Beziera w v (na podstawie stycznej krawędzi wychodzącej) -> gdzie postawić controlPoint
            (double x, double y) requiredTangent = RequiredTangentVector(v.continuity, TangentVectorOfOutgoingEdge(v), TangentVectorOfIncomingEdge(v));
            (double x, double y) newPosition = PositionOfIncomingControlPoint(v, requiredTangent);

            v.firstEdge.controlPoint2!.x = newPosition.x;
            v.firstEdge.controlPoint2!.y = newPosition.y;
        }

        // jaka ma byc styczna po naszej stronie
        // tangentOnOtherSide - styczna krawędzi po drugiej stronie wierzchołka, currentTangent - obecna styczna Beziera
        private static (double x, double y) RequiredTangentVector(Continuity continuity, (double x, double y) tangentOnOtherSide, (double x, double y) currentTangent)
        {
            // dokładnie styczna z drugiej strony
            if (continuity == Continuity.C1)
            {
                return tangentOnOtherSide;
            }

            if (Geometry.VectorLength(tangentOnOtherSide) < 1e-9)
            {
                return currentTangent;
            }

            // G1: kierunek z drugiej strony, dlugosc obecnej stycznej. Punkt kontrolny
            // obraca sie wokol wierzzcholka zachowujac odleglosc
            return Geometry.SameDirectionWithLength(tangentOnOtherSide, Geometry.VectorLength(currentTangent));
        }

        // styczna krawędzi wchodzącej wierzchołka v
        private static (double x, double y) TangentVectorOfIncomingEdge(Vertex v)
        {
            return v.firstEdge.TangentVectorArrivingAtEnd();
        }

        // styczna krawędzi wychodzącej wierzchołka v
        private static (double x, double y) TangentVectorOfOutgoingEdge(Vertex v)
        {
            return v.secondEdge.TangentVectorLeavingStart();
        }

        // gdzie stoi controlPoint1, żeby styczna w początku była requiredTangent
        private static (double x, double y) PositionOfOutgoingControlPoint(Vertex v, (double x, double y) requiredTangent)
        {
            return (v.x + requiredTangent.x / TangentVectorToHandleRatio, v.y + requiredTangent.y / TangentVectorToHandleRatio);
        }

        // gdzie stoi controlPoint2, żeby styczna w końcu była requiredTangent
        private static (double x, double y) PositionOfIncomingControlPoint(Vertex v, (double x, double y) requiredTangent)
        {
            return (v.x - requiredTangent.x / TangentVectorToHandleRatio, v.y - requiredTangent.y / TangentVectorToHandleRatio);
        }
    }
}
