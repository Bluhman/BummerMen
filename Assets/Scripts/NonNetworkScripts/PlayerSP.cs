﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
[RequireComponent(typeof(CharacterController))]

public class PlayerSP : MonoBehaviour {

    static bool testingVERSUS = false; //make sure to set this to false once the game's ready to actually be played.

    public Texture textureSwap;
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
    public bool ghost;

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
    Animator animator;
    public bool usePreviousStats = false;

    void Awake()
    {
        character = GetComponent<CharacterController>();
        HSP = GetComponent<HealthSP>();
        animator = GetComponentInChildren<Animator>();
        bool actuallyChangeColor = true;

        //set player color:
        playerColor = Color.white;

        if (textureSwap != null)
            GetComponentInChildren<Renderer>().material.SetTexture("_MainTex",textureSwap);

        switch (playerNumber)
        {
            case 1:
                actuallyChangeColor = false;
                break;

            case 2:
                actuallyChangeColor = false;
                playerColor = Color.black;
                break;
            case 3:
                actuallyChangeColor = false;
                playerColor = Color.red;
                break;
            case 4:
                actuallyChangeColor = false;
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

        if (actuallyChangeColor)
            GetComponentInChildren<Renderer>().material.color = playerColor;

        resetStats();
        if (usePreviousStats)
            receiveStats();
        currentLives = lives;

        //special check: depending on the gamecontroller's player number, we might not want to spawn players.
        //The one exception is player 1 themselves if we're playing a singleplayer game.
        print(GameController.instance.versus);
        if (GameController.instance.versus && !GameController.instance.players[playerNumber-1] && !testingVERSUS)
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
            //transform.position += new Vector3 (0,70*Time.deltaTime, 0);
            //transform.Rotate(transform.up, 1080 * Time.deltaTime);
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && animator.GetCurrentAnimatorStateInfo(0).IsName("DEAD"))
            {
                invokedPostDeath();
            }

            return;
        }

        if (playerController == null) return;

        if (playerController.Action1.WasPressed)
        {
            LayBomb(false);
        }

        if (playerController.Action2.WasPressed)
        {
            LayBomb(true);
        }
    }

    private void FixedUpdate()
    {
        if (playerController == null) return;
        if (dead) return;

        if (Time.timeScale > 0 && !HSP.hudForPlayer.gameOver)
            moveWithInput();
    }

    //Lays a bomb aligned to the game's grid:
    void LayBomb(bool special)
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

        CreateBomb(bomb, special);
    }
    
    void CreateBomb(GameObject bomb, bool special)
    {
        BombSP bombScript = bomb.GetComponent<BombSP>();
        bombScript.owner = this;
        bombScript.playerController = playerController;
        bombScript.power = power;
        if (special)
        {
            bombScript.powerBomb = powerBomb;
            bombScript.triggerBomb = remoteBomb;
        }
        bombScript.SwapModel();

        if (ghost)
        {
            Collider bombCollider = bomb.GetComponent<Collider>();
            bombCollider.isTrigger = false;
            Physics.IgnoreCollision(bombCollider, character);
        }
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
        animator.SetFloat("MovementSpeed", plannedMovement.magnitude);
        
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
            case 8:
                //ghost allows you to go through all bombs.
                ghost = true;
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
        animator.SetTrigger("Died");
        //Invoke("invokedPostDeath", 2);
    }

    public void invokedPostDeath()
    {
        animator.ResetTrigger("Died");
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
        ghost = false;
    }

    //These two functions get and set stats from the GameController. They are used in campaign mode to transfer stats from level to level.
    public void receiveStats()
    {
        GameController.PlayerStats myStats = GameController.instance.campaignPlayer;
        if (myStats.health <= 0) return;

        currentLives = myStats.lives;
        HSP.currentHealth = myStats.health;
        speed = myStats.speed;
        bombs = myStats.bombs;
        power = myStats.power;
        powerBomb = myStats.nuke;
        remoteBomb = myStats.remote;
        ghost = myStats.ghost;
    }

    //The only situation in which to do this is at the end of a level.
    public void sendStats()
    {
        GameController.PlayerStats myStats = new GameController.PlayerStats();

        myStats.lives = currentLives;
        myStats.health = HSP.currentHealth;
        if (myStats.health <= 0) myStats.health = 1;
        myStats.speed = speed;
        myStats.bombs = bombs;
        myStats.power = power;
        myStats.nuke = powerBomb;
        myStats.remote = remoteBomb;
        myStats.ghost = ghost;

        GameController.instance.campaignPlayer = myStats;
    }
}
