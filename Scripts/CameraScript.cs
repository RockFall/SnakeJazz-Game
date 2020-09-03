using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject followedObject;
    TilesGrid grid;
    public float speedControl;
    public bool rotateCamera;

    // Start is called before the first frame update
    void Start()
    {
        grid = FindObjectOfType<GridGenerator>().gridData;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (rotateCamera)
        {
            Quaternion targetDirection = followedObject.transform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetDirection, 180f * Time.deltaTime);
        }

        Vector3 targetPos = new Vector3(followedObject.transform.position.x, followedObject.transform.position.y, -10f);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, (followedObject.GetComponent<Tail>().speed/speedControl) * Time.deltaTime);
    }
}
