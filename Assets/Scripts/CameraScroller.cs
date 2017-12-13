using UnityEngine;

/*
 * Camera is moved by either destroyer coming closer than minDistanceToDestroyer units from the camera center 
 * or by player reaching height greater than highestY.
*/

public class CameraScroller : MonoBehaviour {
    private GameObject player;
    private GameObject destroyer;
    private float minDistanceToDestroyer = 15.0f;
    private float highestY = 0.0f;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        destroyer = GameObject.Find("Destroyer");
    }

    private void LateUpdate() {
        if(player != null)
            if (player.transform.position.y > highestY) {
                transform.position = new Vector3(transform.position.x, player.transform.position.y,
                    transform.position.z);

                highestY = player.transform.position.y;
            }

        if (transform.position.y - destroyer.transform.position.y < minDistanceToDestroyer)
            transform.position = new Vector3(transform.position.x, destroyer.transform.position.y
                + minDistanceToDestroyer, transform.position.z);
    }
}
