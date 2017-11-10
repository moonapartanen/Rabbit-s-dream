using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotBoosters : MonoBehaviour {

    [HideInInspector] private GameObject shieldParticle = null;
    public static Rigidbody2D playerRb2D;
    private CharacterControl heroScript;
    private bool activateJumpBoost = false, shieldActive = false, boostFinished = false;
    private void Start()
    {
        if (!playerRb2D)
            playerRb2D = GameObject.Find("hero").GetComponent<Rigidbody2D>();

        if (!heroScript)
            heroScript = (CharacterControl)GameObject.Find("hero").GetComponent(typeof(CharacterControl));
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

        public void setBoostActive()
        {
            mBoosterUp = !mBoosterUp;
        }

        public float getLength()
        {
            return mLength;
        }

        public string getName()
        {
            return mBoosterName;
        }

        public bool status()
        {
            return mBoosterUp;
        }
    }

    private void FixedUpdate()
    {
        if (activateJumpBoost)
        {
            playerRb2D.AddForce(new Vector2(0f, 1200f));
            activateJumpBoost = false;
            heroScript.boostStatus();
            heroScript.activeBoostName = "";
        }
    }

    private void ActivateShield(Booster booster)
    {
        if (!shieldParticle && shieldActive)
        {
            Debug.Log((System.Int32)(booster.getLength()));
            shieldParticle = (GameObject)Instantiate(Resources.Load("ShieldParticle"), playerRb2D.transform);
            shieldParticle.name = "shieldParticle";
            Invoke("RemoveShield", (System.Int32)(booster.getLength()));
        }
    }

    public bool boostDone()
    {
        return boostFinished;
    }

    private void RemoveShield()
    {
        Debug.Log("Shield gone now...");
        if (shieldParticle && shieldActive)
        {
            if (shieldParticle != null) {
                Destroy(shieldParticle);
                shieldParticle = null;
                shieldActive = false;
                heroScript.boostStatus();
                heroScript.activeBoostName = "";
            }
        }
    }

    public void CheckBoostName(Booster booster)
    {

        switch(booster.getName())
        {
            case "VerticalBoost":
                activateJumpBoost = !activateJumpBoost;
                break;
            case "Shield":
                shieldActive = true;
                ActivateShield(booster);
                break;
        }
    }

    public static List<Booster> getBoosters()
    {
        List<Booster> boosterList = new List<Booster>();

        Booster verticalBoost = new Booster("VerticalBoost", 0.0f);
        boosterList.Add(verticalBoost);

        Booster shieldBoost = new Booster("Shield", 8f);
        boosterList.Add(shieldBoost);
        boosterList.Add(shieldBoost);
        boosterList.Add(shieldBoost);
        boosterList.Add(shieldBoost);
        return boosterList;
    }
}
