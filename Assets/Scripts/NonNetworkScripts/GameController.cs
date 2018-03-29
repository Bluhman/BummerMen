using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Stores shared variables and stats, such as number of players playing.
/// The GameController remains active in every scene, and is particuarly responsible for handling background music.
/// </summary>

public class GameController : MonoBehaviour {
    
    public static GameController instance;

    public static int MAIN_MENU_INDEX = 1;

    private AudioSource music;
    [HideInInspector]
    public bool[] players;
    [HideInInspector]
    public bool versus;

    void Awake()
    {
        //if there is no gamecontroller, we set it to this version that is running.
        if (instance == null)
        {
            instance = this;
        }

        //if there is one, we don't want multiple instances of it so we trash this new one.
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //Make it so that the GameController does not get removed when switching scenes:
        DontDestroyOnLoad(gameObject);

        //And then we get references to the components of the GameController that we need.
        instance.music = instance.GetComponent<AudioSource>();
        players = new bool[8];
        ResetPlayercount();
    }

    public void PlayMusic(AudioClip song, bool loop = true)
    {
        if (music.clip == song && music.isPlaying) return;

        music.clip = song;
        music.loop = loop;
        music.Play();
    }

    public void StopMusic()
    {
        music.Stop();
    }

    public void LoadNewScene(int sceneIndex)
    {
        Time.timeScale = 1; //Used in case quitting the game while paused.
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetPlayercount()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = false;
        }
    }


}
