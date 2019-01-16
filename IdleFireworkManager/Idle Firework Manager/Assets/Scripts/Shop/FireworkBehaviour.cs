using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FireworkBehaviour : MonoBehaviour
{
    public TextMeshProUGUI totalPriceStatTxt;

    public TextMeshProUGUI levelText;   
    public TextMeshProUGUI priceText; //firework description (price)
    public ShopRevenue shop;               //to refer the shop's object, to access the indieCash variable to determine the active of an upgrade button
    public UpgradeButtonState upgradeButton;      //to refer respective child upgrade button, need to change its sprite based on the active (whether enough cost to upgrade)... grey = insufficient, cyan = sufficient
    public TextMeshProUGUI costTxt; //to refer the child of upgradeButton to update the text

    //fireworks properties
    public float fwPrice; //each firework has different starting price
    int fwLevel = 1;
    int maxLevel = 9999;
    public float costToUpgrade; //each firework has different starting upgrade cost
    bool canUpgrade = false; //to indicate whether this firework can be upgrade with player's current available cash flow   

    //unlocking next firework variables
    public bool isUnlocked;
    public float costToUnlock;
    Transform selfRefer;
    Transform parentRefer;
    int childCountRefer;
    public GameObject unlockPanel;
    public UpgradeButtonState unlockBtn;
    public TextMeshProUGUI unlockTxt;


    AudioSource audio;

    // Use this for initialization
    void Awake()
    {
        //initialize self, parent, and total childcount reference (for fireworks unlocking purpose)
        selfRefer = transform;
        parentRefer = transform.parent;
        childCountRefer = parentRefer.childCount;

        string fwUnlockKey = gameObject.name + "unlock";
        if (PlayerPrefs.HasKey(fwUnlockKey) && PlayerPrefs.GetInt(fwUnlockKey) == 1)
            isUnlocked = true;

        //if this firework is unlocked, set the next firework to active
        if (isUnlocked)
        {
            //initialize properties
            initializeFW();
            unlockPanel.SetActive(false);
            //check full list of firework of which is already unlocked (based on saved state)
            setNextFwActive(true);
            MinigameManager.totalFireworkUnlocked++;
        }
        else
        {
            unlockPanel.SetActive(true);
            unlockTxt.SetText("Unlock\n$" + BigNumManager.BigNumString(costToUnlock));

        }
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //calc finalCostToUpgrade based on currentBulk value
        float finalCostToUpgrade = BulkUpManager.bulkMultiplier(costToUpgrade, 1.2f, BulkUpManager.currentBulk);

        //check if player's available cash flow is sufficient for respective upgrade, if no, set it to unavailable to click.
        canUpgrade = shop.isEnoughMoney(finalCostToUpgrade);          
        //set this so that the upgrade button will update its active sprite on its side
        upgradeButton.isSufficient = canUpgrade;  
        costTxt.SetText("$" + BigNumManager.BigNumString(finalCostToUpgrade));

        if (!isUnlocked)
        {
            unlockBtn.isSufficient = shop.isEnoughMoney(costToUnlock);
        }
    }

    public void UpgradeBtnClicked()
    {
        if (canUpgrade)
        {
            for (int x = 0; x < BulkUpManager.currentBulk; x++)
            {
                //player's cash flow (-)
                shop.indieCash -= costToUpgrade;

                fwLevel++;

                //20% increase on costToUpgrade
                costToUpgrade *= 1.2f;
                //add the price increment (11%) into totalPrice
                ShopRevenue.revPerCust += (fwPrice * 0.11f);

                //every 100 level of a firework, the price (revenue) will have a lil boost. (2x the current price)
                //else just normal increment (11%)
                if (fwLevel % 100 == 0)
                    fwPrice *= 2;
                else
                    fwPrice *= 1.11f;

                //check mission
                if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.fwLvl)
                    MissionManager.UpdateMission(1);
            }
            priceText.SetText("Sales Price: $" + BigNumManager.BigNumString(fwPrice));
            levelText.SetText("Lvl " + fwLevel);
            totalPriceStatTxt.SetText("Total revenue per customer: " + BigNumManager.BigNumString((ShopRevenue.revPerCust)));
            ShopRevenue.revPerCustUpdateReq++;
        }
    }

    //this function will be invoked during initialization because we only save firework's level, and we use this level to re-calc price when player load the game
    float calcPrice(int lvl)
    {
        if (lvl != 1)
        {
            float temp = fwPrice;
            for (int x = 1; x < lvl; x++)
            {
                //every 100 level of a firework, the price (revenue) will have a lil boost. (2x the current price)
                if (x % 100 == 0)
                {
                    temp *= 2;
                }
                else
                    temp *= 1.11f;
            }

            return temp;
        }
        else
            return fwPrice;
    }

    public void unlockBtnClicked()
    {
        if (shop.isEnoughMoney(costToUnlock))
        {
            audio.Play();
            isUnlocked = true;
            //update shop's cash flow
            shop.indieCash -= costToUnlock;
            //set inactive for the unlock panel for the "unlocking" firework
            unlockPanel.SetActive(false);
            //initialize the firework product properties based on the value entered in unity editor
            initializeFW();
            //set active of next firework
            setNextFwActive(false);
            MinigameManager.totalFireworkUnlocked++;

            //check mission
            if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.fwUnlock)
                MissionManager.UpdateMission(1);
        }
    }

    void initializeFW()
    {        
        string fwKey = gameObject.name + "lvl";
        if (PlayerPrefs.HasKey(fwKey))
            fwLevel = PlayerPrefs.GetInt(fwKey);
        else
            fwLevel = 1;

        //please, i think there are better solutions that this to calculate costToUpgrade everytime player first enter to game 
        //instead of storing each upgrade cost FOR EACH FACILITY (and fireworks)
        for (int x = 0; x < fwLevel; x++)
            costToUpgrade *= 1.2f;

        //initiating properties
        fwPrice = calcPrice(fwLevel);
        priceText.SetText("Sales Price: $" + BigNumManager.BigNumString(fwPrice));
        costTxt.SetText("$" + BigNumManager.BigNumString(costToUpgrade));
        levelText.SetText("Lvl " + fwLevel);
        ShopRevenue.revPerCustUpdateReq++;

        //initalizing
        ShopRevenue.revPerCust += fwPrice;
        totalPriceStatTxt.SetText("Total revenue per customer: " + BigNumManager.BigNumString(ShopRevenue.revPerCust));
    }

    void setNextFwActive(bool isFullListCheck)
    {
        //the first one is not firework product, so start from x = 1
        for (int x = 1; x < childCountRefer; x++)
        {
            if (parentRefer.GetChild(x) == selfRefer)
            {
                //if the current child is not the last one, proceed to set next child to active. else dont do that
                if (x != childCountRefer - 1)
                    parentRefer.GetChild(x + 1).gameObject.SetActive(true);
                //if we only want to set active the very next firework ONLY, then dont check the full                
                    break;
            }
        }
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
            if (isUnlocked)
            {
                string fwKey = gameObject.name + "lvl";
                PlayerPrefs.SetInt(fwKey, fwLevel);
                string fwUnlockKey = gameObject.name + "unlock";
                PlayerPrefs.SetInt(fwUnlockKey, 1);
            }
        }
    }
}
