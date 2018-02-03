using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Basic enemy controls, defining how the enemy interacts with obstacles.
/// This example will just move randomly in cardinal directions.
/// </summary>
public class EnemyBaseEntity : MonoBehaviour {

    public float moveTime;
    public float turnSpeed;
    int xGridPos;
    int yGridPos;
    public float gridSize;
    Vector3 currentLocation;
    Vector3 nextLocation;
    float movePercentage;

    void Awake()
    {
        currentLocation = transform.position;
        nextLocation = currentLocation;
        movePercentage = 0;
        setGridPositionFromRealPos();
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		if (nextLocation == null || nextLocation == currentLocation)
        {
            FindNextLocation();
        }
        //not using an else here, as this will run after a nextlocation has been defined.
        if (nextLocation != null)
        {
            //Rotate the player to where the intended movement is facing.
            /*
            Vector3 facing = nextLocation.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(facing);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * turnSpeed);
            */

            //MOVE IT.
            movePercentage += (1 / moveTime) * Time.deltaTime;
            transform.position = Vector3.Lerp(currentLocation, nextLocation, movePercentage);
            if (movePercentage >= 1)
            {
                //reset all the stuff
                movePercentage = 0;
                currentLocation = nextLocation;
            }
        }
	}

    //Navigation logic for the enemy is put here. Optimally you'd sub out the contents of this function to make for some strategy.
    //For now this just picks one of four random cardinal directions to move in.
    void FindNextLocation()
    {
        List<Vector3> locations = new List<Vector3>();
        for (int a = 0; a < 360; a += 90)
        {
            RaycastHit hit;
            Vector3 positionFacing = new Vector3(Mathf.Cos(Mathf.Deg2Rad * a), 0, Mathf.Sin(Mathf.Deg2Rad * a));
            if (Physics.Raycast(transform.position, positionFacing, out hit, gridSize))
            {
                //if (hit.collider.CompareTag("Player"))
                {
                    //print(hit.transform.name + " at " + positionFacing);
                    continue;
                }
            }
            //print(positionFacing);
            locations.Add(transform.position + positionFacing);
        }
        print(locations.Count + " possible spots");
        if (locations.Count > 0)
        {
            nextLocation = locations[Random.Range(0, locations.Count - 1)];
        }
    }

    //Translates the x and z position of the player into x and y position on the game grid. Used to determine where a bomb would be laid.
    void setGridPositionFromRealPos()
    {
        xGridPos = Mathf.RoundToInt(transform.position.x);
        yGridPos = Mathf.RoundToInt(transform.position.z);
        //print(xGridPos + ", " + yGridPos);
    }
}
