using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersusPlayerSlotUI : MonoBehaviour {

    public int playerNumber;
    public Text statusTag;
    public Text nameTag;
    bool ready;
    public string playerStartButton = "Start_P1";
    public string playerCancelButton = "Cancel_P1";
    Color baseColor;
    public Color playerColor;
    Image myImage;

	// Use this for initialization
	void Start () {
        nameTag.text = "Player " + playerNumber;
        myImage = GetComponent<Image>();
        baseColor = myImage.color;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown(playerStartButton) && !ready)
        {
            print("START PRESSED!");
            ready = true;
            statusTag.text = "READY!";
            myImage.color = playerColor;
        }

        if (Input.GetButtonDown(playerCancelButton) && ready)
        {
            ready = false;
            statusTag.text = "Press Start to Join";
            myImage.color = baseColor;
        }
	}
}
