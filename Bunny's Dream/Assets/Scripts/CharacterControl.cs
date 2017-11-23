using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CharacterControl : MonoBehaviour, ICustomMessageSystem {

    [HideInInspector] public bool facingRight = true;
    [HideInInspector] public bool jump = true;
    [HideInInspector] public bool messageSent = false;
    public List<CarrotBoosters.Booster> boosters;
    public int keyForBooster;
    public bool boostActivated, boostDone;
    public string activeBoostName;
    private float moveForce = 365f;
    private float maxSpeed = 5f;
    private float jumpForce = 1000f;
    private Transform groundCheck;
    public CarrotBoosters carrotBoostersScript;
    private bool grounded = false;
    private Animator anim;
    private Rigidbody2D rb2d;

    public AudioClip jumpSound;

    private AudioSource source;
    // Use this for initialization

    private void Awake()
    {
        source = GetComponent<AudioSource>();

        if (!carrotBoostersScript) {
            GameObject carrot = (GameObject)Resources.Load("Carrot");
            carrotBoostersScript = (CarrotBoosters)carrot.GetComponent<CarrotBoosters>();
        }
        
        if(!groundCheck)
        {
            GameObject groundC = GameObject.Find("groundCheck");
            groundCheck = (Transform)groundC.transform;
        }

        boosters = CarrotBoosters.getBoosters();
        boostActivated = false;
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

        if (boostActivated)
        {
            carrotBoostersScript.CheckBoostName(boosters[keyForBooster]);
        }
    }

    public void boostStatus()
    {
        boostActivated = !boostActivated;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        if (Input.GetButtonDown("Jump") && grounded)
        {
            Debug.Log("Hyppäsin");
            source.PlayOneShot(jumpSound);
            jump = true;
        }
        
        if(activeBoostName == "Shield" && !messageSent)
        {
            ExecuteEvents.Execute<ICustomMessageSystem>(GameObject.FindGameObjectWithTag("Enemy"), null, (x, y) => x.BoostActivatedOnHero());
            messageSent = true;
        }
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
        if (collision.gameObject.name.Contains("Carrot"))
        {
            boostStatus();
            keyForBooster = Random.Range(0, boosters.Count - 1);
            boosters[keyForBooster].setBoostActive();
            activeBoostName = boosters[keyForBooster].getName();
            Destroy(collision.gameObject);
        }
    }

    public void BoostActivatedOnHero()
    {
        Debug.Log("Hero: Shield activated by hero");
    }

    public void BoostRemovedFromHero()
    {
        Debug.Log("Shield removed");
        boostStatus();
        activeBoostName = "";
        messageSent = !messageSent;
        ExecuteEvents.Execute<ICustomMessageSystem>(GameObject.FindGameObjectWithTag("Enemy"), null, (x, y) => x.BoostRemovedFromHero());
    }
}