﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script is handling groundspawns for the game, there are 3 spawners that are doing the spawning. This is an infinite scroller (Vertical). First spawner does the randomization and all spawners use the same randomPlatformNumber to find 
 * proper platform heights given in Platforms class that are put on Dictionary declared in function getPremadePlatforms. 
 * The randomPlatformNumber is the key to access these values that are added on top of the highest
 * platform value after each cycle. This is to ensure that each spawn is above each other.
 * EACH CYCLE:
 * - Check that this spawner has less than MAX_PLATFORMS (20) platforms alive, if so cycle continues. 
 * (DestroyerScript handles the Destroyer that terminates all platforms that hit it and removes them from the spawned(List)), making this infinite scroller. 
 * - GET randomPlatformNumber (static) so its always the same with each spawner and only one spawner does the randomization (GroundSpawn 1), 
 * randomization is done for as long as needed to find a value that is != value it was in previous cycle.
 * 
 *  - platformLocations[randomPlatformNumber].platformOneY (GroundSpawn 1) and so on, is the height for GroundSpawn to be added on top of the highest platform value that we get after each cycle to get
 *  proper spacing for the platforms. x-coordinates are staying the same for now, so the groundspawns will never overlap with each other.
 *  - add the spawned gameobject to spawned(List)
 *
 * List to do:
 * - Moving platforms after certain point (Randomized which of the platforms is moving, this can be vertical or horizontal depending on the specific spawn configuration)
 * 
 * 
 * Made by Eetu Leivo
     */

public class SpawnPlatforms : MonoBehaviour
{

    /* Holds premade platform configurations set inside Platforms class that get used to randomize the spawned platforms. This is to ensure that there is always a platform that you can jump on
     * Difficulty will be based on the speed of the camera (Changing after enough spawns have passed / certain height is reached), moving platforms and enemies. 
     */
    public Dictionary<int, Platforms> platformLocations;
    //List for checking if the spawner has spawned in a cycle or not, when this gets to Count 3, 
    //it gets remade and cycle continues, so we can safely update highest-variable 
    public static List<int> spawnersInCycle = new List<int>();
    // This is the variable to hold the highest value for each cycle
    private static float highest = -0.1f;
    //Array for prefabs that hold the grounds that get spawned (LARGE, MEDIUM, SMALL)
    public GameObject[] obj;
    //ID for the spawner, needs to be public so we can access the value inside DestroyerScript.
    public int spawnerID;

    public static bool spawnCarrot = false;
    public static int carrotSpawnerId = 0;
    //location for the spawner
    private Vector2 platformLocation;

    private static int randomPlatformNumber;
    private static int spawnsInCycle = -1;
    //max number of platforms alive for each spawner
    private int MAX_PLATFORMS = 20;
    //speed variable for spawning platforms
    private float m_spawnTime = 2f;
    //Variable for DestroyerScript-script
    public static DestroyerScript destroyerScript;
    public float DestroyerSpeedIncrease;
    public static bool speedAlreadyIncreased = true;
    //List that holds all the objects, that get spawned so we can access them later to destroy them or get values (count etc).
    public List<GameObject> spawned;

    //Counter for spawner
    private static int spawnCounter = 1;
    private static int enemyCounter = 0;

    // Use this for initialization
    void Start()
    {

        //If not set, get the DestroyerScript from the Destroyer GameObject
        if (destroyerScript == null)
        {
            GameObject destroyer = GameObject.Find("Destroyer");
            destroyerScript = (DestroyerScript)destroyer.GetComponent(typeof(DestroyerScript));
        }

        //If not set, variable starts at 0, increase it by marginal value like 0.2f (Starting value)
        if(DestroyerSpeedIncrease == 0)
            DestroyerSpeedIncrease += 0.2f;

        randomPlatformNumber = 0;
        platformLocations = getPremadePlatforms();
        platformLocation = transform.position;
        if (this.name == "GroundSpawn 1")
        {
            this.spawnerID = 1;
        }
        else if (this.name == "GroundSpawn 2")
        {
            this.spawnerID = 2;
        }
        else
        {
            this.spawnerID = 3;
        }
        Spawn();
    }

