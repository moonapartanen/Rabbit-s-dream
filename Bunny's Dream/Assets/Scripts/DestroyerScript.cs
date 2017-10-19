using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DestroyerScript : MonoBehaviour {

    //Variables for the destroyer, Rigidbody2D and speed it moves
    private Rigidbody2D rb2d;
    private float speed = 1.2f;

    //Array for the spawners (GroundSpawn-Objects) and Lists for different spawners which get the references to each spawners own list (These include the spawned objects)
    public GameObject[] generators;
    private List<GameObject> firstSpawner;
    private List<GameObject> secondSpawner;
    private List<GameObject> thirdSpawner;

    //Can be used to stop the destroyer while game is paused, for example
    [SerializeField] private bool m_StopScrolling;

    void Start()
    {
        //Get all objects that have the Tag "PlatformGenerator" (GroundSpawn)
        generators = GameObject.FindGameObjectsWithTag("PlatformGenerator");

        //Get the body of the destroyer and set the speed it moves (vertically)
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = new Vector2(0f, speed);
        //Get the references to the Lists, each spawner has its own ID, Compare to find the right ones
        foreach(GameObject spwn in generators)
        {
            if(spwn.GetComponent<SpawnPlatforms>().spawnerID == 1)
            {
                firstSpawner = spwn.GetComponent<SpawnPlatforms>().spawned;
            } else if (spwn.GetComponent<SpawnPlatforms>().spawnerID == 2)
            {
                secondSpawner = spwn.GetComponent<SpawnPlatforms>().spawned;
            } else
            {
                thirdSpawner = spwn.GetComponent<SpawnPlatforms>().spawned;
            }
        }
    }

    void Update()
    {
        if (m_StopScrolling)
        {
            rb2d.velocity = Vector2.zero;
        }
        else
        {
            rb2d.velocity = new Vector2(0f, speed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            //If collision with the player, pause the game for now before proper end game screen has been made
            Debug.Break();
        }
        //Compare with spawners object was hit, platforms that have been spawned have name in this format: "Spawner " + spawnerID(int) + " Platform Number: " + spawnCounter(int)" 
        //For further reference if needed
        //Delete object from the spawner list and destroy gameobject from game. 
        //If after deletion list has room (n < MAX_PLATFORMS), spawners will continue to a new cycle
        if (collision.name.Contains("Spawner 1"))
        {
            firstSpawner.Remove(collision.gameObject);
            Destroy(collision.gameObject);
        }
        else if(collision.name.Contains("Spawner 2"))
        {
            secondSpawner.Remove(collision.gameObject);
            Destroy(collision.gameObject);
        }
        else if(collision.name.Contains("Spawner 3"))
        {
            thirdSpawner.Remove(collision.gameObject);
            Destroy(collision.gameObject);
        }
        else
        {
            //Destroy anything else it hits (In the future it will be enemies.
            Destroy(collision.gameObject);
        }
    }
}
