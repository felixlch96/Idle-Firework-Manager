using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Advertisements;


public class ShopRevenue : MonoBehaviour
{
    public static float highestCashRecord = 0; //keep track of player's highest cash record, used to determine player's progression
    public float indieCash = 0;                //independent cash flow for this shop
    public static int gem = 100;                 //amount of gems player currently has
    public TextMeshProUGUI indieCashText;                 //to modify the indieCash text UI constantly
    public TextMeshProUGUI gemTxt;
    public static float revPerCust; //(float: Cash)how much does player earn per customer
    [HideInInspector] public float finalRevPerCust;
    [HideInInspector] public float finalCustPerTap;
    public float tipsRate;          //(Percent) how big is the tips does player will get if customer give them tips
    [HideInInspector] public float finalTipsRate;
    public static float autoCustRate = 0f;      //(Customer) how many customer will spawn automatically in 1 sec 
    [HideInInspector] public float finalAutoCustRate;
    int tier = 0;                       //shop's tier, dependent on shop's facilities
    static float tierBonus = 0;
    float spawnCustTimer = 0f;

    public TextMeshProUGUI shopDescTxt; //to update description text upon upgrading shop's tier
    public TextMeshProUGUI shopTierTxt; //to update shop's tier text upon upgrading shop's tier
    public static bool hasWorker = false; //to determine whether the shop has worker in it currently, if yes. enable offline revenue.
    public GameObject InchargeWorker; //to refer to whole worker game object that currently working in this shop
    public Transform workerBubble;
    public SpriteRenderer workerPhotoInBubble;

    //variables for updating main gameplay scene brief stats
    static float cashStack;
    public TextMeshProUGUI custPerSecTXT;      //for homepage brief stats
    public TextMeshProUGUI totalTipsRateTXT;   //for homepage brief stats
    public TextMeshProUGUI revPerCustTXT;      //for homepage brief stats
    public TextMeshProUGUI revPerSecondTXT;      //for homepage brief stats  
    public static int custPerSecUpdateReq = 1;       //the request that will increment upon changes from other script, then this script will update the txt accordingly
    public static int totalTipsRateUpdateReq = 1;    //the request that will increment upon changes from other script, then this script will update the txt accordingly
    public static int revPerCustUpdateReq = 1;       //the request that will increment upon changes from other script, then this script will update the txt accordingly
    public static int revPerSecondUpdateReq = 1;     //the request that will increment upon changes from other script, then this script will update the txt accordingly

    public SpawnManager autoSpawnTrigger; //once auto customer timer been finished countdown, trigger the auto spawn
    public ParticleSystem cashParticles; //to spawn cash particles after a successCust()
    public ParticleSystem tipsParticles; //to spawn tips particles after a successCust() with tips!
    public ParticleSystem gemsParticles; //to spawn gem particles after a successCust() with gem tips!
    public Animator gemErrorMsg; //to trigger the floating error message animation whenever a insufficient gem purchase attemps
    public Animator internetErrorMsg; //to trigger the floating error message animation whenever internet connection failed while requesting ads

    float briefStatsTimer = 1;
    //refer facilities's tier to update shop's tier
    public FacilityBehaviour luckyTapFac;
    public FacilityBehaviour signboardFac;
    public FacilityBehaviour cashierRegFac;

    //Offline Progression system variables------------------------    
    float offlineCash; //cash earned in offline timespan
    public ConfirmationDialog offlineProgPanel;
    Animator offlineProgAnim;
    TextMeshProUGUI offlineProgTXT;
    enum confirmEnum { confirm, cancel }
    //-------------------------------------------------------------     

    //reference to firework service script to check whether to introduce a service from a success cust
    IntroduceService serviceSpawner;

    ShopSFX shopSFX;
    
    private void Awake()
    {
        //initialize shop's variables       
        if (PlayerPrefs.HasKey("CashRecord"))
            highestCashRecord = PlayerPrefs.GetFloat("CashRecord");
        if (PlayerPrefs.HasKey("Cash"))
            indieCash = PlayerPrefs.GetFloat("Cash");
        if (PlayerPrefs.HasKey("Gem"))
            gem = PlayerPrefs.GetInt("Gem");
        if (PlayerPrefs.HasKey("HasWorker") && PlayerPrefs.GetInt("HasWorker") == 1)
            hasWorker = true;
        else
            hasWorker = false;

        //initialize textsss
        finalAutoCustRate = autoCustRate + tierBonus + SkillBehaviour.bonusCustRateEffect + CatBehaviour.specialCatBlessing + MinigameManager.totalAutoCustBoost;
        finalCustPerTap = SpawnManager.numberToSpawn + SkillBehaviour.bonusCustTapEffect;
        finalTipsRate = tipsRate + SkillBehaviour.bonusTipsRateEffect;

        if (DoubleThePrice.isDoublePrice)
            finalRevPerCust = (revPerCust * 2) + (revPerCust * FeverManager.FeverBoostEffect);
        else
            finalRevPerCust = revPerCust + (revPerCust * FeverManager.FeverBoostEffect);

        shopTierTxt.SetText("<color=yellow>Shop</color> - Tier " + tier.ToString());
        shopDescTxt.SetText("Bonus auto customer rate + " + (tier * 10 * 2).ToString() + "%");
    }

