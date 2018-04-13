using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExplosionSP : MonoBehaviour
{

    ParticleSystem PS;
    Collider COL;

    private void Awake()
    {
        PS = GetComponent<ParticleSystem>();
        COL = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        PS.Clear();
        PS.Play();

        COL.enabled = true;

        //Destroy(gameObject, 0.5f);
        Invoke("StopDamage", 0.2f);
        Invoke("ReturnToPool", 0.5f);
    }

    void OnTriggerEnter(Collider other)
    {
        print("Explosion has hit " + other.name);
        HealthSP thingHit = other.GetComponent<HealthSP>();
        if (thingHit != null)
            thingHit.TakeDamage(1);
    }

    void StopDamage()
    {
        COL.enabled = false;
    }

    void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
