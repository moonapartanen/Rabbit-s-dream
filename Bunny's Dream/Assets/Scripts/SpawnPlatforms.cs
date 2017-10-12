using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatforms : MonoBehaviour {

    
    //Array eri prefabeille, joista arvotaan myöhemmin valittava taso LARGE, MEDIUM, SMALL
    public GameObject[] obj;
    //ID joka kertoo mistä spawnerista on kyse
    public int spawnerID;

    //Tason luontia varten
    private Vector2 platformLocation;
    //Jokaisella spawnerilla on maximissaan tämän verran tasoja
    private const int MAX_PLATFORMS = 10;
    private float m_spawnTime = 2f;
    private float min_X = -3f;
    private float max_X = 3f;
    private float min_Y = 4f;
    private float max_Y = 8f;

    //Lista johon tasot jotka on luotu lisätään, käytetään tuhoamaan oikeat tasot, jolloin spawneri voi taas alkaa luomaan tasoja, kun se on alle MAX_PLATFORMS eli 10.
    public List<GameObject> spawned;
    private int spawnCounter;
	// Use this for initialization
	void Start () {
        spawnCounter = 0;
        platformLocation = transform.position;
        Spawn();
	}

    private void Update()
    {
        //GroundSpawnerin nykyinen lokaatio, jonka kohdalle taso luodaan
        platformLocation = transform.position;
    }

    void Spawn()
    {
        //Luodaan tasoja vain jos listassa on tilaa (n < MAX_PLATFORMS)
         
        if (spawned.Count < MAX_PLATFORMS)
        {
            GameObject newPlatform = Instantiate(obj[Random.Range(0, obj.Length)], platformLocation, Quaternion.identity);
            newPlatform.name = "Spawner " + spawnerID + " Platform Number: " + spawnCounter;
            spawned.Add(newPlatform);
            spawnCounter++;
            transform.position = RandomLocationXandY();
        }
        Invoke("Spawn", m_spawnTime * Time.deltaTime);
    }

    /* Tällä hetkellä tasojen luonti on melko random, joten tasot ovat miten sattuu (varsinkin mitä korkeammalle mennään).
     * randomit täytyy Clampata(Mathf.Clamp) niin, että ne ei pysty menemään tiettyjen arvojen yli.
     * Tässä täytyisi verrata alkuperäistä spawnerin lokaatiota niin että minimi ja maksimi ei voi alittaa/ylittää tiettyjä arvoja, jolloin ne pysyisivät "Omalla" puolella. 
     * Lisäksi pitäisi luoda jonkunlainen testi, että ainakin yhden spawnerin luoma taso olisi mahdollista saavuttaa
     * TÄRKEÄ!!!
    */

   Vector2 RandomLocationXandY()
    {
        Vector2 location = new Vector2(Random.Range(platformLocation.x + min_X, platformLocation.x + max_X), Random.Range(platformLocation.y + min_Y, platformLocation.y + max_Y));
        return location;
    }
}
