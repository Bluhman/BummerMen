using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines an area within which a message will be displayed on screen.
/// </summary>

public class MessageArea : MonoBehaviour {
    
    public PlayerHUDControllerSP PHUD;
    public string Message;
    public Sprite Face;
    public float Duration;
    public bool overWrite;

	void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PHUD.ShowMessage(new PlayerHUDControllerSP.Message(Message, Face, Duration, overWrite));
            Destroy(gameObject);
        }
    }
}
