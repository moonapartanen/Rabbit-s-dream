using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarrotBoosters : MonoBehaviour, ICustomMessageSystem {

    private AudioSource source;
    public AudioClip jumpBooster;

    private void OnEnable()
    {
        EventManager.StartListening("CarrotBeingDestroyed", OnDestroy);
    }

    private void OnDisable()
    {
        EventManager.StopListening("CarrotBeingDestroyed", OnDestroy);
    }

    [HideInInspector] private GameObject shieldParticle = null;
    public static Rigidbody2D playerRb2D;
    public static bool activateJumpBoost = false, shieldActive = false;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        if (!playerRb2D)
            playerRb2D = GameObject.Find("hero").GetComponent<Rigidbody2D>();
    }

    public class Booster
    {
        private string mBoosterName;
        private bool mBoosterUp = false;
        private float mLength;

        public Booster(string name)
        {
            mBoosterName = name;
        }

        public Booster(string name, float lasts)
        {
            mBoosterName = name;
            mLength = lasts;
        }

        public void SetBoostActive()
        {
            mBoosterUp = !mBoosterUp;
        }

        public float GetLength()
        {
            return mLength;
        }

        public string GetName()
        {
            return mBoosterName;
        }

        public bool Status()
        {
            return mBoosterUp;
        }
    }

    private void FixedUpdate()
    {
        if (activateJumpBoost)
        {
            Debug.Log("LENNÄ PERKELE");
            source.PlayOneShot(jumpBooster);
            playerRb2D.AddForce(new Vector2(0f, 1200f));
            activateJumpBoost = false;
            ExecuteEvents.Execute<ICustomMessageSystem>(playerRb2D.gameObject, null, (x, y) => x.BoostRemovedFromHero());
        }
    }

    private void ActivateShield(Booster booster)
    {
        if (!shieldParticle && shieldActive)
        {
            Debug.Log((System.Int32)(booster.GetLength()));
            shieldParticle = (GameObject)Instantiate(Resources.Load("ShieldParticle"), playerRb2D.transform);
            shieldParticle.name = "shieldParticle";
            shieldParticle.transform.parent = playerRb2D.gameObject.transform;
            Invoke("RemoveShield", (System.Int32)(booster.GetLength()));
        }
    }

    private void RemoveShield()
    {
        if (shieldParticle && shieldActive)
        {
            if (shieldParticle != null) {
                Destroy(shieldParticle);
                shieldParticle = null;
                shieldActive = false;
                ExecuteEvents.Execute<ICustomMessageSystem>(playerRb2D.gameObject, null, (x, y) => x.BoostRemovedFromHero());
            }
        }
    }

    public void CheckBoostName(Booster booster)
    {

        switch(booster.GetName())
        {
            case "VerticalBoost":
                activateJumpBoost = true;
                break;
            case "Shield":
                shieldActive = true;
                ActivateShield(booster);
                break;
        }
    }

    public void BoostActivatedOnHero()
    {
        Debug.Log("Here for system message");
    }

    public void BoostRemovedFromHero()
    {
        Debug.Log("Wowow?");
    }

    private void OnDestroy()
    {
        EventManager.StopListening("CarrotBeingDestroyed", OnDestroy);
        Debug.Log("Carrot destroyed!");
    }

    public static List<Booster> GetBoosters()
    {
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
}
