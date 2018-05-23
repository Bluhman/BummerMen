using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Counts the players joined on the Versus menu screen and helps the Start button operate.
/// </summary>

public class VersusMenuPlayerCounter : MonoBehaviour {

    public int playersIn;
    public Button playButton;

	// Use this for initialization
	void Start () {
        playersIn = 0;
        GameController.instance.ResetPlayercount();
        GameController.instance.versus = true;
	}
    
    public void ChangePlayercount(int count)
    {
        playersIn += count;
    }

    public void Play(int sceneIndex)
    {
        if (playersIn >= 2)
        {
            GameController.instance.LoadNewScene(sceneIndex);
        }
    }
}
