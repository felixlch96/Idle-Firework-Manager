using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum MissionType { cash, earnGem, luckytap, signboard, cashier, fwUnlock, fwLvl, worker, workShop, service, skill, cat, fwDisplay, facility, finish } 

public class MissionManager : MonoBehaviour
{
    public static List<Mission> missionList = new List<Mission>();
    public static bool isFinish;

    public static int currentIndex = 0;

    //UI object references for updating details
    public GameObject missionBtn;
    public GameObject missionPanel;
    public TextMeshProUGUI descTxt;
    public TextMeshProUGUI currentCountTxt;
    public TextMeshProUGUI neededCountTxt;
    public TextMeshProUGUI gemRewardTxt;
    public UpgradeButtonState claimAvailability; //to change the claim button sprite based on whether player has finished the mission already or not

    public GameObject MissionIndicator1;
    public GameObject MissionIndicator2;
    AudioSource audio;

    //progress bar references for updating details
    public TextMeshProUGUI currentProgress;   
    public TextMeshProUGUI missionLength;
    public Image progressValue;
    
    private void Start()
    {
        audio = GetComponent<AudioSource>();

        //create the mission list over here
        missionList.Add(new Mission("Earn your first <u>100</u> golds!", (int)MissionType.cash, 100, 30));
        missionList.Add(new Mission("Earn 3 gems from customers", (int)MissionType.earnGem, 3, 30));
        missionList.Add(new Mission("Upgrade facility <u>LuckyTap</u> 20 times", (int)MissionType.luckytap, 1, 20, 20));
        missionList.Add(new Mission("Upgrade facility <u>Signboard</u> 20 times", (int)MissionType.signboard, 1, 20, 20));
        missionList.Add(new Mission("Upgrade facility <u>CashierRegister</u> 20 times", (int)MissionType.cashier, 1, 20, 20));
        missionList.Add(new Mission("Unlock new <u>firework</u> product", (int)MissionType.fwUnlock, 1, 30));
        missionList.Add(new Mission("Upgrade any <u>firework</u> product 30 times", (int)MissionType.fwLvl, 30, 30));
        missionList.Add(new Mission("Hire one <u>worker</u>", (int)MissionType.worker, 1, 30));
        missionList.Add(new Mission("Assign one <u>worker</u> to work at shop", (int)MissionType.workShop, 1, 30));
        missionList.Add(new Mission("Send one worker to a firework service", (int)MissionType.service, 1, 30));
        missionList.Add(new Mission("Unlock one <u>skill</u> of any worker", (int)MissionType.skill, 1, 30));
        missionList.Add(new Mission("Interact with 3 <u>cats</u>\n<size=20>*Cats appears at daytime*</size>", (int)MissionType.cat, 3, 20));
        missionList.Add(new Mission("<size=32>Perform <u>Firework-Display</u></size>\n<size=20>*Firework-Display available at night time*</size>", (int)MissionType.fwDisplay, 1, 50));
        missionList.Add(new Mission("Upgrade 3 facilities to TIER-1\n<size=20>*100lv = Tier1*", (int)MissionType.facility, 3, 100));


        //initialized variables based on playerprefs over here
        if (PlayerPrefs.HasKey("isFinish"))
        {
            //if player still has unfinished mission
            if (PlayerPrefs.GetInt("isFinish") == 0)
            {
                isFinish = false;
                currentIndex = PlayerPrefs.GetInt("currentIndex");
                missionList[currentIndex].countCurrent = PlayerPrefs.GetInt("currentProgress");

                //check if the current initalized mision is actually completed
                //in case player quit the game last time without claiming the mission's reward
                if (missionList[currentIndex].countCurrent >= missionList[currentIndex].countNeeded && !missionList[currentIndex].isCompleted)
                {
                    missionList[currentIndex].isCompleted = true;
                }
            }
            else
            {
                isFinish = true;
                //set inactive of mission button
                missionBtn.SetActive(false);
                missionPanel.SetActive(false);
            }
        }

        //initialzie current mission details
        descTxt.SetText(missionList[currentIndex].missionDesc);
        missionList[currentIndex].countCurrent = PlayerPrefs.GetInt("currentProgress");
        currentCountTxt.SetText(missionList[currentIndex].countCurrent.ToString());
        neededCountTxt.SetText(missionList[currentIndex].countNeeded.ToString());
        gemRewardTxt.SetText(missionList[currentIndex].gemReward.ToString());
        missionLength.SetText(missionList.Count.ToString());
        StartCoroutine(UpdateProgressBar());
    }

