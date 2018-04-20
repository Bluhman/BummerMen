using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps track of the different components of a life indicator, to show all stats. Everything in this class is public.
/// </summary>
public class LifeIndicatorTracker : MonoBehaviour {

    public Image playerSymbol;
    public Text livesLeft;
    string lifeTemplate = "Lives: ";
    public Slider healthSlider;

    public void updateStats(int newHealth, int newLives)
    {
        livesLeft.text = lifeTemplate + newLives;
        healthSlider.value = newHealth;
    }
}
