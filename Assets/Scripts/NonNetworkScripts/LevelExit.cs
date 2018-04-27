using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour {

    public PlayerHUDControllerSP playerHud;

    private void OnTriggerEnter(Collider other)
    {
        if (playerHud.howToWin == PlayerHUDControllerSP.winCondition.ReachExit)
        {
            playerHud.Win();
            Destroy(gameObject);
        }
    }
}
