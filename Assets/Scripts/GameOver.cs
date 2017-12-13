using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
    GameObject player;
    Text gameOver;

	// Use this for initialization
	void Start () {
        gameOver = GetComponent<Text>();
        gameOver.enabled = false;
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(player.name);
	}
	
	// Update is called once per frame
	void Update () {
		if(player == null && gameOver != null) {
            gameOver.enabled = true;
            gameOver = null;
        }
	}
}
