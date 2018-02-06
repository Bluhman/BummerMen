using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDControllerSP : MonoBehaviour {

    public PlayerSP playerSP;
    public Text GameOverText;
    public Text LifeCounterText;
    public Text TimerText;
    public GameObject pauseMenu;
    public float timeLimit;
    float timer;
    [HideInInspector]
    public bool paused = false;

	// Use this for initialization
	void Start () {
        GameOverText.enabled = false;
        pauseMenu.SetActive(false);
        UpdateLives();
        timer = timeLimit;
	}

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            print("PAUSE?");
            TogglePause();
        }

        if (timer > 0 && !paused)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = 0;
                playerSP.currentLives = 0;
                UpdateLives();
                Time.timeScale = 0;
                paused = true;
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

    public void TogglePause()
    {
        //Don't let us call up the pause menu if we game-overed.
        if (GameOverText.enabled) return;

        if (!paused)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }

        else
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }

        paused = !paused;
    }
}
