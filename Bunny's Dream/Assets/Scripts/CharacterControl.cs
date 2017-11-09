using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CarrotBoosters))]
public class CharacterControl : MonoBehaviour {

    [HideInInspector] public bool facingRight = true;
    [HideInInspector] public bool jump = true;
    public List<CarrotBoosters.Booster> boosters;
    public int keyForBooster;
    public bool boostActive;
    public string activeBoostName;
    public float moveForce = 365f;
    public float maxSpeed = 5f;
    public float jumpForce = 1000f;
    public Transform groundCheck;

    private EnemyScript enemyScript;
    private bool grounded = false;
    public bool enemyBelow;
    private Animator anim;
    private Rigidbody2D rb2d;
    // Use this for initialization

    private void Awake()
    {
        boosters = CarrotBoosters.getBoosters();
        boostActive = false;
        enemyBelow = false;
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");

        anim.SetFloat("Speed", Mathf.Abs(h));

        if (h * rb2d.velocity.x < maxSpeed)
        {
            rb2d.AddForce(Vector2.right * h * moveForce);
        }

        if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
        {
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);
        }

        if (h > 0 && !facingRight)
        {
            Flip();
        }
        else if (h < 0 && facingRight)
        {
            Flip();
        }

        if (jump && grounded)
        {
            anim.SetTrigger("Jump");
            rb2d.AddForce(new Vector2(0f, jumpForce));
            jump = false;
        }

        if (activeBoostName == "VerticalBoost" && !boostActive)
        {
            boostStatus();
            if (boostActive)
            {
                rb2d.AddForce(new Vector2(0f, 1200f));
                activeBoostName = "";
                boostStatus();
            }
        }
    }

    public void boostStatus()
    {
        boostActive = !boostActive;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        if (Input.GetButtonDown("Jump") && grounded)
        {
            jump = true;
        }
        Debug.Log(enemyBelow);
    }

    //Hahmo käännetään toiseen suuntaan aina käännyttäessä
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject);
        if(collision.name.Contains("Carrot"))
        {
            keyForBooster = Random.Range(0, boosters.Count - 1);
            boosters[keyForBooster].setBoostActive();
            activeBoostName = boosters[keyForBooster].getName();
            Destroy(collision.gameObject);
        }

        if (collision.name.Contains("Enemy") && !collision.gameObject.Equals("groundCollider"))
        {
            Debug.Log(collision.name);
            enemyScript = collision.gameObject.GetComponent<EnemyScript>();
            enemyScript.EnemyDead(collision.gameObject);
        }
    }
}