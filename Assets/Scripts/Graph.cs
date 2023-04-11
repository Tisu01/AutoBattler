using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Graph
{
    private List<Node> nodes;
    private List<Edge> edges;

    public List<Node> Nodes { get => nodes; private set => nodes = value; }
    public List<Edge> Edges { get => edges; private set => edges = value; }

    public Dictionary<Team, List<Node>> unitSlots;

    public Graph()
    {
        Nodes = new List<Node>();
        Edges = new List<Edge>();

        unitSlots = new Dictionary<Team, List<Node>>();

        unitSlots.Add(Team.Team1, new List<Node>());
        unitSlots.Add(Team.Team2, new List<Node>());
    }

    public Node AddNode(Vector3 worldPosition, TileData tileData)
    {
        Node createdNode = new Node(Nodes.Count, worldPosition, tileData);

        Nodes.Add(createdNode);

        if(tileData != null)
        {
            if (tileData.teamSlot == TeamSlot.team1 || tileData.teamSlot == TeamSlot.both)
            {
                unitSlots[Team.Team1].Add(createdNode);
            }

            if (tileData.teamSlot == TeamSlot.team2 || tileData.teamSlot == TeamSlot.both)
            {
                unitSlots[Team.Team2].Add(createdNode);
            }
        }
        return createdNode;
    }

    public void AddEdge(Node from, Node to)
    {
        //Weight is always 1 !!!!
        edges.Add(new Edge(from, to, 1));
    }

    //dijkstra algorithm this method give us information about weight 
    public Edge GetEdge(Node from, Node to)
    {
        foreach(Edge edge in Edges)
        {
            if ( edge.from == from || edge.to == to)
                return edge;
        }

        return null;
    }
}

public class Node
{
    //Fields
    public int index;
    public Vector3 worldPosition;
    private Unit ocupatebyunit = null;
    public List<Node> neighbors;

    public TileData connectedTileData;

    //Properties
    public Unit Ocupatebyunit
    {
        get => ocupatebyunit;
        set
        {
            ocupatebyunit = value;
        }
    }

    //Metods
    public Node(int index, Vector3 worldPosition, TileData tileData)
    {
        this.index = index;
        this.worldPosition = worldPosition;
        connectedTileData = tileData;
        neighbors = new List<Node>();
    }

    public void AddNeighbour(Node neighbour)
    {
        if (neighbors.Contains(neighbour) == false)
            neighbors.Add(neighbour);
    }

    public void RemoveNeighbour(Node neighbour)
    {
        if (neighbors.Contains(neighbour) != false)
            neighbors.Remove(neighbour);
    }

    public bool IsNeighbor(Node node)
    {
        return neighbors.Contains(node);
    }
}

public class Edge
{
    public Node from;
    public Node to;
    private float weight;

    public Edge(Node from, Node to, float weight)
    {
        this.from = from;
        this.to = to;
        this.weight = weight;
    }

    public float GetWeight()
    {
        //if (to.Ocupatebyunit != null)
        //{
        //    return Mathf.Infinity;
        //}

        return weight;
    }
}