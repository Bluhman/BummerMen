using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

/// <summary>
/// Lets the user use the Start button to move to a new scene.
/// </summary>

public class StartButton : MonoBehaviour {

    public int newScene;
    
	
	// Update is called once per frame
	void Update () {
		if (InputManager.ActiveDevice.MenuWasPressed)
        {
            print("LOAD IT!");
            GameController.instance.LoadNewScene(newScene);
        }
	}
}
