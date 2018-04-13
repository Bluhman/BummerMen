using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extremely simple and dumb script that plays a music piece through the gamecontroller.
/// </summary>

public class PlayMusic : MonoBehaviour {

    public AudioClip musicToPlay;
    public bool loopIt = true;

	// Use this for initialization
	void Start () {
        GameController.instance.PlayMusic(musicToPlay, loopIt);
	}
}
