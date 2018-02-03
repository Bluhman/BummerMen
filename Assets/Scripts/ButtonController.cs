using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Manager : MonoBehaviour
{
	public GameObject contentToShow;//shows the content of the button clicked
	public GameObject backButton;//brings you back to menu
	public GameObject buttonsToHide;//hides all other button upon execution

	public void levelLoader(string levelToLoad) //loads a new scene upon execution
	{
		SceneManager.LoadScene(levelToLoad);
	}

	public void quitUnity()
	{
		Application.Quit();
	}
	public void showThisHideRest() //Shows the contents of one button and hides rest
	{
		contentToShow.SetActive(true);
		backButton.SetActive(true);
		buttonsToHide.SetActive(false);
	}
	public void backButtonEvent() //back button functionality, brings you back to menu
	{
		buttonsToHide.SetActive(true);
		backButton.SetActive(false);
		contentToShow.SetActive(false);
	}
}


