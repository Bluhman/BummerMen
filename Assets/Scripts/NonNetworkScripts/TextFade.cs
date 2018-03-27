using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fades a line of text between two colors.
/// </summary>

public class TextFade : MonoBehaviour {

    Text theText;
    public Color baseColor;
    public Color fadeColor;
    public float fadeDuration;
    float fadeTimer;
    bool backwards = false;

	// Use this for initialization
	void Start () {
        fadeTimer = 0;
        theText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (backwards)
            fadeTimer -= Time.deltaTime;
        else
            fadeTimer += Time.deltaTime;

        if (fadeTimer <= 0) backwards = false;
        if (fadeTimer >= fadeDuration) backwards = true;

        theText.color = Color.Lerp(baseColor, fadeColor, fadeTimer / fadeDuration);


	}
}
