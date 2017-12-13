using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarrotBoosters : MonoBehaviour, ICustomMessageSystem {
    [HideInInspector] private GameObject shield = null;
    public static GameObject player;
    public static Rigidbody2D playerRb;
    private AudioSource source;
    public AudioClip jumpBooster;
    public static bool activateJumpBoost = false, shieldActive = false;

    private void OnEnable()
    {
        EventManager.StartListening("CarrotBeingDestroyed", OnDestroy);
    }

    private void OnDisable()
    {
        EventManager.StopListening("CarrotBeingDestroyed", OnDestroy);
    }

    private void Start(){
        source = GetComponent<AudioSource>();

        if (!player) {
            player = GameObject.FindGameObjectWithTag("Player");
            playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        }
    }

    private void FixedUpdate() {
        if (activateJumpBoost) {
            source.PlayOneShot(jumpBooster);
            playerRb.AddForce(new Vector2(0f, 1200f));
            activateJumpBoost = false;
            ExecuteEvents.Execute<ICustomMessageSystem>(player.gameObject, null, (x, y) => x.BoostRemovedFromHero());
        }
    }

    private void ActivateShield(Booster booster) {
        if (!shield && shieldActive) {
            shield = (GameObject)Instantiate(Resources.Load("Shield"), player.transform);
            shield.name = "Shield";
            shield.transform.parent = player.gameObject.transform;
            Invoke("RemoveShield", (System.Int32)(booster.Length));
        }
    }

    private void RemoveShield() {
        if (shield && shieldActive) {
            if (shield != null) {
                Destroy(shield);
                shield = null;
                shieldActive = false;
                ExecuteEvents.Execute<ICustomMessageSystem>(player.gameObject, null, (x, y) => x.BoostRemovedFromHero());
            }
        }
    }

    public void CheckBoostName(Booster booster) {
        switch(booster.Name) {
            case "VerticalBoost":
                activateJumpBoost = true;
                break;
            case "Shield":
                shieldActive = true;
                ActivateShield(booster);
                break;
        }
    }

    public void BoostActivatedOnHero() {
        Debug.Log("Here for system message");
    }

    public void BoostRemovedFromHero() {
        Debug.Log("Wowow?");
    }

    private void OnDestroy() {
        EventManager.StopListening("CarrotBeingDestroyed", OnDestroy);
        Debug.Log("Carrot destroyed!");
    }

    public static List<Booster> GetBoosters() {
        List<Booster> boosterList = new List<Booster>();

        Booster verticalBoost = new Booster("VerticalBoost", 0.0f);
        boosterList.Add(verticalBoost);

        Booster shieldBoost = new Booster("Shield", 8f);
        boosterList.Add(shieldBoost);
        boosterList.Add(shieldBoost);
        boosterList.Add(shieldBoost);
        boosterList.Add(shieldBoost);
        boosterList.Add(shieldBoost);

        return boosterList;
    }

    public class Booster {
        public string Name { get; set; }
        public bool Active { get; set; }
        public float Length { get; set; }

        public Booster(string name, float length) {
            Name = name;
            Length = length;
            Active = false;
        }
    }
}
