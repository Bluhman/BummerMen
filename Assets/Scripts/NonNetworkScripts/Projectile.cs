using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float speed;
    public bool destroyOnBarriers = true;
    public bool destroyOnHit = true;
    public bool destroysBlocks = false;
    public bool detonatesBombs = false;
    BrickSpawnerSP BSSP;

	// Use this for initialization
	void Start () {
        BSSP = FindObjectOfType<BrickSpawnerSP>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.forward * speed * Time.deltaTime;

        //destroy self if it leaves the bounds of the map.
        if (transform.position.x < -BSSP.mapSizeX / 2 || transform.position.x > BSSP.mapSizeX / 2 || transform.position.z < -BSSP.mapSizeY / 2 || transform.position.z > BSSP.mapSizeY / 2)
        {
            Destroy(gameObject);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        print("Projectile hit " + other.name);

        //ignore enemies, because only enemies can fire projectiles.
        if (other.CompareTag("Enemy")) return;

        HealthSP thingHit = other.GetComponent<HealthSP>();
        if (thingHit != null)
        {
            if (destroysBlocks || other.gameObject.layer != 8)
                thingHit.TakeDamage(1);
        }

        BombSP daBomb = other.GetComponent<BombSP>();
        if (daBomb != null && detonatesBombs)
        {
            Destroy(gameObject);
            daBomb.Explode();
        }

        if (destroyOnHit)
            Destroy(gameObject);

        if (destroyOnBarriers && other.gameObject.layer == 8)
            Destroy(gameObject);

    }
}
