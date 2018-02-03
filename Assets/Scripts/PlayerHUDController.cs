using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDController : MonoBehaviour {

    public Player player;
    public Text GameOverText;
    public Text LifeCounterText;

	// Use this for initialization
	void Start () {
        GameOverText.enabled = false;
        UpdateLives();
	}
	
    public void UpdateLives()
    {
        LifeCounterText.text = " x " +player.currentLives;

        if (player.currentLives <= 0)
        {
            GameOverText.enabled = true;
        }
    }
}
