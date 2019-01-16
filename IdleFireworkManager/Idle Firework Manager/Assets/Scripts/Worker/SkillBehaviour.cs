using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillBehaviour : MonoBehaviour
{
    enum rarityEnum { common, rare, epic, legendary } //used to represent worker's rarity

    int gemsToUnlock = 100;
    public string skillName;
    public string skillDesc;
    [HideInInspector] public float skillEffect = 0;           // the exact skill effect value                       
    public float minSkillEffect; //for randomize purpose (enter the common rate) other rarity will use this value to multiply
    public float maxSkillEffect; //for randomize purpose (enter the common rate) other rarity will use this value to multiply
    public static float bonusCustRateEffect = 0;
    public static float bonusTipsRateEffect = 0;
    public static float bonusCustTapEffect = 0;
    public bool isActivated = false; //to prevent the same skill effect applied multiple times to stats
    WorkerBehaviour worker;

    public bool isUnlocked;
    public Sprite skillSprite;
    public Sprite lockedSprite;
    Image currentSkillIcon;
    public GameObject descBox;
    public TextMeshProUGUI skillDescTxt;

    ConfirmationDialog skillUnlockPanel;
    enum confirmEnum { confirm, cancel }

    ShopRevenue shop;
    AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        worker = transform.parent.GetComponent<WorkerBehaviour>();
        shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<ShopRevenue>();
        skillUnlockPanel = GameObject.FindGameObjectWithTag("SkillUnlockPanel").GetComponent<ConfirmationDialog>();

        currentSkillIcon = this.GetComponent<Image>();

        if (isUnlocked)
        {
            initializeSkill();
        }
        else
            currentSkillIcon.sprite = lockedSprite;
    }   

    public void skillBtnHeld()
    {
        //if player tap on the unlocked skill icon, the skill's desc should be display
        if (isUnlocked)
        {
            descBox.SetActive(true);
            descBox.transform.position = new Vector2(this.transform.position.x - 30, this.transform.position.y + 100);
            skillDescTxt.SetText("<b><u>" + skillName + "</u></b>\n" + skillDesc + skillEffect.ToString("F2") + "%");
        }       

    }    

    public void skillBtnReleased()
    {
        if (isUnlocked)
        {
            descBox.SetActive(false);
        }        
    }

    public void skillBtnClicked()
    {
        //if player release on a locked skill slot, prompt user whether want to unlock it
        if (!isUnlocked)
        {
            //set gemsTouUnlock based on worker rarity
            if (worker.rarity == (int)rarityEnum.common)
                gemsToUnlock = 100;
            else if (worker.rarity == (int)rarityEnum.rare)
                gemsToUnlock = 200;
            else if (worker.rarity == (int)rarityEnum.epic)
                gemsToUnlock = 300;
            else if (worker.rarity == (int)rarityEnum.legendary)
                gemsToUnlock = 400;

            skillUnlockPanel.transform.Find("confirmTxt").GetComponent<TextMeshProUGUI>().SetText("Are you sure to unlock this skill slot?\n<b>" + gemsToUnlock.ToString() + "</b>");

            StartCoroutine(skillUnlock());
            skillUnlockPanel.transform.localPosition = new Vector2(0.0f, 0.0f);
        }
    }

    //if player confirmed to unlock the skill slot, use gem to unlock!
    IEnumerator skillUnlock()
    {  
        while (skillUnlockPanel.result == -1)
            yield return null;

        if (skillUnlockPanel.result == (int)confirmEnum.confirm)
        {
            //if player confirmed to unlock, proceed the unlocking process
            if (shop.useGem(gemsToUnlock))
            {
                //determine exact skill effect value based on worker's rarity
                skillEffect = Random.Range(minSkillEffect, maxSkillEffect);
                int tempRarity = worker.getRarity();
                skillEffect *= (tempRarity + 1); //multiply skill effect   
                initializeSkill();
                audio.Play();

                //check mission
                if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.skill)
                    MissionManager.UpdateMission(1);
            }

            skillUnlockPanel.transform.localPosition = new Vector2(5000, 0);
            skillUnlockPanel.resetResult();

        }
        else if (skillUnlockPanel.result == (int)confirmEnum.cancel)
        {
            skillUnlockPanel.transform.localPosition = new Vector2(5000, 0);
            skillUnlockPanel.resetResult();
        }       
    }

    public void initializeSkill()
    {
        isUnlocked = true;
        currentSkillIcon = this.GetComponent<Image>();
        currentSkillIcon.sprite = skillSprite;

        //when unlocking a skill of a worker, and if this particular worker is same with the current worker in the shop
        //activate the skill effect immediately
        shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<ShopRevenue>();
        worker = transform.parent.GetComponent<WorkerBehaviour>();          
        
        if (shop.InchargeWorker != null)
        {
            if (shop.InchargeWorker.GetComponent<WorkerBehaviour>().workerID == worker.workerID)
            {
                activateEffect();
            }
        }        
    }

    public void activateEffect()
    {
        if (!isActivated)
        {
            isActivated = true;

            //add skill's effect onto shop
            if (skillName == "Sympathy")
            {
                bonusTipsRateEffect += skillEffect;
                ShopRevenue.totalTipsRateUpdateReq++;
            }
            if (skillName == "Charm")
            {
                bonusCustRateEffect += (ShopRevenue.autoCustRate * (skillEffect / 100));
                ShopRevenue.custPerSecUpdateReq++;
            }
            if (skillName == "DoppelGanger")
            {
                bonusCustTapEffect += (SpawnManager.numberToSpawn * (skillEffect / 100));
            }
        }        
    }
    
    public void deactivateEffect()
    {
        if (isActivated)
        {
            isActivated = false;

            //add skill's effect onto shop
            if (skillName == "Sympathy")
            {
                bonusTipsRateEffect -= skillEffect;
                ShopRevenue.totalTipsRateUpdateReq++;
            }
            if (skillName == "Charm")
            {
                bonusCustRateEffect -= (ShopRevenue.autoCustRate * (skillEffect / 100));
                ShopRevenue.custPerSecUpdateReq++;
            }
            if (skillName == "DoppelGanger")
            {
                bonusCustTapEffect -= (SpawnManager.numberToSpawn * (skillEffect / 100));
            }
        }        
    }
}
