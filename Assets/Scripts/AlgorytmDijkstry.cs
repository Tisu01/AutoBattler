using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AlgorytmDijkstry
{
    
    //Znajd� �cie�k� w grafie - patrzebuje informacji o grafie, punkcie startowym i punkcie docelowym
    public static List<Node> FindPath(Graph graph, Node start, Node end)
    {
        //Dla ka�dego Node'a w grafie tworzymy obiekt NodeData kt�ry b�dzie przechowywa� informacje o �cie�ce do niego
        NodeData[] nodeData = new NodeData[graph.Nodes.Count];
        //Uzupe�niamy wszystkie informacje o Node'ach podstawowymi danymi
        for (int i = 0; i < graph.Nodes.Count; i++)
        {
            nodeData[i] = new NodeData();
            nodeData[i].node = graph.Nodes[i];
            //je�li ten Node to Node startowy to ustaw dystans do niego na 0 (w innym przypadku na niesko�czono��)
            nodeData[i].minDistance = (graph.Nodes[i] == start) ? 0 : int.MaxValue;
        }

        //Rozpoczynamy przeczesywanie grafu. Dla ka�dego w�z�a w grafie wyznaczamy informacje (NodeData) o kosztach oraz �cie�ce dotarcia do niego
        Dijkstra(nodeData, graph, start, end);

        //Przegl�da dane o Node'ach (NodeData[]) i wyznacza optymaln� �cie�k� od punktu start do punktu end
        List<Node> path = GetOptimalPath(nodeData, start, end);

        return path;
    }

    static void Dijkstra(NodeData[] nodeData, Graph graph, Node node, Node endOfPath)
    {
        //Pobieramy dane wezla
        NodeData daneWezla = GetNodeData(node, nodeData);

        //Wyznaczamy wagi podr�y do wszystkich s�siad�w powy�szego w�z�a
        foreach (Node neighbor in node.neighbors)
        {
            //Pobieramy dane s�siada
            NodeData daneSasiada = GetNodeData(neighbor, nodeData);

            //Nie wracamy do odwiedzonych w�z��w
            if (daneSasiada.czyOdwiedzony == true)
                continue;

            //Sprawdzamy czy nie stoi tam jednostka (chyba �e jest to Node ko�cowy)
            if (neighbor.Ocupatebyunit == true && neighbor != endOfPath)
                continue;

            //Pobieramy wag� (koszt przej�cia) do s�siada
            Edge edge = graph.GetEdge(node, neighbor);

            //Zabezpieczenie - Je�li taka kraw�d� nie istnieje to pomi�
            if (edge == null)
                continue;

            //Je�li koszt dotarcia do s�siada jest kr�tszy ni� jego aktualna "najkr�tsza �cie�ka", to przekazujemy mu now� najkr�sz� �cie�k�
            if (daneWezla.minDistance + edge.GetWeight() < daneSasiada.minDistance)
            {
                //Przekazuje nowy najkr�tszy dystans
                daneSasiada.minDistance = daneWezla.minDistance + edge.GetWeight();
                //Przekazuje �e najkr�tsza droga przechodzi przez tego Node'a
                daneSasiada.predNode = node;
            }
        }

        //Droga do wszystkich s�siad�w zosta�a wyznaczona, wi�c
        //Oznaczamy wierzcho�ek jako odwiedzony, �eby algorytm ju� do niego nie wr�ci�
        daneWezla.czyOdwiedzony = true;

        //Rekurencyjnie powatarzamy wyznaczanie �cie�ek dla wszystkich s�siad�w. Tak d�ugo, a� nie wype�nimy ca�ego grafu.
        foreach (Node neighbor in node.neighbors)
        {
            NodeData daneSasiada = GetNodeData(neighbor, nodeData);

            //Ale nie powtarzamy odwiedzonych Node'�w, �eby nie wpa�� w niesko�czon� p�tl�
            //Oraz nie 
            if (daneSasiada.czyOdwiedzony == false
                && daneSasiada.node.Ocupatebyunit == false)
                Dijkstra(nodeData, graph, neighbor, endOfPath);
        }
    }

    //Na podstawie NodeData[] wyznacza najkr�sz� �cie�k� pomi�dzy punktami start oraz end
    private static List<Node> GetOptimalPath(NodeData[] nodeData, Node start, Node end)
    {
        //Tworzymy pust� �cie�k� kt�r� b�dziemy uzupe�nia�
        List<Node> path = new List<Node>();

        //dodajemy ostatni Node do listy, poniewa� wyznaczanie �cie�ki odbywa si� w odwrotnej kolejno�ci
        //od punktu ko�cowego (end) do punktu pocz�tkowego (start)
        path.Add(end);

        //Wykonujemy p�tl� tak d�ugo dop�ki nie wr�cimy do punktu start
        while (path[path.Count - 1] != start)
        {
            //Pobieramy NodeData ostatniego w�z�a na �cie�ce
            NodeData dane = GetNodeData(path[path.Count - 1], nodeData);

            //Debug.Log("<color=red>Wyznaczanie sciezki: </color>" + dane.node.index + " poprzednik: " + dane.predNode);

            //Pobieramy jego poprzednika
            if (dane.predNode != null)
                path.Add(dane.predNode);
            else // je�li poprzednik nie istnieje to oznacza �e nie jeste�my w stanie wyznaczy� �cie�ki i zwracamy pust� �cie�k�
                return new List<Node>();
        }

        //Na ko�cu odwracamy �cie�k� i zwracamy
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
        //Node kt�rego poni�sze dane dotycz�
        public Node node = null;
        //dystans do tego Node'a od w�z�a pocz�tkowego
        public float minDistance = float.MaxValue;
        //poprzednik w �cie�ce (okre�la jak trafi� do tego Node'a najkr�tsz� �cie�k� od w�z�a pocz�tkowego)
        public Node predNode = null;
        //czy algorytm ju� sprawdzi� ten Node
        public bool czyOdwiedzony = false;
    };
}
