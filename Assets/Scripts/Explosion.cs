using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Explosion : NetworkBehaviour
{

    private void Start()
    {

        Destroy(gameObject, 0.5f);
    }

    void OnTriggerEnter(Collider other)
    {
        print("Explosion has hit " + other.name);
        Health thingHit = other.GetComponent<Health>();
        if (thingHit != null)
            thingHit.TakeDamage(1);
    }
}
