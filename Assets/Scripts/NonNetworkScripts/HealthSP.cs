using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSP : MonoBehaviour
{

    public const int maxHealth = 5;
    public float invulnTime = 3.0f;
    public bool startInvuln = true;
    public bool isAPlayer = true;
    public bool isAnEnemy = false;
    public bool zappable = true; //USED TO MAKE THINGS INVULNERABLE TO THE LIGHTNING!
    public int currentHealth = 1;
    public PlayerHUDControllerSP hudForPlayer;
    public GameObject invulnEffect;

    public float currentInvulnTime;
    public AudioClip dieSound;
    AudioSource AUDIO;

    bool looksInvuln;
    bool alreadyDead;

    public Vector3[] spawnPoints;

    //public GameObject[] dropOnDeath;
    public GenericLootDropTableGameObject dropOnDeath;

    private void Start()
    {
        if (startInvuln)
            currentInvulnTime = invulnTime;

        looksInvuln = (currentInvulnTime > 0);
        if (looksInvuln && invulnEffect != null)
        {
            invulnEffect.SetActive(true);
        }

        AUDIO = GetComponent<AudioSource>();

        alreadyDead = false;

    }

    public void HealUp(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth+1, 0, maxHealth);
    }

    public void TakeDamage(int amount, bool canGenerateDrop = true)
    {
        print("I will now take damage." + currentInvulnTime);

        if (currentInvulnTime > 0) return;

        currentHealth -= amount;
        

        if (currentHealth <= 0 && !alreadyDead)
        {
            //Play death sound if it has one.
            if (AUDIO != null || dieSound != null)
            {
                AUDIO.pitch = 1f;
                AUDIO.timeSamples = 0;
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
            else if (canGenerateDrop)
            {
                ItemSpawnDestroy();
            }


            if (isAnEnemy)
            {
                hudForPlayer.AddOrRemoveEnemy(-1);
            }
        }

        else
        {
            looksInvuln = true;
            if (invulnEffect != null)
                invulnEffect.SetActive(true);
            currentInvulnTime = invulnTime;
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
                if (invulnEffect != null)
                    invulnEffect.SetActive(false);
                looksInvuln = false;
            }
        }
    }

    public void Respawn()
    {
        print("RESPAWN");

        looksInvuln = true;
        if (invulnEffect != null)
            invulnEffect.SetActive(true);
        currentInvulnTime = invulnTime;

        // default our spawn point to the 0 spot.
        Vector3 spawnPoint = new Vector3(0, 0.5f, 0);

        // If there is a spawn point array and the array is not empty, pick a spawn point at random
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        // Set the player’s position to the chosen spawn point
        transform.position = spawnPoint;
        alreadyDead = false;
    }
}
