using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This enemy type is similar to Straight AI, but will move through bombs.
/// Any bombs it moves over it will eat and destroy.
/// </summary>
public class EnemyBombEater : EnemyBaseEntity
{

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
            if (!hit.collider.CompareTag("Player") || !hit.collider.CompareTag("bomb"))
            {
                List<Vector3> locations = new List<Vector3>();
                for (int a = 0; a < 360; a += 90)
                {
                    Vector3 positionFacing = new Vector3(Mathf.Cos(Mathf.Deg2Rad * a), 0, Mathf.Sin(Mathf.Deg2Rad * a));
                    if (Physics.Raycast(transform.position, positionFacing, out hit, gridSize))
                    {
                        if (!hit.collider.CompareTag("Player") || !hit.collider.CompareTag("bomb"))
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

    //Damages the player when they collide, and also destroys bombs.
    void OnTriggerEnter(Collider other)
    {
        if (dead) return;

        if (other.CompareTag("Player"))
        {
            HealthSP thingHit = other.GetComponent<HealthSP>();
            if (thingHit != null)
                thingHit.TakeDamage(1);
        }

        if (other.CompareTag("bomb"))
        {
            //Maybe add an eating sound here?
            Destroy(other.gameObject);
        }
    }

}
