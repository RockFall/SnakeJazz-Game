using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deals with Grid Generation
/// </summary>
public class GridGenerator : MonoBehaviour
{
    // Grid and map DATA
    public TilesGrid gridData { get; protected set; }
    MapGenerator mapData;

    // For Debug Only
    public bool displayGridGizmos;

    // Walkability related variables
    public TilesGrid.TerrainType[] walkableRegions;
    LayerMask walkableMask;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

    // World bottom left of grid Game Object
    public Vector3 worldBottomLeft { get; protected set; }

    // Start is called before the first frame update
    void Awake()
    {
        // Sets up walkable regions
        foreach (TilesGrid.TerrainType region in walkableRegions) {
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }
    }

    public void CreateGrid() {
        // Creates a grid instance
        gridData = new TilesGrid(mapData.worldSize);

        // Gets grid size
        float gridSize = gridData.gridSize;

        // Gets bottom left corner
        worldBottomLeft = (transform.position - Vector3.right * (gridSize) / 2 - Vector3.up * (gridSize) / 2);

        // Creates each tile on the grid
        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                // Current tile world position
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * gridData.nodeDiameter + gridData.nodeRadius) + Vector3.up * (y * gridData.nodeDiameter + gridData.nodeRadius);

                // Make border be unwalkable
                bool isBorder = false;
                if (x == gridSize - 1 || y == gridSize - 1 || x == 0 || y == 0) {
                    isBorder = true;
                }
                int movementPenalty = 0;

                gridData.grid[x, y] = new Tile(x, y, movementPenalty, isBorder, worldPoint);
            }
        }
    }

    public void UpdateGrid() {
        // Gets grid size
        float gridSize = gridData.gridSize;

        // Loop throuhg each tile on the grid
        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                // Current tile
                Tile tile = gridData.grid[x, y];

                // Make obstacle be unwalkable


                //gridData.grid[x, y] = new Tile(x, y, movementPenalty, isBorder, worldPoint);
            }
        }
    }


    // FOR DEGUB -- Display of the nodes grid
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridData.gridSize, gridData.gridSize, 1));

        if (gridData.grid != null && displayGridGizmos) {
            foreach (Tile n in gridData.grid) {
                if (n.HasFood()) {
                    Gizmos.color = Color.yellow;
                } else {
                    Gizmos.color = (n.Walkable()) ? Color.white : Color.red;
                }
                Gizmos.DrawCube(n.worldPos, Vector3.one * (gridData.nodeDiameter - .1f));
            }
        }
    }
}
