using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : IHeapItem<Tile>
{
    // Grid variables
    public int gridX;
    public int gridY;
    public int movementPenalty;

    public int gCost;
    public int hCost;

    // Heap variables
    public Tile parent;
    int heapIndex;

    // Food on this tile
    public GameObject food;

    // World position
    public Vector3 worldPos;

    // Walkability
    public bool hasObstacle;
    public bool isBorder;

    public Tile(int _gridX, int _gridY, int _penalty, bool _isBorder, Vector3 _pos,GameObject _food = null)
    {
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _penalty;
        isBorder = _isBorder;
        worldPos = _pos;
        food = _food;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Tile nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }

    public bool HasFood() {
        return food != null;
    }

    public bool Walkable() {
        if (hasObstacle || isBorder) {
            // Unwalkable
            return false;
        } else {
            // Walkable
            return true;
        }
    }
}