    private void Start()
    {
        serviceSpawner = GetComponent<IntroduceService>();
        offlineProgAnim = offlineProgPanel.GetComponent<Animator>();

        //only process offline progression when the shop has worker
        if (hasWorker)
        {
            initOfflineProgress();

            //if player went offline more than at least 1 minute
            if (offlineCash != 0)
            {                
                //display the offline progression panel to receive offline cash!
                offlineProgPanel.gameObject.SetActive(true);
                offlineProgTXT = offlineProgPanel.transform.Find("ProgressionTxt").GetComponent<TextMeshProUGUI>();
                offlineProgTXT.SetText("Your worker has earned you\n<color=yellow><size=40>$" + BigNumManager.BigNumString(offlineCash) + "</size></color>");
                StartCoroutine(confirmOfflineProg());
            }
        }

        //init shop's audiomanager
        shopSFX = GetComponent<ShopSFX>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (indieCash > highestCashRecord)
            highestCashRecord = indieCash;

        if (indieCash < 0)
            indieCash = 0;
        
        //if player has subscribed to double price package, then double the revPerCust 
        if (DoubleThePrice.isDoublePrice)
        {
            if (DoubleThePrice.doublePriceTimer <= 0)
            {
                revPerCustTXT.color = new Color(244, 244, 244);
                DoubleThePrice.isDoublePrice = false;
                revPerCustUpdateReq++;
            }
            else
            {
                DoubleThePrice.doublePriceTimer -= Time.deltaTime;
                finalRevPerCust = (revPerCust * 2) + (revPerCust * FeverManager.FeverBoostEffect);
            }
        }
        else
        {
            finalRevPerCust = revPerCust + (revPerCust * FeverManager.FeverBoostEffect);
        }

        //calculate final variables' values     
        finalAutoCustRate = autoCustRate + tierBonus + SkillBehaviour.bonusCustRateEffect + CatBehaviour.specialCatBlessing + MinigameManager.totalAutoCustBoost;        
        finalCustPerTap = SpawnManager.numberToSpawn + SkillBehaviour.bonusCustTapEffect;
        finalTipsRate = tipsRate + SkillBehaviour.bonusTipsRateEffect;

        //and SET TEXTS
        if (revPerCustUpdateReq > 0)
        {
            revPerCustTXT.SetText(BigNumManager.BigNumString(finalRevPerCust));
            revPerCustUpdateReq--;
        }
        if (custPerSecUpdateReq > 0)
        {
            custPerSecTXT.SetText(finalAutoCustRate.ToString("F2"));
            custPerSecUpdateReq--;
        }
        if (totalTipsRateUpdateReq > 0)
        {
            totalTipsRateTXT.SetText((finalTipsRate).ToString("F2"));
            totalTipsRateUpdateReq--;
        }

        indieCashText.SetText(BigNumManager.BigNumString(indieCash)); 
        gemTxt.SetText(gem.ToString());

        //if shop has worker in it, play the bubble animation
        if (hasWorker && !workerBubble.gameObject.activeSelf)        
            workerBubble.gameObject.SetActive(true);
        if (!hasWorker)        
            workerBubble.gameObject.SetActive(false);


        //update brief stats panel
        if (briefStatsTimer >= 0)
        {
            briefStatsTimer -= Time.deltaTime;
        }
        else
        {
            briefStatsTimer = 1; //reset timer           
            if (cashStack >= 0)
                revPerSecondTXT.SetText(BigNumManager.BigNumString(cashStack));
            else
                revPerSecondTXT.SetText("0");

            //reset stack value
            cashStack = 0;
        }

        //auto spawn customers calculation
        if (spawnCustTimer < 1) //1 second
        {
            spawnCustTimer += Time.deltaTime;
        }
        else //when timer reached the auto customer rate (1 second), spawn customer
        {
            spawnCustTimer = 0;
            autoSpawnTrigger.AutoSpawn(finalAutoCustRate);
        }

        //PC input to instantly get gem (for demo)
        if (Input.GetKey("g"))
            gem += 200;
    }

