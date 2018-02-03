using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSP : MonoBehaviour
{

    public float invulnTime = 2;
    private float invulnTimer;
    public int powerupType;
    public float rotateSpeed;
    
	void Start () {
        invulnTimer = invulnTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (invulnTimer >= 0)
        {
            invulnTimer -= Time.deltaTime;
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
                Destroy(gameObject);
            }
        }

        if (other.CompareTag("Explosion") && invulnTimer <= 0)
        {
            //Maybe create some custom explosion effect?
            Destroy(gameObject);
        }
    }
}
