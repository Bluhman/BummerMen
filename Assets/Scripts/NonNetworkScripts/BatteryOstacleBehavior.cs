using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryOstacleBehavior : MonoBehaviour {

    public BatteryOstacleBehavior nextConduit;
    public ParticleSystem electricity;
    public bool charged;
    public float chargeTime;
    public GameObject lightningBoltEffect;

    float chargeTimer;
    Vector3 boltDirection;
    float boltDistance;
    Vector3 halfwayPoint;

    AudioSource AS;

    void Start()
    {
        AS = GetComponent<AudioSource>();
        SetCharge(charged);
        chargeTimer = 0f;
        GameObject otherConduitObject = nextConduit.gameObject;
        boltDirection = otherConduitObject.transform.position - transform.position;
        halfwayPoint = (otherConduitObject.transform.position + transform.position) / 2;
        boltDistance = Vector3.Distance(transform.position, otherConduitObject.transform.position);
    }
	
	// Update is called once per frame
	void Update () {
		if (charged)
        {
            chargeTimer += Time.deltaTime;

            if (chargeTimer >= chargeTime)
            {
                chargeTimer = 0f;
                Discharge();
            }
        }
	}

    void Discharge()
    {
        SetCharge(false);
        nextConduit.SetCharge(true);
        
        GameObject lightningBolt = Instantiate(lightningBoltEffect, halfwayPoint, Quaternion.LookRotation(boltDirection));
        lightningBolt.transform.localScale = new Vector3(0.1f, 0.1f, boltDistance);

        AS.Play();

        print("blap");
        RaycastHit[] hits = Physics.RaycastAll(transform.position, boltDirection, boltDistance);
        for (int i = 0; i < hits.Length; i++)
        {
            print("ZAPP");
            HealthSP targetHP = hits[i].transform.gameObject.GetComponent<HealthSP>();
            if (targetHP != null)
                if (targetHP.zappable)
                    targetHP.TakeDamage(1, false);
            if (hits[i].transform.CompareTag("bomb"))
            {
                hits[i].transform.gameObject.GetComponent<BombSP>().Explode();
            }
        }


    }

    public void SetCharge(bool setValue)
    {
        charged = setValue;

        if (!setValue)
        {
            electricity.Clear();
            electricity.Stop();
        }
        else
        {
            electricity.Play();
        }
    }
}
