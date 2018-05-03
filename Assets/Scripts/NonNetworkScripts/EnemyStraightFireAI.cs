using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Same as enemystraightAI but at each leg of the trip, the enemy will pause to fire a projectile towards a player.
/// </summary>

public class EnemyStraightFireAI : EnemyBaseEntity
{
    GameObject player;
    public GameObject projectile;
    Vector3 lastMoveAngle;
    //These override the movement period depending on action;
    public float shooterFiringTime;
    float firingTimer;
    bool firing = false;

    private void Start()
    {
        //Create a random starting angle.
        int rand = Random.Range(0, 4);
        rand *= 90;
        lastMoveAngle = new Vector3(Mathf.Cos(Mathf.Deg2Rad * rand), 0, Mathf.Sin(Mathf.Deg2Rad * rand));

        //get reference to player
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void FindNextLocation()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, lastMoveAngle, out hit, gridSize) && !firing)
        {
            //print("FIRE MAN!");
            //Fires a projectile.
            firing = true;

            //create a projectile;
            GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
            newProjectile.transform.LookAt(player.transform);

            firingTimer = shooterFiringTime;

            nextLocation = currentLocation;
        }

        if (firing)
        {
            //print("Waiting");
            nextLocation = currentLocation;
            firingTimer -= Time.deltaTime;
        }
        if (firingTimer < 0)
        {
            //print("EXIT FIRING MODE.");
            firing = false;
        }

        if (!firing)
        {
            if (Physics.Raycast(transform.position, lastMoveAngle, out hit, gridSize))
            {

                List<Vector3> locations = new List<Vector3>();
                for (int a = 0; a < 360; a += 90)
                {
                    Vector3 positionFacing = new Vector3(Mathf.Cos(Mathf.Deg2Rad * a), 0, Mathf.Sin(Mathf.Deg2Rad * a));
                    if (Physics.Raycast(transform.position, positionFacing, out hit, gridSize))
                    {
                        if (!hit.collider.CompareTag("Player") || !hit.collider.CompareTag("Enemy"))
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

            else
            {
                nextLocation = transform.position + lastMoveAngle;
            }
        }

    }


}

