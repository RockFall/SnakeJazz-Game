using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deals with all visual map matters
/// </summary>
public class MapGenerator : MonoBehaviour
{
    // Grid DATA
    public GridGenerator gridGen { get; protected set; }

    // The world GO
    GameObject world;

    // Size of the world
    public float worldSize { get; protected set; }

    // Inputable world size range
    public Vector2 SizeOfTheWorldRange;

    public GameObject empty;

    public Biome[] biomes;
    public GameObject[] foodPrefabs;

    // All obstancles and foods in the game now
    List<GameObject> existingObstacles = new List<GameObject>();
    List<GameObject> existingFoods = new List<GameObject>();

    // Node/Tile Map
    Dictionary<Tile, GameObject> tileGameObjectMap = new Dictionary<Tile, GameObject>();

    // Current Level
    int level = 0;

    void Awake()
    {
        // Get CustomGrid reference
        gridGen = FindObjectOfType<GridGenerator>();

        // Sets up map
        SetupMap();
    }

    void Update()
    {
        
    }

    public void SetupMap()
    {
        // Checks if there's already a world instance
        if (world)
        {
            Destroy(world);
        }

        // Random Selection of biome, border layout and size
        int biomeIndex = 0;
        if (biomes.Length > 1)
            biomeIndex = Random.Range(0, biomes.Length-1);

        Biome biomeSelected = biomes[biomeIndex];
        int borderIndex = Random.Range(0, biomeSelected.borderPrefabs.Length-1);

        worldSize = Random.Range(SizeOfTheWorldRange.x,SizeOfTheWorldRange.y);
        if (worldSize % 2 != 0) {
            worldSize -= 1;
        }

        // Creation of the grid
        gridGen.CreateGrid();

        // Creates the World GO with selected border
        world = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, this.transform);
        world.name = "World";

        GameObject map = Instantiate(biomeSelected.borderPrefabs[borderIndex], 
            new Vector3(gridGen.gridData.nodeRadius, gridGen.gridData.nodeRadius, 0f), Quaternion.identity, world.transform);
        map.GetComponent<SpriteRenderer>().size = new Vector2(worldSize + 1f, worldSize + 1f);
        map.name = "Map";

        // Generates obstacles and the ground
        GenerateObstacles(biomeIndex);
        GenerateGround(biomeIndex);
        GenerateFood(true);
    }

    void GenerateObstacles(int biomeIndex)
    {
        // Erases any existing obstacle
        if (existingObstacles.Count > 0) {
            foreach (GameObject obstacle in existingObstacles) {
                Destroy(obstacle);
            }
        }

        // Generates a random number of obstacles on the map
        int randNumberOfObstacles = Random.Range(0, Mathf.FloorToInt(worldSize / 4));
        if (randNumberOfObstacles < Mathf.FloorToInt(worldSize / 4))
            randNumberOfObstacles = Mathf.FloorToInt(worldSize / 4);

        // Creates each obstacle
        for (int i = 0; i < randNumberOfObstacles; i++) {

            // Selects which tree is going to be placed from Prefabs
            int treeIndex = 0;
            if (biomes[biomeIndex].treePrefabs.Length > 1) {
                treeIndex = Random.Range(0, biomes[biomeIndex].treePrefabs.Length-1);
            } else if (biomes[biomeIndex].treePrefabs.Length == 0) {
                Debug.LogError("No trees on biome: " + biomes[biomeIndex].name);
            }

            // Gets a random node
            Tile tile = gridGen.gridData.grid[Random.Range(1, gridGen.gridData.gridSize - 1), Random.Range(1, gridGen.gridData.gridSize - 1)];

            // Check if it doesn't already have an obstacle
            if (tile.hasObstacle == false) {
                // Checks if there is a minimun number of walkable neighbours around the obstacle
                int notWalkableCount = 0;
                foreach (Tile neighbour in gridGen.gridData.GetNeighbours(tile)) {
                    if (!neighbour.Walkable()) {
                        notWalkableCount++;
                    }
                }
                if (notWalkableCount > 2) {
                    // It has too many other obstacles around, get another one
                    i--;
                    continue;

                } else {
                    // It's all okay, create the obstacle and update the tile
                    GameObject tree = Instantiate(biomes[biomeIndex].treePrefabs[treeIndex], gridGen.gridData.NodeWorldPos(tile), Quaternion.identity, world.transform);
                    existingObstacles.Add(tree);
                    tile.hasObstacle = true;
                }
            } else {
                // This tile already has an obstacle, find another one
                i--;
                continue;

            }
        }
    }

    void GenerateGround(int biomeIndex)
    {
        // Erases any existing tile
        if (tileGameObjectMap.Count > 0) {
            tileGameObjectMap.Clear();
        }

        // Creates Tiles and add them to TileNode Map
        foreach (Tile tile in gridGen.gridData.grid)
        {
            // Check if is not a border
            if (!tile.isBorder)
            {
                // Creates tile GO
                GameObject newGround = Instantiate(
                    empty, 
                    new Vector3(tile.worldPos.x, tile.worldPos.y, -1f), 
                    Quaternion.identity, 
                    world.transform
                );

                // Adds this tile GO to the Dictionary and List
                tileGameObjectMap.Add(tile, newGround);

                // Selects a random ground
                int groundIndex = 0;
                if (biomes[biomeIndex].groundSprites.Length > 1)
                    groundIndex = Random.Range(0, biomes[biomeIndex].groundSprites.Length - 1);

                // Sets correct ground tile sprite
                newGround.GetComponent<SpriteRenderer>().sprite = biomes[biomeIndex].groundSprites[groundIndex];

            }
        }
    }

    public void GenerateFood(bool eraseAll = false)
    {
        // Destroy all existing foods if needed
        if (eraseAll == true && existingFoods.Count > 0) {
            foreach (GameObject food in existingFoods) {
                // TODO: change when Food script get created
                Destroy(food);
            }

            existingFoods.Clear();
        }

        // Get a random node
        Tile tile = gridGen.gridData.GetRandomTile();

        // Loop until find a valid tile
        // TODO: Remove while loop and create a walkable nodes List
        int counter = 0;
        while (true)
        {
            if (!tile.Walkable())
            {
                // If not walkable, try again
                tile = gridGen.gridData.GetRandomTile();
                counter++;
            }
            else
            {
                // If walkable, break the loop
                break;
            }
            if (counter > 10000) {
                Debug.LogError("GenerateFood :: Tried to find a walkable tile for food placement 10.000 times");
                break;
            }
        }

        // Generates a random food Index
        int foodIndex = 0;
        if (foodPrefabs.Length > 1)
            foodIndex = Random.Range(0, foodPrefabs.Length - 1);

        // Creates the food GO
        GameObject foodGO = Instantiate(foodPrefabs[foodIndex], tile.worldPos, Quaternion.identity);
        Debug.Log("Food generated");

        // Adds it to the track list
        existingFoods.Add(foodGO);

        // Updates the tile
        tile.food = foodGO; // TODO: Change to the food DATA script
    }

}

[System.Serializable]
public class Biome
{
    public string name;
    public GameObject[] treePrefabs, borderPrefabs, otherPrefabs;
    public Sprite[] groundSprites;
}
