using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoring : MonoBehaviour {
    GameObject player;
    float height;
    Text score;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        score = gameObject.GetComponent<Text>();
        height = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (player != null && height < player.transform.position.y) {
            height = player.transform.position.y;
            score.text = string.Format("{0}", (int)(height+0.5f));
        }
	}
}
