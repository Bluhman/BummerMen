using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextIntroScroll : MonoBehaviour {

    Vector2 startPos;
    public Vector2 endPos;
    public float scrollDuration;
    //float scrollSpeed;
    float scrollTimer;
    public int nextScene;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
        scrollTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        scrollTimer += Time.deltaTime;
        transform.position = Vector2.Lerp(startPos, endPos, scrollTimer / scrollDuration);

        if (scrollTimer >= scrollDuration)
        {
            GameController.instance.LoadNewScene(nextScene);
        }
	}
}
