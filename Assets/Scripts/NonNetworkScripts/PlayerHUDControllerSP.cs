﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class PlayerHUDControllerSP : MonoBehaviour {

    public PlayerSP[] playersSP;
    public GameObject GameOverText;
    public GameObject WinText;
    public GameObject lifeCounterTemplate;
    LifeIndicatorTracker[] lifeCounters;
    public float counterSpacing;
    public Text TimerText;
    public GameObject pauseMenu;
    public Image dialogBox;
    public Image dialogPortrait;
    public Text dialogText;
    public float timeLimit;
    float timer;
    [HideInInspector]
    public bool paused = false;
    [HideInInspector]
    public bool gameOver;
    public bool versus = false;
    int currentWinner;
    public int enemiesAlive;

    public AudioClip winMusic;
    public AudioClip loseMusic;
    public int nextLevel;

    public float typeInTime; //time it takes for each letter to be typed into the box.
    float typeInTimer;
    [HideInInspector]
    public float messageDisplayTime;
    [HideInInspector]
    string displayMessage;
    [HideInInspector]
    public float timeAfterMessage;
    bool showingMessage;
    public Vector2 messageBoxOffscreenPos;
    public Vector2 messageBoxOnscreenPos;
    public Sprite testSprite;

    Queue<Message> messageQueue;
    Message currentMessage;

    public enum winCondition { KillAll, ReachExit }
    public winCondition howToWin;


	// Use this for initialization
	void Start () {
        messageQueue = new Queue<Message>();
        GameOverText.SetActive(false);
        pauseMenu.SetActive(false);
        gameOver = false;
        WinText.SetActive(false);
        enemiesAlive = GameObject.FindGameObjectsWithTag("Enemy").Length;
        print("map has " + enemiesAlive + " enemies");
        
        timer = timeLimit;
        currentMessage = new Message("", null, 0);

        GameController.instance.versus = this.versus;

        lifeCounters = new LifeIndicatorTracker[playersSP.Length];

        for (int i = 0; i < playersSP.Length; i++)
        {
            if (!playersSP[i].isActiveAndEnabled) continue;

            GameObject lifeCounter = Instantiate(lifeCounterTemplate, transform);
            lifeCounters[i] = lifeCounter.GetComponentInChildren<LifeIndicatorTracker>();
            lifeCounters[i].playerSymbol.color = playersSP[i].playerColor;
            if (versus)
            {
                //Lists life counters vertically.
                lifeCounter.transform.position = new Vector2(lifeCounter.transform.position.x , lifeCounter.transform.position.y - counterSpacing * i);
                if (i > 0)
                    TimerText.transform.position -= new Vector3(0 , counterSpacing);
            }
            else
            {
                //Lists life counters horizontally.
                lifeCounter.transform.position = new Vector2(lifeCounter.transform.position.x + counterSpacing * i, lifeCounter.transform.position.y);
            }
            //print(lifeCounter.transform.position);
        }
        UpdateLives();

        

        //Testing message display functions.
        //ShowMessage(new Message("This is a test message", null, 3));
        //ShowMessage(new Message("Isn't that neat?", null, 3));
        //ShowMessage(new Message("I can also change my face!", testSprite, 5));
    }

    void Update()
    {
        

        if (gameOver)
        {
            if (InputManager.ActiveDevice.MenuWasPressed && GameOverText.activeInHierarchy)
            {
                //Return to title screen.
                if (versus)
                    GameController.instance.LoadNewScene(GameController.VERSUS_MENU_INDEX);
                else
                    GameController.instance.LoadNewScene(GameController.MAIN_MENU_INDEX);
            }

            if (InputManager.ActiveDevice.MenuWasPressed && WinText.activeInHierarchy)
            {
                //Move to next level
                GameController.instance.LoadNewScene(nextLevel);
            }
            return;
        }

        if (InputManager.ActiveDevice.MenuWasPressed && !versus)
        {
            print("PAUSE?");
            TogglePause();
        }

        HandleMessages();

        if (timer <= -1)
        {
            //no time limit.
            TimerText.text = "";
            return;
        }

        if (timer > 0 && !paused)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = 0;
                for (int i = 0; i < playersSP.Length; i++)
                    playersSP[i].currentLives = 0;
                UpdateLives();
                Time.timeScale = 0;
                paused = true;
            }
            string minSec = string.Format("{0}:{1:00}", (int)timer / 60, (int)timer % 60);
            TimerText.text = minSec;
        }
        
    }
	
    public void AddOrRemoveEnemy(int value)
    {
        enemiesAlive += value;

        if (enemiesAlive <= 0 && howToWin == winCondition.KillAll)
        {
            Win();
        }
    }

    public void Win()
    {
        //WE WIN.
        gameOver = true;
        GameController.instance.PlayMusic(winMusic, false);
        WinText.SetActive(true);
        playersSP[0].sendStats();
        //Time.timeScale = 0;
    }

    public void UpdateLives()
    {
        print("UPDATELIVES");

        bool allDead = true;
        bool oneRemains = true;

        for (int i = 0; i < lifeCounters.Length; i++)
        {
            if (!playersSP[i].isActiveAndEnabled) continue;

            lifeCounters[i].updateStats(playersSP[i].GetComponent<HealthSP>().currentHealth, playersSP[i].currentLives);
            if (playersSP[i].currentLives > 0)
            {
                if (!allDead)
                {
                    oneRemains = false;
                    print("there is more than one remaining.");
                }
                allDead = false;
                currentWinner = i + 1;
                print("not all are dead.");
            }
        }

        if (allDead)
        {
            if (versus)
            {
                Text theText = GameOverText.GetComponent<Text>();
                GameController.instance.PlayMusic(loseMusic, false);
                if (theText != null)
                {
                    theText.text = "DRAW";
                }
                //Time.timeScale = 0;
                //paused = true;
            }

            GameOverText.SetActive(true);
            gameOver = true;
            GameController.instance.PlayMusic(loseMusic, false);
            print("I think everyone is dead.");
        }

        if (!allDead && oneRemains && versus)
        {
            Text theText = GameOverText.GetComponent<Text>();
            if (theText != null)
            {
                theText.text = "Player " + currentWinner + " wins!";
            }
            GameOverText.SetActive(true);
            gameOver = true;
            GameController.instance.PlayMusic(winMusic, false);
            //Time.timeScale = 0;
            //paused = true;
            //print("I think there is one winner.");
        }
    }

    public void TogglePause()
    {
        //Don't let us call up the pause menu if we game-overed.
        if (GameOverText.activeSelf || WinText.activeSelf) return;

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

    public void ShowMessage(Message nextMessage)
    {
        if (nextMessage.overWrite)
        {
            messageQueue.Clear();
            showingMessage = false;
        }
        messageQueue.Enqueue(nextMessage);
    }

    void HandleMessages()
    {
        if (!showingMessage)
        {
            //clear dialogue
            dialogText.text = "";
            
            //Move text box offscreen.
            dialogBox.rectTransform.anchoredPosition = Vector2.Lerp(dialogBox.rectTransform.anchoredPosition, messageBoxOffscreenPos, 0.6f);

            if (messageQueue.Count > 0)
            {
                dialogText.text = "";
                showingMessage = true;
                currentMessage = messageQueue.Dequeue();
                timeAfterMessage = currentMessage.duration;
                displayMessage = currentMessage.text;
                typeInTimer = typeInTime;
                if (currentMessage.portrait != null)
                {
                    dialogPortrait.sprite = currentMessage.portrait;
                }
            }
        }
        else
        {
            //Move the text box onscreen.
            dialogBox.rectTransform.anchoredPosition = Vector2.Lerp(dialogBox.rectTransform.anchoredPosition, messageBoxOnscreenPos, 0.6f);

            //Type in dialog letters one by one.
            if (displayMessage.Length > 0)
            {
                typeInTimer -= Time.deltaTime;
                if (typeInTimer <= 0)
                {
                    dialogText.text += displayMessage[0];
                    displayMessage = displayMessage.Remove(0,1);
                    //print(displayMessage);
                    typeInTimer = typeInTime;
                }
            }

            if (displayMessage.Length <= 0)
            {
                timeAfterMessage -= Time.deltaTime;
                if (timeAfterMessage <= 0)
                {
                    if (messageQueue.Count > 0)
                    {
                        dialogText.text = "";
                        currentMessage = messageQueue.Dequeue();
                        timeAfterMessage = currentMessage.duration;
                        displayMessage = currentMessage.text;
                        typeInTimer = typeInTime;
                        if (currentMessage.portrait != null)
                        {
                            dialogPortrait.sprite = currentMessage.portrait;
                        }
                    }
                    else
                    {
                        showingMessage = false;
                    }
                }
            }
            
        }
    }

    public struct Message
    {
        public string text;
        public Sprite portrait;
        public float duration;
        public bool overWrite;

        public Message (string message, Sprite face, float duration, bool overWriteOldMessage = false)
        {
            text = message;
            portrait = face;
            this.duration = duration;
            overWrite = overWriteOldMessage;
        }
    }
}
