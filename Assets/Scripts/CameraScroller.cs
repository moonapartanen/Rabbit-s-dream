using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroller : MonoBehaviour {
    private GameObject player;
    private Vector3 offset;
	void Start () {
        player = GameObject.Find("hero");
        offset = transform.position - player.transform.position;
	}

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
