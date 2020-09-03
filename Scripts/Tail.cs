using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    // Static minimum speed
    public static float initialSpeed = 2f;

    public enum movementDirection
    {
        front,
        back,
        left,
        right
    };

    public movementDirection lastDirection,currentDirection,nextDirection;
    public Tile nextNode, currentNode, lastNode;
    public Tail parentTailScript;

    public bool isHead = false;
    public float speed;
    float counter = 0;

    // Grid in which we are in
    TilesGrid grid;

    void Start()
    {


        if (!isHead)
        {
            nextDirection = parentTailScript.currentDirection;
            nextNode = parentTailScript.currentNode;
        } else
        {
            if (grid != null) {
                currentNode = grid.centerNode;
                transform.position = grid.NodeWorldPos(grid.centerNode);
            } else {
                Debug.LogError("Tail::Start -- GRID NOT CREATED YET");
            }
        }
    }


    void Update()
    {
        if (isHead)
        {
            counter += Time.deltaTime * speed;
            // Rotate the sprite
            RotateBasedOnDirection(currentDirection);

            if (counter >= 1f)
            {
                // Go to Next Node
                GoToNextNodeBeingHead();
                counter -= 1f;
            }
        } else if (parentTailScript.lastNode == nextNode)
        {
            GoToNextNode();
        }
    }

     movementDirection FindCurrentDirection()
    {
        movementDirection currentDirection = movementDirection.front;
        if (transform.rotation.eulerAngles.z == 0f)
        {
            currentDirection = movementDirection.front;
        }
        else if(transform.rotation.eulerAngles.z == 90f)
        {
            currentDirection = movementDirection.left;
        } else if (transform.rotation.eulerAngles.z == 180f)
        {
            currentDirection = movementDirection.back;
        }
        else if (transform.rotation.eulerAngles.z == -90f)
        {
            currentDirection = movementDirection.right;
        }

        return currentDirection;
    }

    void GoToNextNode()
    {
        transform.position = grid.NodeWorldPos(nextNode);

        lastNode = currentNode;
        currentNode = nextNode;
        nextNode = parentTailScript.currentNode;


        RotateBasedOnDirection(nextDirection);

        lastDirection = currentDirection;
        currentDirection = nextDirection;
        nextDirection = parentTailScript.currentDirection;
    }

    void GoToNextNodeBeingHead()
    {
        // Last node TRACKER
        lastNode = currentNode;

        // Get Next Node
        currentNode = grid.GetNodeFromWorldPoint(transform.position + VectorFromDirection());
        

        // Do Movement
        transform.position = grid.NodeWorldPos(currentNode);

        // Last direction TRACKER
        lastDirection = currentDirection;
    }

    Vector3 VectorFromDirection()
    {
        Vector3 directionPointer = new Vector3(0,0,0);
        if (currentDirection == movementDirection.front)
        {
            directionPointer = new Vector3(0, 1, 0);
        } else if (currentDirection == movementDirection.right)
        {
            directionPointer = new Vector3(1, 0, 0);
        } else if (currentDirection == movementDirection.left)
        {
            directionPointer = new Vector3(-1, 0, 0);
        }
        else if (currentDirection == movementDirection.back)
        {
            directionPointer = new Vector3(0, -1, 0);
        }

        return directionPointer;
    }

    void RotateBasedOnDirection(movementDirection currentDirection)
    {
        if (currentDirection == movementDirection.front)
        {
            transform.rotation = Quaternion.Euler(Vector3.forward * 0);
        }
        else if (currentDirection == movementDirection.back)
        {
            transform.rotation = Quaternion.Euler(Vector3.forward * 180);
        }
        else if (currentDirection == movementDirection.right)
        {
            transform.rotation = Quaternion.Euler(Vector3.forward * -90);
        }
        else if (currentDirection == movementDirection.left)
        {
            transform.rotation = Quaternion.Euler(Vector3.forward * 90);
        }
    }

    public Quaternion RotationFromDirection(movementDirection direction)
    {
        Quaternion rotation = new Quaternion(0,0,0,0);

        if (currentDirection == movementDirection.front)
        {
            rotation = Quaternion.Euler(Vector3.forward * 0);
        }
        else if (currentDirection == movementDirection.back)
        {
            rotation = Quaternion.Euler(Vector3.forward * 180);
        }
        else if (currentDirection == movementDirection.right)
        {
            rotation = Quaternion.Euler(Vector3.forward * -90);
        }
        else if (currentDirection == movementDirection.left)
        {
            rotation = Quaternion.Euler(Vector3.forward * 90);
        }

        return rotation;
    }
}
