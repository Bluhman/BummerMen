using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class BombSP : MonoBehaviour
{
    public GameObject model;
    public GameObject explosion;
    public LayerMask levelMask;
    private bool exploded = false;
    public bool powerBomb = false;
    public bool triggerBomb = false;
    public GameObject noiseSource;

    [HideInInspector]
    public InputDevice playerController;
    [HideInInspector]
    public PlayerSP owner;
    [HideInInspector]
    public float power;

    public float explosionTime = 4;
    private float destroyDelay = 2;
    public float hitExplosionDelay = 0.01f;

    public float bombPulseMagnitude;
    public float bombPulseSpeed;
    public float bombLocalScale = 4;
    float bombPulsePeriod;

    void Start()
    {
        bombPulsePeriod = 0f;
        if (!triggerBomb)
            Invoke("Explode", explosionTime);
    }

    public void SwapModel()
    {   
        //Change the color of the bomb to match that of the owner. Only in versus mode.
        if (GameController.instance.versus)
        {
            Material[] mats = model.GetComponent<Renderer>().materials;
            mats[0].color = owner.playerColor; //blindly guessing which one is the color here.
            model.GetComponent<Renderer>().materials = mats;
        }
        
        //This script will swap the model used by the bomb depending on what bomb type it is (trigger, power, etc). todo: ask allen and the folks to make alternate models lol like that's happening.
        if (powerBomb)
        {
            //Just recolor for now.
            //Material theMaterial = model.GetComponent<Renderer>().material;
            //theMaterial.color = new Color(1, 0.4f, 0.4f, 1);

            bombPulseMagnitude *= 2; //power bombs pulse a LOT.
        }
        if (triggerBomb)
        {
            //Just recolor for now.
            //Material theMaterial = model.GetComponent<Renderer>().material;
            //theMaterial.color = new Color(0.4f, 1, 1, 1);

            bombPulseMagnitude = 0f; //Trigger bombs don't pulse.
        }
    }

    private void Update()
    {
        bombPulsePeriod += Time.deltaTime;
        float currentSize = bombLocalScale + bombPulseMagnitude * Mathf.Sin(bombPulseSpeed * bombPulsePeriod);
        model.transform.localScale = new Vector3(currentSize, currentSize, currentSize);

        if (triggerBomb)
        {
            //Input detector for trigger bombs.
            if (playerController.Action2.WasPressed && !exploded)
            {
                Explode();
            }
        }
    }

    public void Explode()
    {
        GameObject.Instantiate(noiseSource);

        exploded = true;

        //GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity);
        GameObject expl = ExplosionPooler.SharedInstance.GetPooledObject("Explosion");
        if (expl != null)
        {
            expl.transform.position = transform.position;
            expl.transform.rotation = Quaternion.identity;
            expl.SetActive(true);
        }

        //Cross-shaped explosion
        /*
        CreateExplosions(Vector3.forward);
        CreateExplosions(Vector3.right);
        CreateExplosions(Vector3.back);
        CreateExplosions(Vector3.left);
        */

        //Hybrid explosion version:
        //This will only execute sphere-explosion code if it's verified that the
        //space diagonal to the bomb does not contain a solid block. If it does,
        //The diagonals for that quadrant will be excluded from computation.

        //The more powerful the bomb is, the more diagonals we need to fill in possible gaps.
        //So the higher the power, the smaller our rotation factor will be, to fill in more angles.
        float rotationFactor = 90;
        if (power != 0) rotationFactor /= power;
        rotationFactor = Mathf.Max(rotationFactor, 15);

        for (float i = 0; i < 360; i+=90)
        {
            //Start by creating explosions in the base direction (i will only ever be 0, 90, 180, or 270 degrees, mapping to cardinal directions.)
            Vector3 cardinalDirection = new Vector3(Mathf.Sin(Mathf.Deg2Rad * i), 0, Mathf.Cos(Mathf.Deg2Rad * i));
            CreateExplosions(cardinalDirection);

            //Next we want to see whether there is anything occupying the direct diagonal of the bomb:
            //Start by determining the angle needed to check the diagonal
            float diagonalAngle = i + 45;
            Vector3 diagonalDirection = new Vector3(Mathf.Sin(Mathf.Deg2Rad * diagonalAngle), 0, Mathf.Cos(Mathf.Deg2Rad * diagonalAngle));

            //Then we raycast in that direction. Only stop if there is a solid barrier in the given direction, 1 tile away.
            bool barrierBlocking = false;
            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3(0, .5f, 0), diagonalDirection, out hit, Mathf.Sqrt(2.0f), levelMask); //We use square root of 2, as that's the length of the hypotenuse of a triangle with legs that are 1 tile long each.
            if (hit.collider)
            {
                if (hit.collider.CompareTag("Barrier"))
                {
                    barrierBlocking = true;
                }
            }

            if (!barrierBlocking || powerBomb)
            {
                print("No diagonal from " + diagonalAngle);
                
                //If there is no solid block in the way, we will now calculate all the diagonal rays that a bomb of a given size would extrude.
                for (float j = i + rotationFactor; j < i + 90; j+=rotationFactor)
                {
                    Vector3 rayDirection = new Vector3(Mathf.Sin(Mathf.Deg2Rad * j), 0, Mathf.Cos(Mathf.Deg2Rad * j));
                    CreateExplosions(rayDirection);
                }
            }
        }

        /*
        //Let's try something weird.
        float rotationFactor = 90;
        if (power != 0) rotationFactor /= power;
        rotationFactor = Mathf.Max(rotationFactor, 15);
        //print("Rotationfactor is " + rotationFactor);

        //Sphere explosion:
        for (float i = 0; i < 360; i+=rotationFactor)
        {
            Vector3 weirdDirection = new Vector3(Mathf.Sin(Mathf.Deg2Rad * i), 0, Mathf.Cos(Mathf.Deg2Rad * i));
            CreateExplosions(weirdDirection);
        }
        */

        model.SetActive(false);
        Destroy(gameObject, .3f);
    }

    private void CreateExplosions(Vector3 direction)
    {
        for (int i = 1; i <= power; i++)
        {
           
            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3(0, .5f, 0), direction, out hit, i, levelMask);

            if (!hit.collider || powerBomb)
            {
                //GameObject expl =Instantiate(explosion, transform.position + (i * direction),
                //  explosion.transform.rotation);
                GameObject expl = ExplosionPooler.SharedInstance.GetPooledObject("Explosion");
                if (expl != null)
                {
                    expl.transform.position = transform.position + (i*direction);
                    expl.transform.rotation = explosion.transform.rotation;
                    expl.SetActive(true);
                }
            }
            else
            {
                if (hit.collider.GetComponent<HealthSP>() != null)
                {
                    //  GameObject expl = Instantiate(explosion, transform.position + (i * direction),
                    //explosion.transform.rotation);
                    GameObject expl = ExplosionPooler.SharedInstance.GetPooledObject("Explosion");
                    if (expl != null)
                    {
                        expl.transform.position = transform.position + (i * direction);
                        expl.transform.rotation = explosion.transform.rotation;
                        expl.SetActive(true);
                    }
                }
                break;
            }

            //yield return new WaitForSeconds(.0f);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //print(other.tag);

        if (!exploded && other.CompareTag("Explosion"))
        {
            CancelInvoke("Explode");
            //Explode();
            Invoke("Explode", hitExplosionDelay);
        }
    }

    void OnDestroy()
    {
        //print("Script was destroyed");
        owner.currentBombs--;
    }

}
