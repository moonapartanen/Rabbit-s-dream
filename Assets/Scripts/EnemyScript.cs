using System.Collections;
using UnityEngine;

public class EnemyScript : MonoBehaviour, ICustomMessageSystem {
    public BoxCollider2D platformCollider;
    public BoxCollider2D enemyBodyCollider;
    public BoxCollider2D enemyKillCollider;
    private Rigidbody2D rb2dEnemy;
    private GameObject parentPlatform;
    public Vector2 endPoint, startPoint;
    public bool alive = true, facingRight = false;
    public static bool playerShieldActive = false;
    private float duration = 4f;
    private float currentLerpTime, startTime, rateToBeChecked;

    public AudioClip enemyDead;
    private AudioSource source;

	IEnumerator Start () {
        source = GetComponent<AudioSource>();
        enemyBodyCollider = transform.GetComponentInChildren<BoxCollider2D>();
        enemyKillCollider = transform.Find("ketunroppa").GetComponent<BoxCollider2D>();
        //Get the collider in the cloud and get the bounds
        platformCollider = transform.parent.GetComponent<BoxCollider2D>();
        endPoint = platformCollider.bounds.max;
        startPoint = platformCollider.bounds.min;
        //Raising the startPoint to the same level as endPoint
        startPoint.y = endPoint.y = transform.parent.position.y + 2f;
        currentLerpTime = Time.deltaTime;

        while(true) {
            yield return StartCoroutine(MoveEnemy(startPoint, endPoint, duration));
            yield return StartCoroutine(MoveEnemy(endPoint, startPoint, duration));
        }
	}

	// Update is called once per frame
	void Update () {
        currentLerpTime += Time.deltaTime;

        if(rateToBeChecked > 1) {
            currentLerpTime = Time.deltaTime;
        }

        if(!alive) {
            StopAllCoroutines();
        }
    }

    IEnumerator MoveEnemy(Vector2 start, Vector2 end, float time) {
        FlipEnemy();
        float rate = rateToBeChecked = 0f;

        while (rate < 1) {
            rate = currentLerpTime / time;
            rateToBeChecked = rate;
            transform.position = Vector2.Lerp(start, end, rate);

            yield return null;
        }
    }

    void FixedUpdate() {
        if(!alive && rb2dEnemy) {
            //Resize the enemy and make it fall to the destroyer
            Vector3 size = rb2dEnemy.transform.localScale;
            size.y = .2f;
            rb2dEnemy.transform.localScale = size;
            Collider2D[] collidersInObject = new Collider2D[rb2dEnemy.attachedColliderCount];
            rb2dEnemy.GetAttachedColliders(collidersInObject);

            foreach(Collider2D collider in collidersInObject) {
                collider.enabled = false;
            }

            rb2dEnemy.isKinematic = false;
        }
    }

    void FlipEnemy() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void EnemyDead(GameObject enemy) {
        rb2dEnemy = enemy.GetComponent<Rigidbody2D>();
        alive = false;
        source.PlayOneShot(enemyDead);
    }

    void KillEnemy(GameObject collision) {
        EnemyDead(collision.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "Player") {
            if (collision.otherCollider.name == "ketunroppa") {
                if (playerShieldActive) {
                    KillEnemy(collision.otherRigidbody.gameObject);
                } else {
                    GameObject player = collision.gameObject;
                    player.transform.localScale = new Vector3(1.2f, 0.5f, 1.2f);
                    player.GetComponent<Rigidbody2D>().isKinematic = true;
                    player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);
                } 
            } else if (collision.otherCollider.name.Contains("Enemy")) {
                KillEnemy(collision.otherCollider.gameObject);
            }
        }
    }

    public void BoostActivatedOnHero() {
        playerShieldActive = true;
        Debug.Log("Hero: Shield activated, turned on shieldActive in enemy");
    }

    public void BoostRemovedFromHero() {
        playerShieldActive = false;
        Debug.Log("Removed shield from hero");
    }
}
