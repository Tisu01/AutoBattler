using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AlgorytmDijkstry
{
    
    //ZnajdŸ œcie¿kê w grafie - patrzebuje informacji o grafie, punkcie startowym i punkcie docelowym
    public static List<Node> FindPath(Graph graph, Node start, Node end)
    {
        //Dla ka¿dego Node'a w grafie tworzymy obiekt NodeData który bêdzie przechowywa³ informacje o œcie¿ce do niego
        NodeData[] nodeData = new NodeData[graph.Nodes.Count];
        //Uzupe³niamy wszystkie informacje o Node'ach podstawowymi danymi
        for (int i = 0; i < graph.Nodes.Count; i++)
        {
            nodeData[i] = new NodeData();
            nodeData[i].node = graph.Nodes[i];
            //jeœli ten Node to Node startowy to ustaw dystans do niego na 0 (w innym przypadku na nieskoñczonoœæ)
            nodeData[i].minDistance = (graph.Nodes[i] == start) ? 0 : int.MaxValue;
        }

        //Rozpoczynamy przeczesywanie grafu. Dla ka¿dego wêz³a w grafie wyznaczamy informacje (NodeData) o kosztach oraz œcie¿ce dotarcia do niego
        Dijkstra(nodeData, graph, start, end);

        //Przegl¹da dane o Node'ach (NodeData[]) i wyznacza optymaln¹ œcie¿kê od punktu start do punktu end
        List<Node> path = GetOptimalPath(nodeData, start, end);

        return path;
    }

    static void Dijkstra(NodeData[] nodeData, Graph graph, Node node, Node endOfPath)
    {
        //Pobieramy dane wezla
        NodeData daneWezla = GetNodeData(node, nodeData);

        //Wyznaczamy wagi podró¿y do wszystkich s¹siadów powy¿szego wêz³a
        foreach (Node neighbor in node.neighbors)
        {
            //Pobieramy dane s¹siada
            NodeData daneSasiada = GetNodeData(neighbor, nodeData);

            //Nie wracamy do odwiedzonych wêz³ów
            if (daneSasiada.czyOdwiedzony == true)
                continue;

            //Sprawdzamy czy nie stoi tam jednostka (chyba ¿e jest to Node koñcowy)
            if (neighbor.Ocupatebyunit == true && neighbor != endOfPath)
                continue;

            //Pobieramy wagê (koszt przejœcia) do s¹siada
            Edge edge = graph.GetEdge(node, neighbor);

            //Zabezpieczenie - Jeœli taka krawêdŸ nie istnieje to pomiñ
            if (edge == null)
                continue;

            //Jeœli koszt dotarcia do s¹siada jest krótszy ni¿ jego aktualna "najkrótsza œcie¿ka", to przekazujemy mu now¹ najkrósz¹ œcie¿kê
            if (daneWezla.minDistance + edge.GetWeight() < daneSasiada.minDistance)
            {
                //Przekazuje nowy najkrótszy dystans
                daneSasiada.minDistance = daneWezla.minDistance + edge.GetWeight();
                //Przekazuje ¿e najkrótsza droga przechodzi przez tego Node'a
                daneSasiada.predNode = node;
            }
        }

        //Droga do wszystkich s¹siadów zosta³a wyznaczona, wiêc
        //Oznaczamy wierzcho³ek jako odwiedzony, ¿eby algorytm ju¿ do niego nie wróci³
        daneWezla.czyOdwiedzony = true;

        //Rekurencyjnie powatarzamy wyznaczanie œcie¿ek dla wszystkich s¹siadów. Tak d³ugo, a¿ nie wype³nimy ca³ego grafu.
        foreach (Node neighbor in node.neighbors)
        {
            NodeData daneSasiada = GetNodeData(neighbor, nodeData);

            //Ale nie powtarzamy odwiedzonych Node'ów, ¿eby nie wpaœæ w nieskoñczon¹ pêtlê
            //Oraz nie 
            if (daneSasiada.czyOdwiedzony == false
                && daneSasiada.node.Ocupatebyunit == false)
                Dijkstra(nodeData, graph, neighbor, endOfPath);
        }
    }

    //Na podstawie NodeData[] wyznacza najkrósz¹ œcie¿kê pomiêdzy punktami start oraz end
    private static List<Node> GetOptimalPath(NodeData[] nodeData, Node start, Node end)
    {
        //Tworzymy pust¹ œcie¿kê któr¹ bêdziemy uzupe³niaæ
        List<Node> path = new List<Node>();

        //dodajemy ostatni Node do listy, poniewa¿ wyznaczanie œcie¿ki odbywa siê w odwrotnej kolejnoœci
        //od punktu koñcowego (end) do punktu pocz¹tkowego (start)
        path.Add(end);

        //Wykonujemy pêtlê tak d³ugo dopóki nie wrócimy do punktu start
        while (path[path.Count - 1] != start)
        {
            //Pobieramy NodeData ostatniego wêz³a na œcie¿ce
            NodeData dane = GetNodeData(path[path.Count - 1], nodeData);

            //Debug.Log("<color=red>Wyznaczanie sciezki: </color>" + dane.node.index + " poprzednik: " + dane.predNode);

            //Pobieramy jego poprzednika
            if (dane.predNode != null)
                path.Add(dane.predNode);
            else // jeœli poprzednik nie istnieje to oznacza ¿e nie jesteœmy w stanie wyznaczyæ œcie¿ki i zwracamy pust¹ œcie¿kê
                return new List<Node>();
        }

        //Na koñcu odwracamy œcie¿kê i zwracamy
        path.Reverse();
        return path;
    }

    //Dla podanego Node'a zwraca jego NodeData
    private static NodeData GetNodeData(Node node, NodeData[] nodeData)
    {
        for (int i = 0; i < nodeData.Length; i++)
        {
            if (nodeData[i].node == node)
                return nodeData[i];
        }

        return null;
    }


    class NodeData
    {
        //Node którego poni¿sze dane dotycz¹
        public Node node = null;
        //dystans do tego Node'a od wêz³a pocz¹tkowego
        public float minDistance = float.MaxValue;
        //poprzednik w œcie¿ce (okreœla jak trafiæ do tego Node'a najkrótsz¹ œcie¿k¹ od wêz³a pocz¹tkowego)
        public Node predNode = null;
        //czy algorytm ju¿ sprawdzi³ ten Node
        public bool czyOdwiedzony = false;
    };
}
