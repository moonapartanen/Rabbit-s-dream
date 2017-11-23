using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyScript : MonoBehaviour, ICustomMessageSystem {
    public BoxCollider2D mPlatformCollider;
    public BoxCollider2D mEnemyBodyCollider;
    public BoxCollider2D mEnemyKillCollider;
    private Rigidbody2D rb2dEnemy;
    private GameObject parentPlatform;
    public Vector2 endPoint, startPoint;
    public bool EnemyAlive = true, facingRight = false;
    public static bool playerShieldActive = false;
    private float duration = 4f;
    private float currentLerpTime, startTime, rateToBeChecked;

    public AudioClip enemyDead;
    private AudioSource source;
	// Use this for initialization
	IEnumerator Start () {
        source = GetComponent<AudioSource>();
        mEnemyBodyCollider = transform.GetComponentInChildren<BoxCollider2D>();
        mEnemyKillCollider = transform.FindChild("ketunroppa").GetComponent<BoxCollider2D>();
        //Get the collider in the cloud and get the bounds
        mPlatformCollider = this.transform.parent.GetComponent<BoxCollider2D>();
        endPoint = mPlatformCollider.bounds.max;
        startPoint = mPlatformCollider.bounds.min;
        //Raising the startPoint to the same level as endPoint
        startPoint.y = endPoint.y = this.transform.parent.position.y + 2f;

        currentLerpTime = Time.deltaTime;
        while(true)
        {
            yield return StartCoroutine(MoveEnemy(startPoint, endPoint, duration));
            yield return StartCoroutine(MoveEnemy(endPoint, startPoint, duration));
        }
	}

	// Update is called once per frame
	void Update () {
        currentLerpTime += Time.deltaTime;

        if(rateToBeChecked > 1)
        {
            currentLerpTime = Time.deltaTime;
        }

        if(!EnemyAlive)
        {
            StopAllCoroutines();
        }
    }

    IEnumerator MoveEnemy(Vector2 start, Vector2 end, float time)
    {
        FlipEnemy();
        float rate = rateToBeChecked = 0f;
        while (rate < 1)
        {
            rate = currentLerpTime / time;
            rateToBeChecked = rate;
            transform.position = Vector2.Lerp(start, end, rate);
            yield return null;
        }
    }

    private void FixedUpdate()
    {
        if(!EnemyAlive && rb2dEnemy)
        {
            //Resize the enemy and make it fall to the destroyer
            Vector3 size = rb2dEnemy.transform.localScale;
            size.y = .2f;
            rb2dEnemy.transform.localScale = size;
            Collider2D[] collidersInObject = new Collider2D[rb2dEnemy.attachedColliderCount];
            rb2dEnemy.GetAttachedColliders(collidersInObject);
            foreach(Collider2D collider in collidersInObject)
            {
                collider.enabled = false;
            }

            rb2dEnemy.isKinematic = false;
        }
    }

    public void FlipEnemy()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void EnemyDead(GameObject enemy)
    {
        rb2dEnemy = enemy.GetComponent<Rigidbody2D>();
        this.EnemyAlive = false;
        source.PlayOneShot(enemyDead);
    }

    public void KillEnemy(GameObject collision)
    {
        EnemyDead(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "hero")
        {
            if (collision.otherCollider.name == "ketunroppa")
            {
                if (playerShieldActive)
                    KillEnemy(collision.otherRigidbody.gameObject);
                else
                {
                    Debug.Log("You died");
                    Debug.Break();
                }
            } else if(collision.otherCollider.name.Contains("Enemy"))
            {
                KillEnemy(collision.otherCollider.gameObject);
            }
        }
    }

    public void BoostActivatedOnHero()
    {
        playerShieldActive = true;
        Debug.Log("Hero: Shield activated, turned on shieldActive in enemy");
        Debug.Log(playerShieldActive);
    }

    public void BoostRemovedFromHero()
    {
        playerShieldActive = false;
        Debug.Log("Removed shield from hero");
    }
}
