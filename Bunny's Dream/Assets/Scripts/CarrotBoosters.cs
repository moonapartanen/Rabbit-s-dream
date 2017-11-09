using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotBoosters : MonoBehaviour {

    public class Booster
    {
        private string mBoosterName;
        private bool mBoosterUp = false;
        private float mVerticalBoost;

        public Booster(string name)
        {
            mBoosterName = name;
        }

        public Booster(string name, float distance)
        {
            mBoosterName = name;
            mVerticalBoost = distance;
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

    public static List<Booster> getBoosters()
    {
        List<Booster> boosterList = new List<Booster>();

        Booster verticalBoost = new Booster("VerticalBoost", 12f);
        boosterList.Add(verticalBoost);

        Booster shieldBoost = new Booster("Shield");
        boosterList.Add(shieldBoost);

        return boosterList;
    }
}