    private void Update()
    {
        //Update groundspawner location
        platformLocation = transform.position;
    }

    private void LateUpdate()
    {
        //Get the highest platform in previous cycle, check if we need to spawn carrots (Boosts, no functionality yet), Randomize which platform has the carrot. Increase speed for camera & Destroyer
        if (spawnersInCycle.Count == 3)
        {
            spawnCounter++;

            if (spawnCounter % 3 == 0 && !spawnCarrot)
            {
                spawnCarrot = true;
                carrotSpawnerId = Random.Range(1, 3);
            }

            if (spawnCounter % 5 == 0)
            {
                speedAlreadyIncreased = (speedAlreadyIncreased) ? false : true;
                //Only spawner 1 increases the speed, otherwise there would be 3 increases per cycle
                if (spawnerID == 1)
                {
                    if (!speedAlreadyIncreased)
                    {
                        destroyerScript.IncreaseSpeedForDestroyer(DestroyerSpeedIncrease);
                        boolIncreasedCameraSpeed();
                    }
                }
            }
            spawnersInCycle = new List<int>();
            highest = highestPlatform();
            randomPlatformNumber = GameObject.Find("GroundSpawn 1").GetComponent<SpawnPlatforms>().randomNumber();
        }

    }

    void boolIncreasedCameraSpeed()
    {
        speedAlreadyIncreased = !speedAlreadyIncreased;
    }

    void Spawn()
    {
        //Only spawn if (n < MAX_PLATFORMS)
        if (spawned.Count < MAX_PLATFORMS)
        {
            //Only spawn if the spawners ID cannot be found in the list, this list gets remade in lateUpdate every cycle when the Count gets to 3 (All spawners are inside the list)
            if (!spawnersInCycle.Contains(this.spawnerID))
            {
                spawnersInCycle.Add(this.spawnerID);
                GameObject newPlatform = Instantiate(obj[Random.Range(0, obj.Length)], platformLocation, Quaternion.identity);
                newPlatform.name = "Spawner " + spawnerID + " Platform Number: " + spawnCounter;

                if (spawnCarrot && this.spawnerID == carrotSpawnerId)
                {
                    GameObject carrot = (GameObject)Instantiate(Resources.Load("Carrot"), new Vector2(platformLocation.x, platformLocation.y + 1f), Quaternion.identity);
                    carrot.transform.parent = newPlatform.transform;
                    spawnCarrot = false;
                }

                if(spawnCounter % 4 == 0)
                {
                    GameObject enemy = (GameObject)Instantiate(Resources.Load("Enemy"), new Vector2(platformLocation.x, platformLocation.y + 3f), Quaternion.identity);
                    enemyCounter++;
                    enemy.name = "Enemy: " + enemyCounter;
                    enemy.transform.parent = newPlatform.transform;
                }
                spawned.Add(newPlatform);
                
                transform.position = PlatformCoords();
            }
        }
        Invoke("Spawn", m_spawnTime);
    }

    //This function checks the highest value in the dictionary to get the highest spawner in each cycle.
    //getHighest function is inside Platforms class, that checks which of the values is the highest returns (1-3, 0 if all spawners are on the same y-coordinate)
    //Using the randomPlatformNumber (This happens before a new randomPlatformNumber is set). Returns the value of the highest spawners y-coordinate (float)
    private float highestPlatform()
    {
        GameObject spawnerLocation;
        if (platformLocations[randomPlatformNumber].getHighest() == 1)
        {
            spawnerLocation = GameObject.Find("GroundSpawn 1").gameObject;
        }
        else if (platformLocations[randomPlatformNumber].getHighest() == 2)
        {
            spawnerLocation = GameObject.Find("GroundSpawn 2").gameObject;
        }
        else if (platformLocations[randomPlatformNumber].getHighest() == 3)
        {
            spawnerLocation = GameObject.Find("GroundSpawn 3").gameObject;
        }
        else
        {
            //Since all spawns are on the same level, it doesn't matter which spawner we use to get the highest y-value.
            spawnerLocation = GameObject.Find("GroundSpawn 1").gameObject;
        }

        return spawnerLocation.gameObject.transform.position.y;
    }

