using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesGrid
{
    // The Grid itself
    public Tile[,] grid { get; protected set; }

    // Grid's center
    public Tile centerNode { get; protected set; }
    public Vector3 centerPosition { get; protected set; }

    // Size of the grid of nodes
    public int gridSize { get; protected set; }

    // Diamater and radius of nodes.
    public float nodeRadius { get; protected set; }
    public float nodeDiameter { get; protected set; }

    // Bottom left corner of the grid on the World
    public Vector3 worldBottomLeft { get; protected set; }

    public TilesGrid(float worldSize) {

        // Sets up grid sizes based on world input
        SizeSetup(worldSize);

        // Creates matrix of nodes
        grid = new Tile[gridSize, gridSize];
    }

    public void SizeSetup(float worldSize)
    {
        // Sets up grid size
        nodeRadius = 0.5f;
        nodeDiameter = nodeRadius * 2;
        Vector2 gridWorldSize = new Vector2(1, 1) * (worldSize + 2);
        gridSize = Mathf.FloorToInt(gridWorldSize.x / nodeDiameter);
    }

    /// <summary>
    /// Returns the total number of nodes on the grid
    /// </summary>
    public int GridArea
    {
        get
        {
            return gridSize * gridSize;
        }
    }

    /// <summary>
    /// Returns a list of neighbours of a given node.
    /// </summary>
    public List<Tile> GetNeighbours(Tile node)
    {
        List<Tile> neighbours = new List<Tile>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSize && checkY >= 0 && checkY < gridSize)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    /// <summary>
    /// Returns a node from given world position.
    /// </summary>
    public Tile GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridSize / 2) / gridSize;
        float percentY = (worldPosition.y + gridSize / 2) / gridSize;
        int x = Mathf.RoundToInt((gridSize * percentX) - 1);
        int y = Mathf.RoundToInt((gridSize * percentY) - 1);
        if (grid[x,y] == null) {
            Debug.LogError("NodeFromWorldPoint at" + worldPosition + " returned NULL");
            return grid[0, 0];
        }
        return grid[x, y];
    }

    public Vector3 NodeWorldPos(Tile node) {
        return worldBottomLeft + new Vector3(nodeRadius, nodeRadius, 0f)
            + Vector3.right * (node.gridX * nodeDiameter + nodeRadius) 
            + Vector3.up * (node.gridY * nodeDiameter + nodeRadius);
    }

    public Tile GetRandomTile() {
        return grid[Random.Range(1, gridSize - 1),Random.Range(1, gridSize - 1)];
    }


    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}
