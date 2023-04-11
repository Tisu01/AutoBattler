using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    public Transform tileColliderParent;
    public TileColider tileColiderPrefab;
    public Tilemap grid;
    public NeighborMode neighborMode;
    public GameManager gameManager;
    public Graph graph;

    public List<TileColider> tileColliders = new List<TileColider>();


    public void GenerateGraph()
    {
        InitializeGraph();
    }

    private void InitializeGraph()
    {
        graph = new Graph();
        InitNodes();
        InitEdges();
    }

    private void InitNodes()
    {
        for (int x = grid.cellBounds.xMin; x < grid.cellBounds.xMax; x++)
        {
            for (int y = grid.cellBounds.yMin; y < grid.cellBounds.yMax; y++)
            {
                Vector3Int tileCoordinates = new Vector3Int(x, y, (int)grid.transform.position.y);

                //Sprawdz czy na tych wspolrzednych miesci sie Tile
                if (grid.HasTile(tileCoordinates) == true)
                {
                    CustomTile customTile = grid.GetTile(tileCoordinates) as CustomTile;
                    TileData tileData = null;

                    bool createNode = true;

                    if (customTile != null)
                    {
                        tileData = customTile.tileData;

                        if (tileData.unitCanEnter == false)
                        {
                            createNode = false;
                        }
                    }

                    if (createNode == true)
                    {
                        float cellSizeX = grid.cellSize.x / 2;
                        float cellSizeY = grid.cellSize.y / 2;
                        Vector3 offset = new Vector3(cellSizeX, cellSizeY, 0);

                        Vector3 worldPosition = grid.CellToWorld(tileCoordinates) + offset;

                        Node node = graph.AddNode(worldPosition, tileData);

                        if (node.connectedTileData.teamSlot == TeamSlot.team1)
                            SpawnTileColider(node);
                    }
                }
            }
        }
    }

    private void SpawnTileColider(Node node)
    {
        TileColider tileColider = Instantiate(tileColiderPrefab, tileColliderParent);
        tileColider.transform.position = node.worldPosition;

        tileColider.InitTileCollider(node, grid.cellSize, gameManager);

        tileColliders.Add(tileColider);
    }

    private void InitEdges()
    {
        List<Node> allNodes = graph.Nodes;

        foreach (Node from in allNodes)
        {
            foreach (Node to in allNodes)
            {
                if (from == to) continue;

                float maxDistanceToNeigbor;

                if (neighborMode == NeighborMode.direct)
                    maxDistanceToNeigbor = 3;
                else
                    maxDistanceToNeigbor = 4;

                if (Vector3.Distance(from.worldPosition, to.worldPosition) < maxDistanceToNeigbor)
                {
                    graph.AddEdge(from, to);
                    from.AddNeighbour(to);
                }
            }
        }
    }

    public void DestroyTileColliders()
    {
        for (int i = 0; i < tileColliders.Count; i++)
        {
            Destroy(tileColliders[i].gameObject);
        }

        tileColliders.Clear();
    }


    private void OnDrawGizmos()
    {
        if (graph == null)
            return;

        List<Edge> allEdges = graph.Edges;

        //foreach (Edge e in allEdges)
        //{
        //    Debug.DrawLine(e.from.worldPosition, e.to.worldPosition, Color.black, 1);
        //}

        List<Node> allNodes = graph.Nodes;

        foreach (Node n in allNodes)
        {
            Gizmos.color = n.Ocupatebyunit ? Color.red : Color.green;
            Gizmos.DrawSphere(n.worldPosition, 0.3f);
        }

    }

    public enum NeighborMode
    {
        direct,
        diagonal
    }
}
