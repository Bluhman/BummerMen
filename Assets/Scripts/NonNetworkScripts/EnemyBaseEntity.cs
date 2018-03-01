using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Basic enemy controls, defining how the enemy interacts with obstacles.
/// This example will just move randomly in cardinal directions.
/// </summary>
public class EnemyBaseEntity : MonoBehaviour {

    public float moveTime;
    public float turnSpeed;
    int xGridPos;
    int yGridPos;
    public float gridSize;
    Vector3 currentLocation;
    [HideInInspector]
    public Vector3 nextLocation;
    float movePercentage;
    [HideInInspector]
    public bool dead = false;

    void Awake()
    {
        currentLocation = transform.position;
        nextLocation = currentLocation;
        movePercentage = 0;
        setGridPositionFromRealPos();
        dead = false;
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        if (dead)
        {
            //DEATH ANIMATION WHOO
            transform.Rotate(transform.up, 1080 * Time.deltaTime);
            transform.position += transform.up * 30 * Time.deltaTime;
            return;
        }

        if (nextLocation == null || nextLocation == currentLocation)
        {
            FindNextLocation();
        }
        //not using an else here, as this will run after a nextlocation has been defined.
        if (nextLocation != null)
        {
            //Rotate the player to where the intended movement is facing.
            transform.LookAt(nextLocation);

            //MOVE IT.
            movePercentage += (1 / moveTime) * Time.deltaTime;
            transform.position = Vector3.Lerp(currentLocation, nextLocation, movePercentage);
            if (movePercentage >= 1)
            {
                //reset all the stuff
                movePercentage = 0;
                currentLocation = nextLocation;
            }
        }
	}

    //Navigation logic for the enemy is put here. Optimally you'd sub out the contents of this function to make for some strategy.
    //For now this just picks one of four random cardinal directions to move in.
    public virtual void FindNextLocation()
    {
        List<Vector3> locations = new List<Vector3>();
        for (int a = 0; a < 360; a += 90)
        {
            RaycastHit hit;
            Vector3 positionFacing = new Vector3(Mathf.Cos(Mathf.Deg2Rad * a), 0, Mathf.Sin(Mathf.Deg2Rad * a));
            if (Physics.Raycast(transform.position, positionFacing, out hit, gridSize))
            {
                if (!hit.collider.CompareTag("Player"))
                {
                    //print(hit.transform.name + " at " + positionFacing);
                    continue;
                }
            }
            //print(a + " is an option");
            locations.Add(transform.position + positionFacing);
        }
        //print(locations.Count + " possible spots");
        if (locations.Count > 0)
        {
            nextLocation = locations[Random.Range(0, locations.Count)];
        }
    }

    //Translates the x and z position of the player into x and y position on the game grid. Used to determine where a bomb would be laid.
    void setGridPositionFromRealPos()
    {
        xGridPos = Mathf.RoundToInt(transform.position.x);
        yGridPos = Mathf.RoundToInt(transform.position.z);
        //print(xGridPos + ", " + yGridPos);
    }

    //Damages the player when they collide.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !dead)
        {
            HealthSP thingHit = other.GetComponent<HealthSP>();
            if (thingHit != null)
                thingHit.TakeDamage(1);
        }
    }

    public void DeathAnimation()
    {
        dead = true;
        Destroy(gameObject, 1.0f);
    }
}
