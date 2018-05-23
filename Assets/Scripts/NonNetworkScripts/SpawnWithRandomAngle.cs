using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objects with this script will just spawn at a random right angle, used on stone objects to increase organic appearance.
/// </summary>


public class SpawnWithRandomAngle : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector3 eulerRotation = new Vector3(0, Random.Range(0, 5) * 90, 0);
        transform.Rotate(eulerRotation, Space.World);
        Destroy(this);
	}
}