    //This function returns location for each spawner, location.x does not change, location.y is the HIGHEST of previous cycle + the float we get from the dictionary (platformLocations) for each spawner
    Vector2 PlatformCoords()
    {
        Vector2 location = new Vector2();
        if (this.spawnerID == 1)
        {
            location.x = transform.position.x;
            location.y = highest + platformLocations[randomPlatformNumber].platformOneY;
        }
        else if (this.spawnerID == 2)
        {
            location.x = transform.position.x;
            location.y = highest + platformLocations[randomPlatformNumber].platformTwoY;
        }
        else if (this.spawnerID == 3)
        {
            location.x = transform.position.x;
            location.y = highest + platformLocations[randomPlatformNumber].platformThreeY;
        }
        return location;
    }

    // This function randoms as long as needed to find another value that was previously in randomPlatformNumber so we get different platform setups each cycle
    int randomNumber()
    {
        int arvottu = 0;
        do
        {
            arvottu = Random.Range(0, platformLocations.Count - 1);
        } while (arvottu == randomPlatformNumber);
        return arvottu;
    }

    //Not required for now
    int getRandomNumber()
    {
        return randomPlatformNumber;
    }

    //Not required for now
    void setRandomNumber(int number)
    {
        randomPlatformNumber = number;
    }

    public class Platforms
    {
        public float platformOneY { get; private set; }
        public float platformTwoY { get; private set; }
        public float platformThreeY { get; private set; }
        public int numberOfPremadePlatform { get; private set; }

        public Platforms(int premadeNumber, float one, float two, float three)
        {
            platformOneY = one;
            platformTwoY = two;
            platformThreeY = three;
            numberOfPremadePlatform = premadeNumber;
        }
        //highest value in instance of this class
        public int getHighest()
        {
            int highest = 0;
            if (platformOneY > platformTwoY && platformOneY > platformThreeY)
            {
                highest = 1;
            }
            else if (platformTwoY > platformOneY && platformTwoY > platformThreeY)
            {
                highest = 2;
            }
            else if (platformThreeY > platformTwoY && platformThreeY > platformOneY)
            {
                highest = 3;
            }

            return highest;
        }
    }

    public Dictionary<int, Platforms> getPremadePlatforms()
    {
        Dictionary<int, Platforms> premades = new Dictionary<int, Platforms>();
        //Key to access the values (randomPlatformNumber)
        // Spawner 1.y coordinates, Spawner 2.y coordinates, Spawner 3.y coordinates (To be added above the HIGHEST value in previous cycle) 
        Platforms one = new Platforms(0, 6f, 10f, 15f);
        Platforms two = new Platforms(1, 6f, 10f, 5f);
        Platforms three = new Platforms(2, 6f, 6f, 6f);
        Platforms four = new Platforms(3, 7f, 5f, 7f);
        Platforms five = new Platforms(4, 15f, 10f, 6f);
        Platforms six = new Platforms(5, 8f, 5f, 5f);

        premades.Add(one.numberOfPremadePlatform, one);
        premades.Add(two.numberOfPremadePlatform, two);
        premades.Add(three.numberOfPremadePlatform, three);
        premades.Add(four.numberOfPremadePlatform, four);
        premades.Add(five.numberOfPremadePlatform, five);
        premades.Add(six.numberOfPremadePlatform, six);

        return premades;
    }
}