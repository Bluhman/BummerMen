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
    public float delay = 0;

    private bool consumed;

    void Start()
    {
        consumed = false;
    }

	void OnTriggerStay(Collider other)
    {
        if (consumed) return;

        if (other.CompareTag("Player"))
        {
            consumed = true;
            StartCoroutine(getMessage());
        }
    }

    IEnumerator getMessage()
    {
        yield return new WaitForSeconds(delay);
        PHUD.ShowMessage(new PlayerHUDControllerSP.Message(Message, Face, Duration, overWrite));
        Destroy(gameObject);
    }
}
