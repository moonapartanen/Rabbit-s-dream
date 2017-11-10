using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotBoosters : MonoBehaviour {

    [HideInInspector] private GameObject shieldParticle = null;
    public static Rigidbody2D playerRb2D;
    private bool activateJumpBoost = false, shieldActive = false, boostFinished = false;
    private void Start()
    {
        if (!playerRb2D)
            playerRb2D = GameObject.Find("hero").GetComponent<Rigidbody2D>();
    }

    public class Booster
    {
        private string mBoosterName;
        private bool mBoosterUp = false;
        private float mVerticalBoost;

        public Booster(string name)
        {
            mBoosterName = name;
        }

        public Booster(string name, float lasts)
        {
            mBoosterName = name;
            mVerticalBoost = lasts;
        }

        public void setBoostActive()
        {
            mBoosterUp = !mBoosterUp;
        }

        public float getDistance()
        {
            return mVerticalBoost;
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
            boostFinished = true;
        }
    }

    private void ActivateShield()
    {
        if (!shieldParticle && shieldActive)
        {
            shieldParticle = (GameObject)Instantiate(Resources.Load("ShieldParticle"), playerRb2D.transform);
            shieldParticle.name = "shieldParticle";
            Invoke("RemoveShield", 5);
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
            GameObject shield = GameObject.Find("shieldParticle");

            if (shield != null) {
                Destroy(shield);
                shieldParticle = null;
                shieldActive = false;
                boostFinished = true;
            }
        }
    }

    public void CheckBoostName(string booster)
    {

        switch(booster)
        {
            case "VerticalBoost":
                activateJumpBoost = !activateJumpBoost;
                break;
            case "Shield":
                shieldActive = true;
                ActivateShield();
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
