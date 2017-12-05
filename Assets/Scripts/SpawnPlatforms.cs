using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatforms : MonoBehaviour {
    public GameObject[] platformPrefabs;
    private List<LocationSet> platformLocationSets;
    private LocationSet currentLocationSet;
    private float highestPlatformY = 0.0f;

    private bool spawnCarrot = false;
    private bool spawnEnemies = false;

    private int enemyRandomization = 5;
    private int carrotRandomization = 3;

    private int enemiesToSpawn = 0;
    private int enemyCounter = 0;

    private int maxPlatforms = 20;
    public int platformsActive = 0;

    private int spawnRoundInterval = 2;
    private int spawnRoundCount = 1;

    void Start() {
        InitializePlatformLocations();
        currentLocationSet = platformLocationSets[0];

        Spawn();
    }

    private void Update() {
        //Adds a bit of randomization for the carrot spawns, so its not set in stone when they spawn
        if (spawnRoundCount % carrotRandomization == 0 && !spawnCarrot) {
            spawnCarrot = true;
            carrotRandomization = spawnRoundCount + Random.Range(3, 8);
        }

        //Adds a bit of randomization for the enemy spawns, when they start to spawn
        if (spawnRoundCount % enemyRandomization == 0 && !spawnEnemies) {
            spawnEnemies = true;
            enemyRandomization = spawnRoundCount + Random.Range(4, 8);
            enemiesToSpawn = Random.Range(1, 3);
        }
    }

    /* Spawning is done in rounds in which a set of 3 platforms is spawned to locations which are semi-random.
     * Platform prefab is randomized from 3 possibilities (small, medium, large), then the platform is created.
     * Depending on certain variables, enemies and carrots can be spawned on top of each platform.
     * When all 3 platforms have been created, highest platform location is updated and the next platform
     * location set for the next spawning round is set.
    */
    void Spawn() {
        if (platformsActive < maxPlatforms) {
            for(int i = 0; i < 3; i++) {
                GameObject platformPrefab = platformPrefabs[Random.Range(0, platformPrefabs.Length)];
                Vector2 platformPosition = new Vector2(currentLocationSet[i].x,
                    currentLocationSet[i].y + highestPlatformY);

                GameObject platform = Instantiate(platformPrefab, platformPosition, Quaternion.identity);

                platform.name = "Round: " + spawnRoundCount + ", Platform: " + (i + 1);

                if (spawnCarrot) {
                    GameObject carrot = Instantiate((GameObject)Resources.Load("Carrot"),
                        new Vector2(platform.transform.position.x, platform.transform.position.y + 1f),
                        Quaternion.identity);

                    carrot.transform.parent = platform.transform;
                    spawnCarrot = false;
                }

                if (spawnEnemies) {
                    GameObject enemy = Instantiate((GameObject)Resources.Load("Enemy"),
                        new Vector2(platform.transform.position.x, platform.transform.position.y + 3f),
                        Quaternion.identity);

                    enemy.transform.parent = platform.transform;

                    enemyCounter++;
                    enemy.name = "Enemy: " + enemyCounter;

                    if (enemyCounter == enemiesToSpawn) {
                        spawnEnemies = false;
                        enemyCounter = 0;
                    }
                }

                platformsActive++;
            }

            highestPlatformY = currentLocationSet.Highest.y + highestPlatformY;

            spawnRoundCount++;
            SetNextPlatformLocations();
        }

        Invoke("Spawn", spawnRoundInterval);
    }

    // Picks a new platform location set for the next round of spawning
    private void SetNextPlatformLocations() {
        LocationSet set = currentLocationSet;

        while(set == currentLocationSet) {
            set = platformLocationSets[Random.Range(0, platformLocationSets.Count)];
        }

        currentLocationSet = set;
    }

    /* Represents a set of platform locations which are used in each spawning round
     * X-coords are fixed (10f, 0f, -10f), y-coords are set during initialization
    */
    private class LocationSet {
        private Vector2[] set;

        public LocationSet(float y1, float y2, float y3) {
            set = new Vector2[] {
                new Vector2(-10f, y1),
                new Vector2(0f, y2),
                new Vector2(10f, y3)
            };
        }

        public Vector2 this[int index] {
            get {
                return set[index];
            }
        }

        public Vector2 Highest {
            get {
                Vector2 highest = new Vector2(0.0f, 0.0f);

                foreach (Vector2 position in set)
                    highest = position.y > highest.y ? position : highest;

                return highest;
            }
        }
    }

    /* Sets of locations to choose from in every iteration of platform spawning
     * (TODO): Actual randomization of locations
    */
    public void InitializePlatformLocations() {
        platformLocationSets = new List<LocationSet> {
            new LocationSet(0f, 4f, 8f),
            new LocationSet(6f, 10f, 15f),
            new LocationSet(6f, 10f, 5f),
            new LocationSet(7f, 5f, 7f),
            new LocationSet(15f, 10f, 6f),
            new LocationSet(8f, 5f, 5f)
        };
    }
}
 