    private void Update()
    {
        if (!isFinish)
        {
            //constantly set the txt of player progression on current mission
            currentCountTxt.SetText(missionList[currentIndex].countCurrent.ToString());
                        
            if (missionList[currentIndex].isCompleted)
            {
                claimAvailability.isSufficient = true;
                MissionIndicator1.SetActive(true);
                MissionIndicator2.SetActive(true);
            }
            else
                claimAvailability.isSufficient = false;           
        }           
    }    

    public void InitNextMission()
    {
        currentIndex++;

        //check if the current completed mission is the last mission
        if (currentIndex >= missionList.Count)
        {
            isFinish = true; //player has successfully finished "Road To Manager" missions!!
            FinishingMissionList();
        }
        else
        {
            //set mission details to next mission if there is any
            descTxt.SetText(missionList[currentIndex].missionDesc);
            currentCountTxt.SetText("0");
            neededCountTxt.SetText(missionList[currentIndex].countNeeded.ToString());
            gemRewardTxt.SetText(missionList[currentIndex].gemReward.ToString());
        }
    }

    public void claimBtnClicked()
    {
        if (!isFinish)
        {
            if (claimAvailability.isSufficient)
            {               
                //play SFX
                audio.Play();
                //grant player completion rewards
                ShopRevenue.gem += missionList[currentIndex].gemReward;

                //initialize next mission            
                InitNextMission();
                StartCoroutine(UpdateProgressBar());
            }
        }     
        //if player has reached the last mission, after claim everything (above), then disable objectssss
        else
        {
            //set inactive of mission button
            missionBtn.SetActive(false);
            Animator panelAnim = missionPanel.GetComponent<Animator>();
            panelAnim.SetTrigger("close");            
            ShopRevenue.gem += 300;
        }
    }

    void FinishingMissionList()
    {
        MissionIndicator2.SetActive(true);
        claimAvailability.isSufficient = true;

        Mission finishMission = new Mission("<color=#176331FF>Congratulations!\nYou're officially a firework shop manager!</color>", (int)MissionType.finish, 1, 300);
        //set mission details to this last mission 
        descTxt.SetText(finishMission.missionDesc);
        currentCountTxt.SetText("1");
        neededCountTxt.SetText(finishMission.countNeeded.ToString());
        gemRewardTxt.SetText(finishMission.gemReward.ToString());
    }

    IEnumerator UpdateProgressBar()
    {
        currentProgress.SetText(currentIndex.ToString());

        float desiredFillAmount = (1.0f / missionList.Count) * currentIndex;
        while(progressValue.fillAmount < desiredFillAmount)
        {
            progressValue.fillAmount += 0.02f;
            yield return null;
        }

        progressValue.fillAmount = desiredFillAmount;
    }   

    private void OnApplicationQuit()
    {
        OnApplicationPause(true);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {           
            if (!isFinish)
            {
                PlayerPrefs.SetInt("isFinish", 0);
                PlayerPrefs.SetInt("currentIndex", currentIndex);
                PlayerPrefs.SetInt("currentProgress", missionList[currentIndex].countCurrent);                
            }
            else
            {
                PlayerPrefs.SetInt("isFinish", 1);
            }
        }
    }

    public static int GetMissionType()
    {
        if (missionList.Count > 0)
            return missionList[currentIndex].missionType;
        return 0;
    }

    public static void UpdateMission(int amount)
    {
        missionList[currentIndex].Increment(amount);
    }
}
