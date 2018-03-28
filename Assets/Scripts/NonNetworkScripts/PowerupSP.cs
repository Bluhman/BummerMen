using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSP : MonoBehaviour
{

    public float invulnTime = 2;
    private float invulnTimer;
    public bool respawns = false;
    public float respawnTime = 0.0f;
    float respawnTimer;
    public int powerupType;
    public float rotateSpeed;
    
    Collider col;
    Renderer ren;

	void Start () {
        invulnTimer = invulnTime;
        respawnTimer = 0;

        col = GetComponent<Collider>();
        ren = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (invulnTimer >= 0)
        {
            invulnTimer -= Time.deltaTime;
        }

        if (respawnTimer > 0)
        {
            respawnTimer -= Time.deltaTime;
            //print(respawnTimer);
            if (respawnTimer <= 0)
            {
                //gameObject.SetActive(true);
                col.enabled = true;
                ren.enabled = true;
            }
        }

        

        //YOU SPIN ME RIGHT ROUND BABY RIGHT ROUND!
        transform.Rotate(Vector3.down, rotateSpeed * Time.deltaTime);
	}

    public void OnTriggerEnter(Collider other)
    {
        //print(other.tag);

        if (other.CompareTag("Player"))
        {
            PlayerSP playerObject = other.gameObject.GetComponent<PlayerSP>();
            if (playerObject != null)
            {
                playerObject.applyPowerup(powerupType);
                DestroyOrRespawn();
            }
        }

        if (other.CompareTag("Explosion") && invulnTimer <= 0)
        {
            //Maybe create some custom explosion effect?
            DestroyOrRespawn();
        }
    }

    void DestroyOrRespawn()
    {
        if (respawns)
        {
            //gameObject.SetActive(false);
            col.enabled = false;
            ren.enabled = false;
            respawnTimer = respawnTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
