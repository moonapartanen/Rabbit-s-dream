using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script is handling groundspawns for the game, there are 3 spawners that are doing the spawning. This is an infinite 2D Platformer (Vertical). 
 * First spawner does the randomization and all spawners use the same randomPlatformNumber to find 
 * proper platform heights given in Platforms class that are put on Dictionary declared in function getPremadePlatforms. 
 * The randomPlatformNumber is the key to access these values that are added on top of the highest
 * platform value after each cycle. This is to ensure that each spawn is above each other.
*/

public class SpawnPlatforms : MonoBehaviour
{

    // Holds premade platform configurations set inside Platforms class that get used to randomize the spawned platforms. This is to ensure that there is always a platform that you can jump on. 
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
    //This will be random number between 1-6 cycles
    private static int numberOfEnemySpawns;
    //Starting value will be 5, randomized between 4-8
    private static int enemyRandomization = 5;
    public static bool spawnEnemies = false;
    public static int enemyPlatformID = 0;
    //used to count the cycles we have spawned enemies
    private static int enemyCounter = 0;
    //Starting value will be 3, it will be randomized between 3-8 after the first carrot spawn
    private static int carrotRandomization = 3;
    //max number of platforms alive for each spawner
    private int MAX_PLATFORMS = 20;
    //speed variable for spawning platforms
    private float m_spawnTime = 2f;
    //Variable for DestroyerScript-script
    public static DestroyerScript destroyerScript;
    public float destroyerSpeedIncrease;
    public static bool speedAlreadyIncreased = true;
    //List that holds all the objects, that get spawned so we can access them later to destroy them or get values (count etc).
    public List<GameObject> spawned;

    //Counter for spawner
    private static int spawnCounter = 1;
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
        if(destroyerSpeedIncrease == 0)
            destroyerSpeedIncrease += 0.2f;

        randomPlatformNumber = 0;
        platformLocations = GetPremadePlatforms();
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

        //Get the highest platform in previous cycle, check if we need to spawn carrots (Boosts, no functionality yet), Randomize which platform has the carrot. Increase speed for camera & Destroyer
        if (spawnersInCycle.Count == 3)
        {
            spawnCounter++;

            if(spawnEnemies)
            {
                enemyPlatformID = Random.Range(1, 3);
            }

            //Adds a bit of randomization for the carrot spawns, so its not set in stone when they spawn
            if (spawnCounter % carrotRandomization == 0 && !spawnCarrot)
            {
                carrotRandomization = spawnCounter + RandomizationForCarrots();
                spawnCarrot = true;
                carrotSpawnerId = Random.Range(1, 3);
            }

            //Adds a bit of randomization for the enemy spawns, when they start to spawn
            if (spawnCounter % enemyRandomization == 0 && !spawnEnemies)
            {
                spawnEnemies = true;
                enemyRandomization = spawnCounter + RandomizationForEnemies();
                numberOfEnemySpawns = RumberOfEnemiesToSpawn();
            }

            if (spawnCounter % 5 == 0)
            {
                speedAlreadyIncreased = (speedAlreadyIncreased) ? false : true;
                //Only spawner 1 increases the speed, otherwise there would be 3 increases per cycle
                if (spawnerID == 1)
                {
                    if (!speedAlreadyIncreased)
                    {
                        if (spawnCounter % 15 == 0)
                            destroyerSpeedIncrease += 0.1f;

                        destroyerScript.IncreaseSpeedForDestroyer(destroyerSpeedIncrease);
                        boolIncreasedDestroyerSpeed();
                    }
                }
            }
            spawnersInCycle = new List<int>();
            highest = HighestPlatform();
            randomPlatformNumber = GameObject.Find("GroundSpawn 1").GetComponent<SpawnPlatforms>().RandomNumber();
        }
    }

    void boolIncreasedDestroyerSpeed()
    {
        speedAlreadyIncreased = !speedAlreadyIncreased;
    }

    public int RumberOfEnemiesToSpawn()
    {
        return Random.Range(1, 6);
    }

    public int RandomizationForCarrots()
    {
        return Random.Range(3, 8);
    }

    public int RandomizationForEnemies()
    {
        return Random.Range(4, 8);
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

                if(spawnEnemies && this.spawnerID == enemyPlatformID)
                {
                    GameObject enemy = (GameObject)Instantiate(Resources.Load("Enemy"), new Vector2(platformLocation.x, platformLocation.y + 3f), Quaternion.identity);
                    enemyCounter++;
                    enemy.name = "Enemy: " + enemyCounter;
                    enemy.transform.parent = newPlatform.transform;

                    if(enemyCounter == numberOfEnemySpawns)
                    {
                        spawnEnemies = false;
                        enemyCounter = 0;
                    }
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
    private float HighestPlatform()
    {
        GameObject spawnerLocation;
        if (platformLocations[randomPlatformNumber].GetHighest() == 1)
        {
            spawnerLocation = GameObject.Find("GroundSpawn 1").gameObject;
        }
        else if (platformLocations[randomPlatformNumber].GetHighest() == 2)
        {
            spawnerLocation = GameObject.Find("GroundSpawn 2").gameObject;
        }
        else if (platformLocations[randomPlatformNumber].GetHighest() == 3)
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
    int RandomNumber()
    {
        int arvottu = 0;
        do
        {
            arvottu = Random.Range(0, platformLocations.Count - 1);
        } while (arvottu == randomPlatformNumber);
        return arvottu;
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
        public int GetHighest()
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

    public Dictionary<int, Platforms> GetPremadePlatforms()
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