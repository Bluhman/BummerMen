using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthSP : MonoBehaviour
{

    public const int maxHealth = 5;
    public float invulnTime = 3.0f;
    public bool startInvuln = true;
    public bool isAPlayer = true;
    public int currentHealth = 1;
    public PlayerHUDControllerSP hudForPlayer;

    public float currentInvulnTime;
    public AudioClip dieSound;
    AudioSource AUDIO;

    bool looksInvuln;
    bool alreadyDead;

    private NetworkStartPosition[] spawnPoints;

    //public GameObject[] dropOnDeath;
    public GenericLootDropTableGameObject dropOnDeath;

    Color playerColor;
    Color invulnColor;

    private void Start()
    {
        if (startInvuln)
            currentInvulnTime = invulnTime;

        looksInvuln = (currentInvulnTime > 0);

        playerColor = GetComponent<Renderer>().material.color;
        invulnColor = playerColor;
        invulnColor.a = 0.5f;

        GetComponent<Renderer>().material.color = invulnColor;

        AUDIO = GetComponent<AudioSource>();

        spawnPoints = FindObjectsOfType<NetworkStartPosition>();

        alreadyDead = false;

    }

    public void TakeDamage(int amount)
    {
        print("I will now take damage." + currentInvulnTime);

        if (currentInvulnTime > 0) return;

        currentHealth -= amount;
        looksInvuln = true;
        GetComponent<Renderer>().material.color = invulnColor;
        currentInvulnTime = invulnTime;

        if (currentHealth <= 0 && !alreadyDead)
        {
            //Play death sound if it has one.
            if (AUDIO != null || dieSound != null)
            {
                AUDIO.clip = dieSound;
                AUDIO.Play();
            }
            alreadyDead = true;
            //This bool is set to make sure that the death scripting isn't run more than once.
            //This is important for objects that spawn things when destroyed, such as powerups.
            if (isAPlayer)
            {
                currentHealth = 1;
                //Reset all the player's stats.
                PlayerSP playerScript = GetComponent<PlayerSP>();
                if (playerScript != null)
                {
                    playerScript.currentLives--;
                    if (hudForPlayer != null)
                        hudForPlayer.UpdateLives();
                    print("reduced to " + playerScript.currentLives + " lives.");
                    playerScript.DIE();
                    /*
                    if (playerScript.currentLives > 0)
                        Respawn();
                    else
                    {
                        gameObject.SetActive(false);
                    }
                    */
                }


            }
            else
            {
                ItemSpawnDestroy();
            }
        }


    }

    //This function is called when a non-player dies, and is run on the server.
    //If the item has assigned drops, it will randomly generate one to show when destroyed.
    //Regardless, it will destroy the given object.
    void ItemSpawnDestroy()
    {
        /*
        print("rpcitemspawn "+dropOnDeath.lootDropItems.Count);
        for (int i = 0; i < dropOnDeath.lootDropItems.Count; i++) 
        {
            if (dropOnDeath.lootDropItems[i].item != null)
            print(dropOnDeath.lootDropItems[i].item.name);
        }
        */
        Debug.Log(dropOnDeath);

        if (dropOnDeath.lootDropItems.Count > 0)
        {
            //GameObject powerUp = Instantiate(dropOnDeath[Random.Range(0, dropOnDeath.Length)], transform.position, Quaternion.identity);
            GenericLootDropItemGameObject powerUp = dropOnDeath.PickLootDropItem();
            if (powerUp.item != null)
            {
                GameObject newPowerUp = Instantiate(powerUp.item, transform.position, Quaternion.identity);
            }
        }
        EnemyBaseEntity enemyScript = GetComponent<EnemyBaseEntity>();
        if (enemyScript != null)
        {
            enemyScript.DeathAnimation();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnValidate()
    {
        // Validate table and notify the programmer / designer if something went wrong.
        dropOnDeath.ValidateTable();
    }


    void Update()
    {
        //Reduce invulnerability time:
        if (looksInvuln)
        {
            currentInvulnTime -= Time.deltaTime;

            if (currentInvulnTime <= 0)
            {
                print("I am no longer invuln.");
                GetComponent<Renderer>().material.color = playerColor;
                looksInvuln = false;
            }
        }
    }

    public void Respawn()
    {
        print("RESPAWN");

        // default our spawn point to the 0 spot.
        Vector3 spawnPoint = new Vector3(0, 0.5f, 0);

        // If there is a spawn point array and the array is not empty, pick a spawn point at random
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        }

        // Set the player’s position to the chosen spawn point
        transform.position = spawnPoint;
        alreadyDead = false;
    }
}
