using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a camera that will track after the position of a player object.
/// </summary>

public class PlayerTrackCamera : MonoBehaviour {
    public GameObject[] playersToTrack;
    private GameObject listenerProbe;
    private Vector3 playerPosition;
    private Vector3 lastKnownLegalPosition;
    
    public float xAngle = 70f;
    public float cameraDistance = 30f;
    //wiggle distance determines how far the player can move before the camera begins to follow them.
    public float playerWiggleDistance = 3f;
    //the following variables determine minimum and maximum position in X and Z coordinates.
    public float XMin;
    public float XMax;
    public float ZMin;
    public float ZMax;

    public float cameraTightness = 0.99f;
    public float distanceModifier;

    float maxDistanceBetweenPlayers;


	// Use this for initialization
	void Start () {
        //get the camera's rotation right.
        transform.rotation = Quaternion.Euler(xAngle, 0, 0);
        Vector3 averagePlayerPosition = Vector3.zero;
        foreach (GameObject player in playersToTrack)
        {
            averagePlayerPosition += player.transform.position;
        }
        averagePlayerPosition /= playersToTrack.Length;
        lastKnownLegalPosition = averagePlayerPosition;

        //get reference to the listener probe, and move it forward to match the elevation of the map.
        listenerProbe = transform.GetChild(0).gameObject;
        listenerProbe.transform.position += transform.forward * cameraDistance;

        maxDistanceBetweenPlayers = 0;

    }
	
	// Update is called once per frame
	void LateUpdate () {
        int playersActive = 0;
        Vector3 averagePlayerPosition = Vector3.zero;
        foreach (GameObject player in playersToTrack)
        {
            if (!player.activeInHierarchy) continue;
            averagePlayerPosition += player.transform.position;
            playersActive++;
        }
        if (playersActive <= 0) return;
        else averagePlayerPosition /= playersActive;

        playerPosition = averagePlayerPosition;

        //We want to smooth our camera experience so let us use LERPs instead of hard transitioning. This means copying over our current camera position.
        Vector3 newPosition = transform.position;

        //If the last known legal position of the player is beyond the bounds of the wiggle distance defined, we want to move it towards the current player's position.
        //also put a failsafe here because I hate Unity freezing:
        int loopLimit = 10000;
        while ((lastKnownLegalPosition - playerPosition).magnitude > playerWiggleDistance && loopLimit > 0)
        {
            lastKnownLegalPosition = Vector3.MoveTowards(lastKnownLegalPosition, playerPosition, 0.01f);
            loopLimit--;
            if (loopLimit <= 0) print("MICAH YOU DONE GOOFED UP.");
        }

        //Measure out the maximum distance between characters
        maxDistanceBetweenPlayers = 0;
        for (int a = 0; a < playersToTrack.Length; a++)
        {
            if (!playersToTrack[a].activeInHierarchy) continue;
            for (int b = a; b < playersToTrack.Length; b++)
            {
                if (!playersToTrack[b].activeInHierarchy) continue;
                if (a == b) continue;
                float distance = Vector3.Distance(playersToTrack[a].transform.position, playersToTrack[b].transform.position);
                if (distance > maxDistanceBetweenPlayers) maxDistanceBetweenPlayers = distance;
            }
        }

        //Simple part is to match the camera's position to that of the player.
        newPosition = lastKnownLegalPosition;
        newPosition -= transform.forward * (cameraDistance + maxDistanceBetweenPlayers * distanceModifier);

        //Bind the camera's position to that of the play area according to the mins and maxes:
        newPosition = new Vector3(Mathf.Clamp(newPosition.x, XMin, XMax), newPosition.y, Mathf.Clamp(newPosition.z, ZMin, ZMax));

        //lerp
        transform.position = Vector3.Lerp(transform.position, newPosition, cameraTightness);
    }
}
