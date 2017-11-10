using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DestroyerScript : MonoBehaviour {

    //Destroyerin omat muuttujat
    private Rigidbody2D rb2d;
    public float speed = 1f;

    //taulukko generaattoreille (GroundSpawn-Objektit) ja spawnereille (Sisältävät niiden luomat tasot.)
    public GameObject[] generators;
    private List<GameObject> firstSpawner;
    private List<GameObject> secondSpawner;
    private List<GameObject> thirdSpawner;

    //Voi käyttää pysäyttämään destroyerin jos peli esim. laitetaan pauselle
    [SerializeField] private bool m_StopScrolling;

    void Start()
    {
        //Haetaan kaikki objektit jotka sisältävät tagin "PlatformGenerator" (GroundSpawn), joilla alustamme generators taulukon
        generators = GameObject.FindGameObjectsWithTag("PlatformGenerator");
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = new Vector2(0f, speed);
        //Otetaan alussa jokaisen spawnerin omat listat kiinni muuttujiin, joita käytämme tuhoamaan listasta objekteja. Jokaisella Groundspawnilla on oma ID, jota vertailemme.
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

    public void IncreaseSpeedForDestroyer(float increase)
    {
        speed += increase;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            //Gameover, pausettaa pelin kun pelaaja osuu Destroyeriin
            Debug.Break();
        }
        //Vertaillaan mikä Groundspawneri kyseessä, tasoille on annettu nimeksi "Spawner " + spawnerID + " Platform Number: " + spawnCounter" 
        //missä spawnerID on tason luojan ID ja spawnCounter merkkaa numeroa, joka vastaa tason luontinumeroa.
        //Poistetaan gameobjecti listasta ja tuhotaan se pelistä. 
        //Tämän jälkeen SpawnPlatforms jatkaa tasojen luontia harmaaseen tulevaisuuteen asti, kun listasta vapautuu tilaa (n < 10).
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
            //Destroyeri tuhoaa kaiken muun, mikä osuu siihen. Eli kaikki missä ei ole käytetty GroundSpawneria (Pelaaja, Eka platform, you name it.)
            Debug.Log(collision.gameObject.name);
            Destroy(collision.gameObject);
            
        }
    }
}
