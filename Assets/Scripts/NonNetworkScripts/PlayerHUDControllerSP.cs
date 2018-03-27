using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDControllerSP : MonoBehaviour {

    public PlayerSP[] playersSP;
    public GameObject GameOverText;
    public GameObject lifeCounterTemplate;
    Text[] lifeCounters;
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
    bool gameOver;
    public bool versus = false;
    int currentWinner;


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


	// Use this for initialization
	void Start () {
        messageQueue = new Queue<Message>();
        GameOverText.SetActive(false);
        pauseMenu.SetActive(false);
        gameOver = false;
        
        timer = timeLimit;
        currentMessage = new Message("", null, 0);

        lifeCounters = new Text[playersSP.Length];

        for (int i = 0; i < playersSP.Length; i++)
        {
            if (!playersSP[i].isActiveAndEnabled) continue;

            GameObject lifeCounter = Instantiate(lifeCounterTemplate, transform);
            lifeCounters[i] = lifeCounter.GetComponentInChildren<Text>();
            lifeCounter.GetComponentInChildren<Image>().color = playersSP[i].playerColor;
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

        GameController.instance.versus = this.versus;

        //Testing message display functions.
        //ShowMessage(new Message("This is a test message", null, 3));
        //ShowMessage(new Message("Isn't that neat?", null, 3));
        //ShowMessage(new Message("I can also change my face!", testSprite, 5));
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            print("PAUSE?");
            TogglePause();
        }

        if (gameOver)
        {
            if (Input.GetButtonDown("Submit"))
            {
                //Return to title screen.
                GameController.instance.LoadNewScene(GameController.MAIN_MENU_INDEX);
            }
        }

        HandleMessages();

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
	
    public void UpdateLives()
    {
        print("UPDATELIVES");

        bool allDead = true;
        bool oneRemains = true;

        for (int i = 0; i < lifeCounters.Length; i++)
        {
            if (!playersSP[i].isActiveAndEnabled) continue;

            lifeCounters[i].text = " x " + playersSP[i].currentLives;
            if (playersSP[i].currentLives > 0)
            {
                if (!allDead)
                {
                    oneRemains = false;
                    //print("there is more than one remaining.");
                }
                allDead = false;
                currentWinner = i + 1;
                //print("not all are dead.");
            }

            if (allDead)
            {
                if (versus)
                {
                    Text theText = GameOverText.GetComponent<Text>();
                    if (theText != null)
                    {
                        theText.text = "DRAW";
                    }
                    Time.timeScale = 0;
                    paused = true;
                }

                GameOverText.SetActive(true);
                gameOver = true;
                //print("I think everyone is dead.");
            }

            
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
            Time.timeScale = 0;
            paused = true;
            //print("I think there is one winner.");
        }
    }

    public void TogglePause()
    {
        //Don't let us call up the pause menu if we game-overed.
        if (GameOverText.activeSelf) return;

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

        public Message (string message, Sprite face, float duration)
        {
            text = message;
            portrait = face;
            this.duration = duration;
        }
    }
}
