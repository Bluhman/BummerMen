using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombspawn : MonoBehaviour
{
    public Rigidbody bomb1;
    public Rigidbody bomb2;
    public float bombSpeed1 = 1;
    public float bombSpeed2 = 5;
    public float spawnTime = 3;
    public bool setBomb;



    private void Start()
    {
        setBomb = true;
    }
    void FixedUpdate () {

        if (!setBomb)
        {
            spawnTime -= Time.deltaTime;
            if (spawnTime < 0)
            {
                setBomb = true;

            }
        }
        
        if (setBomb)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StationaryBomb();
                setBomb = false;
                spawnTime = 3;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                RollingBomb();
                setBomb = false;
                spawnTime = 3;
            }
        }
    }
    void StationaryBomb()
    {
        Rigidbody bomb1Clone = (Rigidbody) Instantiate(bomb1, transform.position - (transform.forward * 1), transform.rotation);
        bomb1Clone.velocity = transform.forward * bombSpeed1;
    }
    void RollingBomb()
    {
        Rigidbody bomb2Clone = (Rigidbody) Instantiate(bomb2, transform.position + (transform.forward * 1), transform.rotation);
        bomb2Clone.velocity = transform.forward * bombSpeed2;
    }
}
