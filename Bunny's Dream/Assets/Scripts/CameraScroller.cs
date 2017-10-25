using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroller : MonoBehaviour {

    private Rigidbody2D rb2d;
    public float speed;
    [SerializeField] private bool m_StopScrolling;

	void Start () {
        speed = 2f;
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = new Vector2(0f, speed);
	}
	
	// Update is called once per frame
	void Update () {
		if(m_StopScrolling)
        {
            rb2d.velocity = Vector2.zero;
        }
        else
        {
            rb2d.velocity = new Vector2(0f, speed);
        }
	}
}
