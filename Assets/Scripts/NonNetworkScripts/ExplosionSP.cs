using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExplosionSP : MonoBehaviour
{

    private void Start()
    {

        Destroy(gameObject, 0.5f);
    }

    void OnTriggerEnter(Collider other)
    {
        print("Explosion has hit " + other.name);
        HealthSP thingHit = other.GetComponent<HealthSP>();
        if (thingHit != null)
            thingHit.TakeDamage(1);
    }
}