    //a successful customer's transaction will give player revenues. Many variables is affecting this function!
    public void SucessCust()
    {
        //determine whether to give bonus tips from customer over here
        float tempRandom = UnityEngine.Random.Range(0f, 100.0f);
        if (tempRandom <= 3.0f) //3% to get tips
        {
            if (tempRandom <= 0.1f)//0.1% to get gem as tips! 
            {
                gem++;
                gemsParticles.Play();

                //check mission
                if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.earnGem)
                    MissionManager.UpdateMission(1);
            }
                
            else
            {
                indieCash += finalRevPerCust + (finalRevPerCust * (finalTipsRate / 100));                
                tipsParticles.Play();
            }
            shopSFX.playTips();
        }
        else
        {
            indieCash += finalRevPerCust;
            cashParticles.Play();
            cashStack += finalRevPerCust;
        }

        //determine if want to introduce a firework service to player, if yes, execute it
        serviceSpawner.ToSpawnService();

        //check mission        
        if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.cash)
        {
            MissionManager.UpdateMission((int)finalRevPerCust);
        }
    }

    //to check if the shop has sufficient money to afford certain cost
    public bool isEnoughMoney(float cost)
    {
        if (indieCash >= cost)
            return true;
        else
            return false;
    }      

    //being called by one of the facilityBehaviour to update shop's tier
    //if all referred facilities has the same tier as the passed in parameter "checkFacTier"
    //then update shop's tier, else do nothing
    public void updateShopTier(int checkFacTier)
    {
        if (luckyTapFac.facTier == checkFacTier && signboardFac.facTier == checkFacTier && cashierRegFac.facTier == checkFacTier)
        {
            //update shop's tier
            tier = checkFacTier;
            shopTierTxt.SetText("<color=yellow>Shop</color> - Tier " + tier.ToString());
            //update the bonus auto customer effect (tier1 20%, tier2 40%, tier3 60%...)
            tierBonus = autoCustRate * 0.2f;            
            shopDescTxt.SetText("Bonus auto customer rate + " + (tier * 10 * 2).ToString() + "%");
        }            
    }
    
    public bool useGem(int gemToUse)
    {
        //if player has sufficient amount of gem to afford certain thing, proceed
        if (gem >= gemToUse)
        {
            gem -= gemToUse;
            if (gem < 0)
                gem = 0;

            return true;
        }
        else
        {
            gemErrorMsg.SetTrigger("gemError");
            return false;
        }
            
    }

    void initOfflineProgress()
    {       
        if (PlayerPrefs.HasKey("lastTime")) //note: first playthrough of the game will not have "lastTime" playrpref key
        {           
            offlineCash = (float)(GameManager.dateDifference.TotalSeconds * finalAutoCustRate) * finalRevPerCust / 2; //offline revenue is half of the normal revenues
        }       
    }
        
    IEnumerator confirmOfflineProg()
    {
        reinput: while (offlineProgPanel.result == -1)
            yield return null;

        if (offlineProgPanel.result == (int)confirmEnum.confirm)
        {
            //DO SOMETHING WHEN CONFIRM BUTTON IS CLICKED
            //watch ads
            if (Advertisement.IsReady())
            {
                Advertisement.Show("rewardedVideo", new ShowOptions() { resultCallback = HandleAdResult });
            }
            else
            {
                internetErrorMsg.SetTrigger("gemError");
                offlineProgPanel.resetResult();
                goto reinput;
            }

            offlineProgPanel.resetResult();
        }
        else if (offlineProgPanel.result == (int)confirmEnum.cancel)
        {
            //DO SOMETHING WHEN CANCEL BUTTON IS CLICKED
            indieCash += offlineCash;
            offlineProgAnim.SetTrigger("close");
            StartCoroutine(disableObjAfterSec(offlineProgPanel.gameObject, 1));
            offlineProgPanel.resetResult();
        }
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
            PlayerPrefs.SetFloat("CashRecord", highestCashRecord);
            PlayerPrefs.SetFloat("Cash", indieCash);
            PlayerPrefs.SetInt("Gem", gem);

            //last check on whether the shop has actually has worker, if no then reset hasWorker (in case theres bug)            
            if (hasWorker && InchargeWorker != null)
            {
                PlayerPrefs.SetInt("HasWorker", 1);
                PlayerPrefs.SetInt("InchargeWorker", InchargeWorker.GetComponent<WorkerBehaviour>().workerID);
            }
            else
                PlayerPrefs.SetInt("HasWorker", 0);
        }
    }

    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                offlineCash *= 2;
                indieCash += offlineCash;
                offlineProgAnim.SetTrigger("close");
                StartCoroutine(disableObjAfterSec(offlineProgPanel.gameObject, 1)); 
                    break;
            case ShowResult.Skipped: print("player didnt finish the ad"); break;
            case ShowResult.Failed: internetErrorMsg.SetTrigger("gemError"); break;
        }
    }

    IEnumerator disableObjAfterSec(GameObject obj, float sec)
    {
        yield return new WaitForSeconds(sec);
        obj.SetActive(false);
    }
}
