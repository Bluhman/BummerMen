using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This enemy moves mostly randomly, but has a chance to head in a direction towards the player.
/// </summary>
public class EnemySeekAI : EnemyBaseEntity
{
    GameObject player;
    public float followChance;
    public float aggroRange;
    //These two settings override MoveTime for when the enemy is moving normally, and when stalking the player.
    public float baseMoveTime;
    public float aggroMoveTime;
    public bool requireLineOfSight;

    // Use this for initialization
    void Start()
    {
        //get reference to player
        player = GameObject.FindGameObjectWithTag("Player");

    }
    
    public override void FindNextLocation()
    {
        moveTime = baseMoveTime;
        RaycastHit hit;

        //move a direction towards the player.
        Vector3 positionRelativeToPlayer = player.transform.position - transform.position;
        bool follow = (positionRelativeToPlayer.magnitude <= aggroRange);

        if (follow && requireLineOfSight)
        {
            if (Physics.Raycast(transform.position, positionRelativeToPlayer, out hit))
            {
                if (!hit.collider.CompareTag("Player")) follow = false;
            }
        }

        if (follow) moveTime = aggroMoveTime;

        //pick a random direction to move.
        List<Vector3> locations = new List<Vector3>();
        for (int a = 0; a < 360; a += 45)
        {
            
            Vector3 positionFacing = new Vector3(Mathf.Cos(Mathf.Deg2Rad * a), 0, Mathf.Sin(Mathf.Deg2Rad * a));

            float customGridSize = gridSize;
            if (a % 90 != 0) customGridSize *= Mathf.Sqrt(2);

            if (Physics.Raycast(transform.position, positionFacing, out hit, customGridSize))
            {
                if (!hit.collider.CompareTag("Player") || !hit.collider.CompareTag("Explosion"))
                {
                    //print(hit.transform.name + " at " + positionFacing);
                    continue;
                }

                
            }

            if (a%90 != 0)
            {
                positionFacing *= Mathf.Sqrt(2);
            }

            locations.Add(positionFacing);

            //If the angle is legal and close to an angle that would send us towards the player, we go that direction.
            if (Vector3.Angle(positionFacing, positionRelativeToPlayer) <= 22.5f && follow && Random.Range(0,1) <= followChance)
            {
                nextLocation = transform.position + positionFacing;
                return;
            }
        }
        //print(locations.Count + " possible spots");
        if (locations.Count > 0)
        {
            int randomIndex = Random.Range(0, locations.Count);
            nextLocation = transform.position + locations[randomIndex];
        }


    }
}
