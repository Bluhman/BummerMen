using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This version of an enemy will move straight down hallways and only change directions when it cannot move in a straight line anymore.
/// </summary>
public class EnemyStraightAI : EnemyBaseEntity {

    Vector3 lastMoveAngle;

    private void Start()
    {
        //Create a random starting angle.
        int rand = Random.Range(0, 4);
        rand *= 90;
        lastMoveAngle = new Vector3(Mathf.Cos(Mathf.Deg2Rad * rand), 0, Mathf.Sin(Mathf.Deg2Rad * rand));
    }

    public override void FindNextLocation()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, lastMoveAngle, out hit, gridSize))
        {
            if (!hit.collider.CompareTag("Player"))
            {
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
