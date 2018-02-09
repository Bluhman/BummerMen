using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnScript : MonoBehaviour {

    AudioSource AS;
    public float soundDuration;
    public float pitchRandomizerRange;

	// Use this for initialization
	void Start () {
        AS = GetComponent<AudioSource>();
        AS.pitch = Random.Range(1 - pitchRandomizerRange, 1 + pitchRandomizerRange);
        Destroy(gameObject, soundDuration);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
