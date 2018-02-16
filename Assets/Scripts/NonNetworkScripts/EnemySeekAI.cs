using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This enemy AI will move in straight lines down hallways similar to EnemyStraight.
/// On each reselection of a direction to move, it will have a chance of trying to move towards the player.
/// </summary>
public class EnemySeekAI : EnemyBaseEntity
{
    Vector3 lastMoveAngle;
    GameObject player;

	// Use this for initialization
	void Start () {
        //get reference to player
        player = GameObject.FindGameObjectWithTag("Player");

	}

    // Update is called once per frame
    public override void FindNextLocation()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, lastMoveAngle, out hit, gridSize))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                //move a direction towards the player.
                Vector3 positionRelativeToPlayer = transform.position - player.transform.position;


                //pick a random direction to move.
                List<Vector3> locations = new List<Vector3>();
                for (int a = 0; a < 360; a += 90)
                {
                    Vector3 positionFacing = new Vector3(Mathf.Cos(Mathf.Deg2Rad * a), 0, Mathf.Sin(Mathf.Deg2Rad * a));
                    if (Physics.Raycast(transform.position, positionFacing, out hit, gridSize))
                    {
                        if (!hit.collider.CompareTag("Player"))
                        {
                            //print(hit.transform.name + " at " + positionFacing);
                            continue;
                        }
                    }
                    //print(a + " is an option");
                    locations.Add(positionFacing);
                }
                //print(locations.Count + " possible spots");
                if (locations.Count > 0)
                {
                    int randomIndex = Random.Range(0, locations.Count);
                    nextLocation = transform.position + locations[randomIndex];
                    lastMoveAngle = locations[randomIndex];
                }
            }
        }
        else
        {
            nextLocation = transform.position + lastMoveAngle;
        }


    }
}
