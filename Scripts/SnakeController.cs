using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnakeController : MonoBehaviour
{
    // Input mode
    public enum Mode
    {
        keyboard,
        swipe
    };
    public Mode mode;

    // The map we're in
    MapGenerator mapInfo;

    public Text foodCount;
    int pointCounter;
    int tailCount;
    GameObject lastTail;

    public GameObject greenTail01, greenTail02, greenTail03, greenTail04;
    Tail bodyInfo;

    // Start is called before the first frame update
    void Start()
    {
        pointCounter = 0;
        mapInfo = FindObjectOfType<MapGenerator>();
        bodyInfo = GetComponent<Tail>();
        lastTail = this.gameObject;

        mode = Mode.keyboard;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround(bodyInfo.currentNode);

        if (mode == Mode.keyboard) 
        {
            MoveWithKeyboard();
        }

        if (mode == Mode.swipe)
        {
            SwipeToMove();
        }
    }

    void MoveWithKeyboard()
    {

        if (Input.GetKeyDown(KeyCode.W) && bodyInfo.currentDirection != Tail.movementDirection.back)
        {
            bodyInfo.currentDirection = Tail.movementDirection.front;
        }
        else if (Input.GetKeyDown(KeyCode.D) && bodyInfo.currentDirection != Tail.movementDirection.left)
        {
            bodyInfo.currentDirection = Tail.movementDirection.right;
        }
        else if (Input.GetKeyDown(KeyCode.A) && bodyInfo.currentDirection != Tail.movementDirection.right) {
            bodyInfo.currentDirection = Tail.movementDirection.left;
        }
        else if (Input.GetKeyDown(KeyCode.S) && bodyInfo.currentDirection != Tail.movementDirection.front)
        {
            bodyInfo.currentDirection = Tail.movementDirection.back;
        }
    }

    void CheckGround(Tile node)
    {
        // Check if is stepping in obstacle
        if (!node.Walkable())
        {
            // Die if you do
            Die(); 
        }

        // Check if stepping in a tile that has a food object
        else if (node.HasFood())
        {
            /*
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
            Ray2D ray = new Ray2D(new Vector2(pos2D.x, pos2D.y), Vector2.down);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero, 150);
            if (hit)
            {
                if (hit.collider.gameObject.tag == "Eatable")
                {
                    //print("Eatable found is a" + hit.collider.gameObject.name);
                    Eat(hit);
                }
            }*/

            Eat(bodyInfo.currentNode);
        }
    }

    void Eat(Tile currentNode)
    {

        GameObject foodAte = currentNode.food;
        foodAte.tag = "Untagged";
        Destroy(foodAte);
        currentNode.food = null;

        pointCounter += 1;
        bodyInfo.speed += .1f * pointCounter/10;
        foodCount.text = pointCounter.ToString();
        AddTail();
        mapInfo.GenerateFood();
    }

    void Die()
    {
        // Sets all to 0
        pointCounter = 0;
        foodCount.text = pointCounter.ToString();
        bodyInfo.speed = Tail.initialSpeed;

        // Creates a New Map
        mapInfo.SetupMap();

        TilesGrid grid = mapInfo.gridGen.gridData;
        // Move snake to center
        if (grid != null) {
            bodyInfo.currentNode = grid.centerNode;
            bodyInfo.nextNode = null;
            bodyInfo.lastNode = null;

            transform.position = grid.NodeWorldPos(grid.centerNode);
        } else {
            Debug.LogError("Tail::Start -- GRID NOT CREATED YET");
        }
    }

    
        Vector2 firstPressPos;
        Vector2 secondPressPos;
        Vector2 currentSwipe;

    public void SwipeToMove()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                //save began touch 2d point
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }
            if (t.phase == TouchPhase.Ended)
            {
                //save ended touch 2d point
                secondPressPos = new Vector2(t.position.x, t.position.y);

                //create vector from the two points
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                //normalize the 2d vector
                currentSwipe.Normalize();

                //swipe upwards
            if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f && bodyInfo.currentDirection != Tail.movementDirection.back)
                {
                    bodyInfo.currentDirection = Tail.movementDirection.front;
                }
                //swipe down
                if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f && bodyInfo.currentDirection != Tail.movementDirection.front)
                {
                    bodyInfo.currentDirection = Tail.movementDirection.back;
                }
                //swipe left
                if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f && bodyInfo.currentDirection != Tail.movementDirection.right)
                {
                    bodyInfo.currentDirection = Tail.movementDirection.left;
                }
                //swipe right
                if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f && bodyInfo.currentDirection != Tail.movementDirection.left)
                {
                    bodyInfo.currentDirection = Tail.movementDirection.right;
                }
            }
        }
    }
    

    void AddTail()
    {
        GameObject tailToBeAdded = greenTail01;
        tailCount += 1;
        Tail lastTailScript = lastTail.GetComponent<Tail>();
        lastTail = Instantiate(tailToBeAdded, lastTailScript.currentNode.worldPos, lastTailScript.RotationFromDirection(lastTailScript.lastDirection));
        lastTail.GetComponent<Tail>().parentTailScript = lastTailScript;
    }
}

    

