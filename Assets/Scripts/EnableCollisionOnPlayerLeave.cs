using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableCollisionOnPlayerLeave : MonoBehaviour {

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        { // When the player exits the trigger area
            GetComponent<Collider>().isTrigger = false; // Disable the trigger
        }
    }
}
