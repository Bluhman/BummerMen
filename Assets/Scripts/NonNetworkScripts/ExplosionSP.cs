using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExplosionSP : MonoBehaviour
{

    ParticleSystem PS;

    private void Awake()
    {
        PS = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        PS.Clear();
        PS.Play();

        //Destroy(gameObject, 0.5f);
        Invoke("ReturnToPool", 0.5f);
    }

    void OnTriggerEnter(Collider other)
    {
        print("Explosion has hit " + other.name);
        HealthSP thingHit = other.GetComponent<HealthSP>();
        if (thingHit != null)
            thingHit.TakeDamage(1);
    }

    void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
