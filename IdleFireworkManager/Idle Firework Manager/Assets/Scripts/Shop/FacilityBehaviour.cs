using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class FacilityBehaviour : MonoBehaviour
{
    public string facName;
    int facLevel = 1;
    int maxLevel = 500;
    float costToUpgrade = 1;
    [HideInInspector] public int facTier = 0;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nameTierText;
    public TextMeshProUGUI descText; //facility description
    public TextMeshProUGUI secondaryDescTxt;
    public TextMeshProUGUI costText;
    //public TextMeshProUGUI upDescText; //upgrade button description

    public ShopRevenue shop;               //to refer the shop's object, to access the indieCash variable to determine the active of an upgrade button
    public SpawnManager custPerTap;         //to update the spawnManager in the scene to spawn multiple customer PER tap (based on Lucky Tap facility)
    UpgradeButtonState upgradeButton;      //to refer respective child upgrade button, need to change its sprite based on the active (whether enough cost to upgrade)... grey = insufficient, cyan = sufficient
    bool canUpgrade = false; //to indicate whether this facility can be upgrade with player's current available cash flow

    //game object (button) reference to determine which facility is upgraded to update their respective effect on the shop
    public GameObject luckyTapObj;
    public GameObject signboardObj;
    public GameObject cashierRegObj;

    bool isUpdatedLastMission = false;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("luckyTapLvl"))
        {
            if (gameObject == luckyTapObj) { facLevel = PlayerPrefs.GetInt("luckyTapLvl"); }
            if (gameObject == signboardObj) { facLevel = PlayerPrefs.GetInt("signboardLvl"); }
            if (gameObject == cashierRegObj) { facLevel = PlayerPrefs.GetInt("cashierRegLvl"); }
        }
        else
            facLevel = 1;

        string lastMissionKey = facName + "LastMis";
        if (PlayerPrefs.HasKey(lastMissionKey))
        {
            if (PlayerPrefs.GetInt(lastMissionKey) == 1)
                isUpdatedLastMission = true;
            else
                isUpdatedLastMission = false;
        }

        levelText.SetText("Lvl " + facLevel.ToString());
        nameTierText.SetText(facName + " <color=white> Tier " + facTier.ToString() + "</color>");        

        //INITIALIZE FACILITY BASED ON LEVELS SAVED
        if (facLevel != 1)
        {
            for (int x = 1; x < facLevel; x++)
            {
                costToUpgrade *= 1.2f;
                if (gameObject == luckyTapObj)
                {
                    //enhance customer per tap (increase 0.1 cust)
                    SpawnManager.numberToSpawn += 0.1f;
                }

                //if the upgraded facility is Signboard, decrease time needed to spawn an autom customer
                else if (gameObject == signboardObj)
                {
                    //stop the current coroutine, and reset with a new autoCustRate (increase 0.1f customer)                
                    ShopRevenue.autoCustRate += 0.1f;
                }
                //if the upgraded facility is cashier register, increase chances to get 
                else if (gameObject == cashierRegObj)
                {
                    //increase tips rate (increase 5%)
                    shop.tipsRate *= 1.05f;
                }                
            }

            //perform facility tier updating at last
            setFacTier();
            levelText.SetText("Lvl " + facLevel.ToString());
        }            

        costText.SetText("$" + BigNumManager.BigNumString(costToUpgrade));
        upgradeButton = transform.Find("UpgradeBtn").gameObject.GetComponent<UpgradeButtonState>();
    }    

    // Update is called once per frame
    void Update()
    {
        //calc finalCostToUpgrade based on currentBulk value
        float finalCostToUpgrade = BulkUpManager.bulkMultiplier(costToUpgrade, 1.2f, BulkUpManager.currentBulk);

        if (facLevel < maxLevel)        
            canUpgrade = shop.isEnoughMoney(finalCostToUpgrade);          
        else
            canUpgrade = false;

        //set this so that the upgrade button will update its active sprite on its side
        upgradeButton.isSufficient = canUpgrade;

        //each facility has different description
        if (gameObject == luckyTapObj)
        {
            descText.SetText(((int)SpawnManager.numberToSpawn).ToString());
            secondaryDescTxt.SetText(((SpawnManager.numberToSpawn % 1) * 100).ToString("F2") + "%");
        }            
        else if (gameObject == signboardObj)
        {
            descText.SetText(((int)ShopRevenue.autoCustRate).ToString());
            secondaryDescTxt.SetText(((ShopRevenue.autoCustRate % 1) * 100).ToString("F2") + "%");
        }

        else //cashier register
            descText.SetText(shop.tipsRate.ToString("F2") + "%");

        costText.SetText("$" + BigNumManager.BigNumString(finalCostToUpgrade));
    }

    public void UpgradeBtnClicked()
    {
        if (canUpgrade)
        {
            for (int x = 0; x < BulkUpManager.currentBulk; x++)
            {
                //player's cash flow (-)
                shop.indieCash -= costToUpgrade;
                
                //only upgrade facility if it is not maxed level
                if (facLevel < 500)
                    facLevel++;

                //facility's tier changing based on level
                //perform this only if the level modulus 100 is 0 (means it is 100 200 300 400 or 500)
                //increment of tier will have a big jump on costToUpgrade
                if (facLevel % 100 == 0)
                    setFacTier();

                //update level text
                levelText.SetText("Lvl " + facLevel.ToString());

                //increase cost (20% price increment on each upgrade)            
                costToUpgrade *= 1.2f;

                //INCREASE SHOP's EFFECT!=============================================================================
                //if the upgraded facility is Lucky Tap, increase customer invite rate per tap (1% per upgrade)
                if (gameObject == luckyTapObj)
                {
                    //enhance customer per tap (increase 0.1 cust)
                    SpawnManager.numberToSpawn += 0.1f;

                    //check mission
                    if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.luckytap)
                        MissionManager.UpdateMission(1);
                }

                //if the upgraded facility is Signboard, decrease time needed to spawn an autom customer
                if (gameObject == signboardObj)
                {
                    //stop the current coroutine, and reset with a new autoCustRate (increase 0.1 cust)                
                    ShopRevenue.autoCustRate += 0.1f;
                    ShopRevenue.custPerSecUpdateReq++;

                    //check mission
                    if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.signboard)
                        MissionManager.UpdateMission(1);
                }

                //if the upgraded facility is cashier register, increase chances to get 
                if (gameObject == cashierRegObj)
                {
                    //increase tips rate (increase 5%)
                    shop.tipsRate *= 1.05f;
                    ShopRevenue.totalTipsRateUpdateReq++;

                    //check mission
                    if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.cashier)
                        MissionManager.UpdateMission(1);
                }
            }

            //check mission            
            if (facLevel >= 100 && !isUpdatedLastMission)
            {
                if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.facility)
                    MissionManager.UpdateMission(1);

                isUpdatedLastMission = true;
            }
        }            
    }

    void setFacTier()
    {
        if (facLevel >= 1 && facLevel <= 99 && facTier != 0)
        {
            facTier = 0;
            nameTierText.SetText(facName + " <color=white> Tier " + facTier.ToString() + "</color>");            
        }
        else if (facLevel >= 100 && facLevel <= 199 && facTier != 1)
        {
            facTier = 1;
            nameTierText.SetText(facName + " <color=white> Tier " + facTier.ToString() + "</color>");
            costToUpgrade *= 2.0f;            
        }
        else if (facLevel >= 200 && facLevel <= 299 && facTier != 2)
        {
            facTier = 2;
            nameTierText.SetText(facName + " <color=white> Tier " + facTier.ToString() + "</color>");
            costToUpgrade *= 2.0f;
        }
        else if (facLevel >= 300 && facLevel <= 399 && facTier != 3)
        {
            facTier = 3;
            nameTierText.SetText(facName + " <color=white> Tier " + facTier.ToString() + "</color>");
            costToUpgrade *= 2.0f;
        }
        else if (facLevel >= 400 && facLevel <= 499 && facTier != 4)
        {
            facTier = 4;
            nameTierText.SetText(facName + " <color=white> Tier " + facTier.ToString() + "</color>");
            costToUpgrade *= 2.0f;
        }
        else if (facLevel >= 500 && facTier != 5)
        {
            facTier = 5;
            nameTierText.SetText(facName + " <color=white> Tier " + facTier.ToString() + "</color>");
            costToUpgrade *= 2.0f;
        }

        //update shop's tier if all facilities' tier reached the same threshold (e.g. 100, 200, 300, 400 or 500)
        shop.updateShopTier(facTier);

    }

    private void OnDisable()
    {
        OnApplicationPause(true);        
    }

    private void OnApplicationQuit()
    {
        OnApplicationPause(true);
    }

    private void OnDestroy()
    {
        OnApplicationPause(true);

    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (gameObject == luckyTapObj) { PlayerPrefs.SetInt("luckyTapLvl", facLevel); }
            if (gameObject == signboardObj) { PlayerPrefs.SetInt("signboardLvl", facLevel); }
            if (gameObject == cashierRegObj) { PlayerPrefs.SetInt("cashierRegLvl", facLevel); }

            string lastMissionKey = facName + "LastMis";
            if (isUpdatedLastMission)
            {
                PlayerPrefs.SetInt(lastMissionKey, 1);
            }
            else
                PlayerPrefs.SetInt(lastMissionKey, 0);
        }
    }
}
