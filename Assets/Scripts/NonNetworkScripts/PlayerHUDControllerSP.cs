using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDControllerSP : MonoBehaviour {

    public PlayerSP playerSP;
    public Text GameOverText;
    public Text LifeCounterText;
    public Text TimerText;
    public float timeLimit;
    float timer;

	// Use this for initialization
	void Start () {
        GameOverText.enabled = false;
        UpdateLives();
        timer = timeLimit;
	}

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = 0;
                playerSP.currentLives = 0;
                UpdateLives();
                Time.timeScale = 0;
            }
            string minSec = string.Format("{0}:{1:00}", (int)timer / 60, (int)timer % 60);
            TimerText.text = minSec;
        }
    }
	
    public void UpdateLives()
    {
        LifeCounterText.text = " x " +playerSP.currentLives;

        if (playerSP.currentLives <= 0)
        {
            GameOverText.enabled = true;
        }
    }
}
