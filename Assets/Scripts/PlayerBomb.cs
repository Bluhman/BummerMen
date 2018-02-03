using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(Rigidbody))]

public class PlayerBomb : NetworkBehaviour {

    public float speed;
    public float turnSpeed;
    int xGridPos;
    int yGridPos;
    public float gridSize;

    Rigidbody rb;

    //bomb spawner script
    public Rigidbody bomb1;
    public Rigidbody bomb2;
    public float bombSpeed1 = 0;
    public float bombSpeed2 = 5;
    public float spawnTime = 2;
    public bool setBomb;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        setGridPositionFromRealPos();
        setBomb = true;
    }

    public override void OnStartLocalPlayer()
    {
        print(NetworkManager.singleton.numPlayers + " right now.");
        //GetComponent<Renderer>().material.color = Color.blue;
        Color playerColor = Color.white;
        //We'll need to fix this, right now the user's player is just white.
        switch (NetworkManager.singleton.numPlayers)
        {
            case 1:
                break;

            case 2:
                playerColor = Color.black;
                break;
            case 3:
                playerColor = Color.red;
                break;
            case 4:
                playerColor = Color.blue;
                break;
            case 5:
                playerColor = Color.yellow;
                break;
            case 6:
                playerColor = Color.magenta;
                break;
            case 7:
                playerColor = Color.cyan;
                break;
            case 8:
                playerColor = Color.green;
                break;
        }

        GetComponent<Renderer>().material.color = playerColor;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (!isLocalPlayer)
        {
            return;
        }

        moveWithInput();
        setGridPositionFromRealPos();
        BombCooldown();
	}

    //Takes input to move the player.
    void moveWithInput()
    {


        Vector3 plannedMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        
        if (plannedMovement.magnitude != 0)
        {
            //Moving up against barriers should cause the player to slide along their edge instead of stopping.
            //This behavior should only apply if the player is moving in a single axis, however.
            if (plannedMovement.x == 0 || plannedMovement.z == 0)
            {
                //Perform a raycast in the intended direction of movement.
                RaycastHit hit;
                if (Physics.Raycast(transform.position, plannedMovement, out hit, gridSize))
                {
                    if (hit.collider.CompareTag("Barrier"))
                    {
                        //print("Hit a barrier.");

                        //We want to measure the difference between the collision's position and the player's position, whether the x or y is more severe.
                        //This will help us determine if we're hitting a block from the top/bottom or side.
                        Vector3 hitComparison = hit.point - transform.position;

                        //If the x component was more severe, we want to slide up or down.
                        if (Mathf.Abs(hitComparison.x) > Mathf.Abs(hitComparison.z))
                        {
                            //Sliding is determined by whether the player is to the upper or lower half of the block.
                            if (transform.position.z > hit.transform.position.z)
                            {
                                plannedMovement = new Vector3(0, 0, 1);
                            }
                            else
                            {
                                plannedMovement = new Vector3(0, 0, -1);
                            }
                        }
                        else
                        {
                            if (transform.position.x > hit.transform.position.x)
                            {
                                plannedMovement = new Vector3(1, 0, 0);
                            }
                            else
                            {
                                plannedMovement = new Vector3(-1, 0, 0);
                            }
                        }
                    }
                }
            }

            //Rotate the player to where the intended movement is facing.
            Vector3 facing = plannedMovement.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(facing);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * turnSpeed);
        }

        //plannedMovement.Normalize();

        transform.Translate(plannedMovement * speed * Time.fixedDeltaTime, Space.World);
    }

    //Translates the x and z position of the player into x and y position on the game grid. Used to determine where a bomb would be laid.
    void setGridPositionFromRealPos()
    {
        xGridPos = Mathf.RoundToInt(transform.position.x);
        yGridPos = Mathf.RoundToInt(transform.position.z);
        //print(xGridPos + ", " + yGridPos);
    }
    void BombCooldown()
    {

        if (!setBomb)
        {
            spawnTime -= Time.deltaTime;
            if (spawnTime < 0)
            {
                setBomb = true;

            }
        }

        if (setBomb)
        {
            if (Input.GetKeyDown("space"))
            //if (Input.GetKeyDown(Keycode.Mouse1))
            {
                StationaryBomb();
                setBomb = false;
                spawnTime = 3;
            }

            /*(if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                RollingBomb();
                setBomb = false;
                spawnTime = 3;
            }*/
        }
    }
    void StationaryBomb()
    {
        Rigidbody bomb1Clone = (Rigidbody)Instantiate(bomb1, transform.position - (transform.forward * 1), transform.rotation);
        bomb1Clone.velocity = transform.forward * bombSpeed1;
    }
    void RollingBomb()
    {
        Rigidbody bomb2Clone = (Rigidbody)Instantiate(bomb2, transform.position + (transform.forward * 1), transform.rotation);
        bomb2Clone.velocity = transform.forward * bombSpeed2;
    }

}
