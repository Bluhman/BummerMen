using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]

public class PlayerSP : MonoBehaviour {

    public float baseSpeed;
    float speed;
    public float turnSpeed;
    public float maxSpeed;
    public float minSpeed;
    public float speedPowerUpStep;
    int xGridPos;
    int yGridPos;
    public GameObject bombPrefab;
    public float gridSize;

    public int playerNumber;
    public string layBombKey = "Fire1_P1";
    public string horizAxis = "Horizontal_P1";
    public string vertiAxis = "Vertical_P1";

    AudioSource AUDIO;
    public AudioClip pickUp;

    public int baseBombs = 3;
    int bombs;
    public int currentBombs;
    public int basePower = 3;
    int power;

    public int maxBombs = 6;
    public int maxPower = 12;

    public int lives = 3;
    [HideInInspector]
    public int currentLives;

    CharacterController character;
    private CollisionFlags charCollisionFlags;

    [HideInInspector]
    public bool controllable = true;

    void Awake()
    {
        character = GetComponent<CharacterController>();

        //set player color:
        Color playerColor = Color.white;
        switch (playerNumber)
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

        resetStats();
        currentLives = lives;
    }

	// Use this for initialization
	void Start () {
        AUDIO = GetComponent<AudioSource>();
        setGridPositionFromRealPos();
    }

    private void Update()
    {
        setGridPositionFromRealPos();

        if (lives <= 0) return;

        if (Time.timeScale > 0)
        moveWithInput();

        if (Input.GetButtonDown(layBombKey))
        {
            LayBomb();
        }
    }

    //Lays a bomb aligned to the game's grid:
    void LayBomb()
    {
        if (currentBombs >= bombs) return;

        print("Pressed space.");

        //Prevent player from laying more bombs on top of bombs.
        Collider[] thingsInPlace = Physics.OverlapSphere(new Vector3(xGridPos, 0.5f, yGridPos), 0.3f);
        foreach (Collider thing in thingsInPlace)
        {
            if (thing.gameObject.GetComponent<BombSP>() != null) return;
        }

        GameObject bomb = Instantiate(bombPrefab, new Vector3(xGridPos, 0.5f, yGridPos), Quaternion.identity);
        
        currentBombs++;
        print("Bomb laid");

        CreateBomb(bomb);
    }
    
    void CreateBomb(GameObject bomb)
    {
        BombSP bombScript = bomb.GetComponent<BombSP>();
        bombScript.owner = this;
        bombScript.power = power;
    }

    //Takes input to move the player.
    void moveWithInput()
    {
        Vector3 plannedMovement = new Vector3(Input.GetAxis(horizAxis), 0, Input.GetAxis(vertiAxis));
        
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

        //transform.Translate(plannedMovement * speed * Time.fixedDeltaTime, Space.World);
        charCollisionFlags = character.Move(plannedMovement * speed * Time.fixedDeltaTime);
    }

    //Translates the x and z position of the player into x and y position on the game grid. Used to determine where a bomb would be laid.
    void setGridPositionFromRealPos()
    {
        xGridPos = Mathf.RoundToInt(transform.position.x);
        yGridPos = Mathf.RoundToInt(transform.position.z);
        //print(xGridPos + ", " + yGridPos);
    }

    //Method called when the player picks up a PowerUp. This matches up the number of the powerup to an effect.
     public void applyPowerup(int type)
    {
        AUDIO.clip = pickUp;
        AUDIO.Play();

        switch (type)
        {
            case 0:
                //Speed up
                speed += speedPowerUpStep;
                speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
                break;
            case 1:
                //Extra bomb.
                bombs++;
                bombs = Mathf.Clamp(bombs, 1, maxBombs);
                break;
            case 2:
                //Extra power.
                power++;
                power = Mathf.Clamp(power, 1, maxPower);
                break;
            case 3:
                //Slow down.
                speed -= speedPowerUpStep;
                speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
                break;
            default:
                //Do nothing because it's not a valid powerup.
                return;
        }
    }

    //Resets the stats to their original values.
    public void resetStats()
    {
        speed = baseSpeed;
        bombs = baseBombs;
        power = basePower;
    }

}
