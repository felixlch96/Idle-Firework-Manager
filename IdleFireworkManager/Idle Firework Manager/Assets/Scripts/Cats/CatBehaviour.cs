using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Advertisements;

public class CatBehaviour : MonoBehaviour
{
    public bool isSpecialCat;
    [HideInInspector] public bool isBlessingTriggered = false; //for special cat, once the auto cust rate blessing triggered, this special wont be dismiss until blessing end
    public static float specialCatBlessing = 0f; //boost auto customer rate 30% for 3 hours
    [HideInInspector] public ParticleSystem specialCatBlessingParticle;

    int rewardType; //For non-special cat. reward is either cash reward, or gem reward
    float reward; //storing the actual reward (cash or gem) value that will provide to player after they watch ads
    enum rewardEnum { cash, gem }
    ShopRevenue shop;

    ConfirmationDialog adsPanel; //tapping on cats will prompt the adsPanel 
    enum confirmEnum { confirm, cancel }
    TextMeshProUGUI blessingTxt;

    AudioSource audio;   

    private void Awake()
    {
        shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<ShopRevenue>();
        adsPanel = GameObject.FindGameObjectWithTag("catAdsPanel").GetComponent<ConfirmationDialog>();
        blessingTxt = adsPanel.transform.Find("BlessingTxt").GetComponent<TextMeshProUGUI>();
        audio = GetComponent<AudioSource>();
        
        //normal cat init
        if (!isSpecialCat)
            rewardType = Random.Range(0, 2);
        else //special cat init
        {
            specialCatBlessingParticle = gameObject.GetComponent<ParticleSystem>();
            specialCatBlessingParticle.Stop();
        }
    }

    private void Update()
    {
        //if special cat timer has counted down almost to 0, then dismiss this special cat
        if (isSpecialCat)
            if (CatSpawnManager.specialCatTimer <= 5)
                disableSpecialBlessing();
    }

    private void OnMouseUpAsButton()
    {
        // cat will only respond to player's touch only if the cat is not dismissing
        CatMovement tempCat = gameObject.GetComponent<CatMovement>();
        if (!tempCat.isDismiss)
        {
            audio.Play();
            //set blessingTxt based on reward type
            if (isSpecialCat)
            {
                if (!isBlessingTriggered)
                    blessingTxt.SetText("<u>Special Cat's Blessing</u>\n<color=#A1FF94FF>Auto Customer Rate + 30%</color>");
                else
                    return;
            }                
            else if (rewardType == (int)rewardEnum.cash)
            {
                reward = Random.Range(ShopRevenue.highestCashRecord * 0.5f, ShopRevenue.highestCashRecord * 0.8f);
                blessingTxt.SetText("<u>Cat's Blessing</u>\n<color=yellow>$" + BigNumManager.BigNumString(reward) + "</color>");
            }
            else if (rewardType == (int)rewardEnum.gem)
            {
                reward = Random.Range(3, 6);
                blessingTxt.SetText("<u>Cat's Blessing</u>\n<color=#BA0600FF>" + ((int)reward).ToString() + " gems</color>");
            }

            //display ads watch panel
            adsPanel.transform.localPosition = new Vector2(0, 0);
            StartCoroutine(confirmWatchAds());
        }        
    }

    IEnumerator confirmWatchAds()
    {
        while (adsPanel.result == -1)
            yield return null;

        if (adsPanel.result == (int)confirmEnum.confirm)
        {
            //watch ads
            if (Advertisement.IsReady())            
                Advertisement.Show("rewardedVideo", new ShowOptions() { resultCallback = HandleAdResult });        
            else
                shop.internetErrorMsg.SetTrigger("gemError");

            adsPanel.transform.localPosition = new Vector2(5000, 0);
            adsPanel.resetResult();
        }
        else if (adsPanel.result == (int)confirmEnum.cancel)
        {
            //DO SOMETHING WHEN CANCEL BUTTON IS CLICKED           
            //dismiss the cat regardless whether the player chose to watch ads or ignore the cat
            CatMovement tempCat = gameObject.GetComponent<CatMovement>();
            tempCat.dismissCatInstant();
            adsPanel.transform.localPosition = new Vector2(5000, 0);

            adsPanel.resetResult();
        }

        //check mission
        if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.cat)
            MissionManager.UpdateMission(1);
    }

    public void disableSpecialBlessing()
    {
        specialCatBlessing = 0f;
        //disable particle and dismiss special cat
        specialCatBlessingParticle.Stop();
        isSpecialCat = false;
        CatMovement tempCat = gameObject.GetComponent<CatMovement>();
        tempCat.dismissCatInstant();
        ShopRevenue.custPerSecUpdateReq++;
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
            if (isSpecialCat && isBlessingTriggered)
                PlayerPrefs.SetInt("hasSpecialCat", 1);
        }
    }

    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished: 
                //DO SOMETHING WHEN ADS IS PLAYED
                if (isSpecialCat)
                {
                    isBlessingTriggered = true;
                    specialCatBlessingParticle.Play();
                    CatSpawnManager.catIndicatorTXT.transform.parent.gameObject.SetActive(true);
                    specialCatBlessing += ShopRevenue.autoCustRate * 0.3f; //+ 30% auto customer rate 
                    ShopRevenue.custPerSecUpdateReq++;

                    //dismiss the cat regardless whether the player chose to watch ads or ignore the cat
                    CatMovement tempCat = gameObject.GetComponent<CatMovement>();
                    tempCat.dismissCatInstant();
                }
                else
                {
                    if (rewardType == (int)rewardEnum.cash)
                        shop.indieCash += reward;
                    else if (rewardType == (int)rewardEnum.gem)
                        ShopRevenue.gem += (int)reward;
                    //dismiss the cat regardless whether the player chose to watch ads or ignore the cat
                    CatMovement tempCat = gameObject.GetComponent<CatMovement>();
                    tempCat.dismissCatInstant();
                }
                break;
            case ShowResult.Skipped: print("player didnt finish the ad"); break;
            case ShowResult.Failed: shop.internetErrorMsg.SetTrigger("gemError"); break;
        }
    }
}
