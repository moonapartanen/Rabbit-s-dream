using UnityEngine;
using UnityEngine.UI;

/* Destroys every object it collides with, while keeping up if the player progresses faster than destroyer.
 * Destroyer speed is basically the difficulty level of the game at this stage.
 * Could be stopped using stopScrolling to implement pausing, speed of the destroyer is the only thing 
 * forcing the player to progress in the game.
*/

public class DestroyerScript : MonoBehaviour {
    private Rigidbody2D destroyer;
    private GameObject player;
    private SpawnPlatforms spawnerScript;

    private int destroyCount = 0;
    private float destroyerSpeed = 1f;
    private float speedIncrease = 0.2f;
    private float maxDistanceFromPlayer = 15f;
    private bool stopScrolling = false;

    void Start() {
        spawnerScript = GameObject.FindGameObjectWithTag("PlatformGenerator").GetComponent<SpawnPlatforms>();
        player = GameObject.FindGameObjectWithTag("Player");
        destroyer = GetComponent<Rigidbody2D>();
        destroyer.velocity = new Vector2(0.0f, destroyerSpeed);
    }

    void Update() {
        if (stopScrolling)
            destroyer.velocity = Vector2.zero;
        else if (destroyer.velocity.y == 0f)
            destroyer.velocity = new Vector2(0f, destroyerSpeed);

        if(player != null)
            if (player.transform.position.y - maxDistanceFromPlayer > transform.position.y)
                transform.position = new Vector2(0f, player.transform.position.y - maxDistanceFromPlayer);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            stopScrolling = true;
        } else if (other.gameObject.tag == "ground") {
            destroyCount++;
            spawnerScript.platformsActive--;

            if (destroyCount % 10 == 0)
                destroyer.velocity = new Vector2(0f, destroyer.velocity.y + speedIncrease);
        }

        Destroy(other.gameObject);
    }
}
