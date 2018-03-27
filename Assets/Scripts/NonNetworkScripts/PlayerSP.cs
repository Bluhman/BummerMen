using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
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
    public Vector2 gridPos
    {
        get { return new Vector2(xGridPos, yGridPos); }
    }

    public GameObject bombPrefab;
    public float gridSize;

    public int playerNumber;
    InputDevice playerController;
    /*
    public string layBombKey = "Fire1_P1";
    public string triggerKey = "Fire2_P1";
    public string horizAxis = "Horizontal_P1";
    public string vertiAxis = "Vertical_P1";
    */

    AudioSource AUDIO;
    public AudioClip pickUp;

    public int baseBombs = 3;
    int bombs;
    public int currentBombs;
    public int basePower = 3;
    int power;

    public bool powerBomb;
    public bool remoteBomb;

    public int maxBombs = 6;
    public int maxPower = 12;

    public int lives = 3;
    [HideInInspector]
    public int currentLives;

    CharacterController character;
    private CollisionFlags charCollisionFlags;

    [HideInInspector]
    public bool controllable = true;
    [HideInInspector]
    public bool dead = false;
    [HideInInspector]
    public Color playerColor;

    HealthSP HSP;

    void Awake()
    {
        character = GetComponent<CharacterController>();
        HSP = GetComponent<HealthSP>();

        //set player color:
        playerColor = Color.white;
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
        GetComponentInChildren<Renderer>().material.color = playerColor;

        resetStats();
        currentLives = lives;

        //special check: depending on the gamecontroller's player number, we might not want to spawn players.
        //The one exception is player 1 themselves.
        if (playerNumber > 1 && playerNumber > GameController.instance.players)
        {
            gameObject.SetActive(false);
        }
        else if (InputManager.Devices.Count >= playerNumber)
        {
            playerController = InputManager.Devices[playerNumber - 1];
        }
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

        if (dead)
        {
            //Uhhhhhhhh play some animation iu ddunno
            transform.position += new Vector3 (0,70*Time.deltaTime, 0);
            transform.Rotate(transform.up, 1080 * Time.deltaTime);
            return;
        }

        if (Time.timeScale > 0)
        moveWithInput();

        if (playerController.Action1.WasPressed)
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
        bombScript.playerController = playerController;
        bombScript.power = power;
        bombScript.powerBomb = powerBomb;
        bombScript.triggerBomb = remoteBomb;
        bombScript.SwapModel();
    }

    //Takes input to move the player.
    void moveWithInput()
    {
        //REALLY MAKE SURE THE Y POSITION REMAINS LOCKED
        Vector3 temp = transform.position;
        temp.y = 0.5f;
        transform.position = temp;

        float xMovementSum = Mathf.Clamp(playerController.LeftStickX + playerController.DPadX, -1, 1);
        float yMovementSum = Mathf.Clamp(playerController.LeftStickY + playerController.DPadY, -1, 1);

        Vector3 plannedMovement = new Vector3(xMovementSum, 0, yMovementSum);
        
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
        AUDIO.pitch = 1f;
        AUDIO.timeSamples = 0;
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
                AUDIO.pitch = -0.5f;
                AUDIO.timeSamples = AUDIO.clip.samples -1;
                AUDIO.Play();
                speed -= speedPowerUpStep;
                speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
                break;
            case 4:
                //POWER BOMB!
                powerBomb = true;
                remoteBomb = false;//If we want to make these exclusive. I think it's a good idea.
                break;
            case 5:
                //REMOTE BOMB!
                remoteBomb = true;
                powerBomb = false;
                break;
            case 6:
                //Curse! Resets your power.
                AUDIO.pitch = -1f;
                AUDIO.timeSamples = AUDIO.clip.samples -1;
                AUDIO.Play();
                resetStats();
                HSP.currentHealth = 1;
                break;
            case 7:
                //Don't worry my friends! I am your shield!
                HSP.HealUp(1);
                break;
            default:
                //Do nothing because it's not a valid powerup.
                return;
        }
    }

    public void DIE()
    {
        //Play the death animation
        dead = true;
        Invoke("invokedPostDeath", 1);
    }

    public void invokedPostDeath()
    {
        resetStats();
        if (currentLives > 0)
            HSP.Respawn();
        else
        {
            gameObject.SetActive(false);
        }
    }

    //Resets the stats to their original values.
    public void resetStats()
    {
        speed = baseSpeed;
        bombs = baseBombs;
        power = basePower;
        powerBomb = remoteBomb = false;
        dead = false;
    }

}
