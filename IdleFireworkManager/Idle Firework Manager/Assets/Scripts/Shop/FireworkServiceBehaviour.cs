using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class FireworkServiceBehaviour : MonoBehaviour
{
    public string[] serviceNamePool; //names are added in unity inspector, then will be randomly used when generating service
    string serviceName;
    public float[] serviceDurationPool; //durations are added in unity inspector, then will be randomly assign when generating service
    float durationRequirement; //indicate the duration required to finish the service
    float durationLeft;
    public Sprite[] serviceIconPool; //icons are added in unity inspector, index of serviceNamePool and serviceIconPool will shared (means that the sequence of putting icons in inspector is important)
    [HideInInspector] public bool isProcessing = false; //if the service has worker, and are processing, durationLeft need to be update constantly
    [HideInInspector] public bool isEmpty = true; //indicating whether the service slot is empty at the moment, player has 3 slot maximum
    [HideInInspector] public bool isFinished = false; //indicate whether the service is finish processing
    [HideInInspector] public GameObject inchargeWorker;
    float cashReward;
    int gemReward;

    //display free worker list variables
    public Transform freeWorkerListPanel;
    [HideInInspector] public Animator freeWorkerAnim;
    public Transform contentList;
    public GameObject freeWorkerPrefab;
    public GameObject noFreeWorkerTxtPrefab;

    //components and objs reference
    ShopRevenue shop;
    public Button statusBtn;
    public Image icon;
    public Image iconBorder;
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI durationTxt;
    public TextMeshProUGUI statusTxt;
    public Image fireworkServiceImg;
    public Sprite emptyBg;
    public Sprite occupiedBg;
    ConfirmationDialog serviceRewardPanel;
    TextMeshProUGUI rewardTxt;
    enum confirmEnum { confirm, cancel }
    AudioSource audio;

    //notification large icon
    public Texture2D serviceNotfIcon;

    //instant service completion gem price    
    int instantPrice;
    public TextMeshProUGUI instantPriceTxt;
    public ConfirmationDialog instantCompletePanel;


    private void Awake()
    {
        //playerprefs init
        string serviceNameKey = name + "Name";
        string serviceIsEmptyKey = name + "IsEmpty";
        string serviceIsProcessKey = name + "IsProcess";
        string serviceDurationReqKey = name + "DurationReq";
        string serviceDurationKey = name + "DurationLeft";
        string serviceInchargeWorker = name + "InchargeWorker";
        string serviceIconKey = name + "Icon";

        if (PlayerPrefs.HasKey(serviceIsEmptyKey))
        {
            if (PlayerPrefs.GetInt(serviceIsEmptyKey) == 0) //if service is not empty, init the service details based on saved data
            {
                isEmpty = false;
                if (PlayerPrefs.HasKey(serviceNameKey))
                {
                    serviceName = PlayerPrefs.GetString(serviceNameKey);
                    nameTxt.SetText(serviceName);
                }                

                if (PlayerPrefs.HasKey(serviceDurationReqKey))
                {
                    durationRequirement = PlayerPrefs.GetFloat(serviceDurationReqKey);
                    durationTxt.SetText((durationRequirement / 3600).ToString("F2").Replace(".", ":") + " hours");
                }
                if (PlayerPrefs.HasKey(serviceDurationKey))
                    durationLeft = PlayerPrefs.GetFloat(serviceDurationKey);
                if (PlayerPrefs.HasKey(serviceIconKey))
                    icon.sprite = serviceIconPool[PlayerPrefs.GetInt(serviceIconKey)];

                if (PlayerPrefs.HasKey(serviceIsProcessKey)) //if the service is currently in process based on saved data, continue process it
                {
                    if (PlayerPrefs.GetInt(serviceIsProcessKey) == 1)
                    {
                        isProcessing = true;
                        durationLeft -= (float)GameManager.dateDifference.TotalSeconds;
                    }
                    else
                    {
                        statusTxt.SetText("Send Worker");
                        isProcessing = false;
                    }
                }
                else
                    statusTxt.SetText("Send Worker");
            }
            else
                isEmpty = true;
        }
    }

    // Use this for initialization
    void Start()
    {
        audio = GetComponent<AudioSource>();
        shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<ShopRevenue>();
        serviceRewardPanel = GameObject.FindGameObjectWithTag("ServiceReward").GetComponent<ConfirmationDialog>();
        rewardTxt = serviceRewardPanel.transform.Find("RewardTxt").GetComponent<TextMeshProUGUI>();
        freeWorkerAnim = freeWorkerListPanel.GetComponent<Animator>();
    
        //init firework service obj appearance
        if (isEmpty)
        {
            fireworkServiceImg.sprite = emptyBg;
            statusBtn.gameObject.SetActive(false);
            iconBorder.gameObject.SetActive(false);
            nameTxt.gameObject.SetActive(false);
            durationTxt.gameObject.SetActive(false);
            statusTxt.gameObject.SetActive(false);
        }
        else
        {
            fireworkServiceImg.sprite = occupiedBg;
            //set active and initialize texts
            statusBtn.gameObject.SetActive(true);
            iconBorder.gameObject.SetActive(true);
            nameTxt.gameObject.SetActive(true);
            durationTxt.gameObject.SetActive(true);
            statusTxt.gameObject.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {        
		//if the service is processing, decrease durationLeft
        if (isProcessing)
        {
            durationLeft -= Time.deltaTime;
            if (durationLeft <= 0)
            {
                isFinished = true;
                isProcessing = false;

                //set status txt to finished
                statusTxt.SetText("<color=#3AB200FF>Receive Rewards!</color>");

                //random cash reward and gem reward (longer duration services gives bigger reward)
                cashReward = ((durationRequirement / 3600) + 1) * (Random.Range(ShopRevenue.highestCashRecord * 0.5f, ShopRevenue.highestCashRecord * 0.8f));
                gemReward = Random.Range(1, 5);
            }
            else
            {
                int hours = (int)durationLeft / 3600;
                int minutes = (int)(durationLeft % 3600) / 60;
                int seconds = (int)(durationLeft % 3600) % 60;
                statusTxt.SetText("<i>In Progress</i> (<color=#F87474FF>" + hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00") + "</color>)");
            }            
        }
    }

    //this function gonna invoke by TapSpawn() at SpawnManager, player is to find new firework services through introducing by customer they serve (tapping)
    //there is a small chance of customer introducing service to player, this is to encourage player to tap more!
    public void newService()
    {
        isEmpty = false;
        //string serviceIsEmptyKey = name + "IsEmpty";
        //PlayerPrefs.SetInt(serviceIsEmptyKey, 0);

        //random a service name, and set text
        int tempIndex = Random.Range(0, serviceNamePool.Length - 1);
        serviceName = serviceNamePool[tempIndex];
        nameTxt.SetText(serviceName);        

        //set icons and save the index of this icons
        icon.sprite = serviceIconPool[tempIndex];
        string tempIconKey = name + "Icon";
        PlayerPrefs.SetInt(tempIconKey, tempIndex);

        //random a duration, and set text
        int tempDuration = Random.Range(0, serviceDurationPool.Length - 1);
        durationRequirement = serviceDurationPool[tempDuration] * 3600; //multiply by 3600 because converting hour to seconds (we will update the durationLeft with seconds manner) 
        durationLeft = durationRequirement;
        durationTxt.SetText(serviceDurationPool[tempDuration].ToString("F2").Replace(".",":") + " hours");

        //set status TXT
        statusTxt.SetText("Send Worker");

        //set background img
        fireworkServiceImg.sprite = occupiedBg;        

        //set active and initialize texts
        statusBtn.gameObject.SetActive(true);
        iconBorder.gameObject.SetActive(true);
        nameTxt.gameObject.SetActive(true);
        durationTxt.gameObject.SetActive(true);
        statusTxt.gameObject.SetActive(true);
    }

    public void statusBtnClicked()
    {
        if (isProcessing)
        {
            //pay gem to finish it instantly!
            instantPrice = (int)Mathf.Ceil((durationLeft / 60 / 10) * 2); //10mins = 2 gems. ceiling implemented. e.g. 6 mins also 1 

            instantPriceTxt.SetText("Use " + instantPrice.ToString());
            instantCompletePanel.gameObject.SetActive(true);
            StartCoroutine(InstantComplete());
        }
        else if (isFinished)
        {           
            audio.Play();            

            //claim rewards (multiply rewards if incharge worker is businessman-type)
            WorkerBehaviour tempWorker = inchargeWorker.gameObject.GetComponent<WorkerBehaviour>();
            if (tempWorker.type == "businessman")
            {
                for (int x = 0; x < tempWorker.workerSkills.Length; x++)
                {
                    if (tempWorker.workerSkills[x].isUnlocked)
                    {
                        cashReward += cashReward * (tempWorker.workerSkills[x].skillEffect / 100);
                        gemReward += gemReward * (int)(tempWorker.workerSkills[x].skillEffect / 100);
                    }
                }
            }            
            if (gemReward >= 0)
                rewardTxt.SetText("<u>Service rewards</u>\n<color=yellow><size=40>$" + BigNumManager.BigNumString(cashReward) + "</color>, <color=#F87474FF>" + gemReward.ToString() + " gems </color></size>");
            else
                rewardTxt.SetText("<u>Service rewards</u>\n<color=yellow><size=40>$" + BigNumManager.BigNumString(cashReward) + "</color></size>");

            serviceRewardPanel.transform.localPosition = new Vector2(0, 0);
            StartCoroutine(confirmReceiveReward());
        }
        else
        {
            //if the free worker panel is opened, close it
            if (freeWorkerListPanel.localScale.y == 1)
            {
                freeWorkerAnim.SetTrigger("close");
                for (int x = 0; x < contentList.childCount; x++)
                {
                    Destroy(contentList.GetChild(x).gameObject);
                }                
            }
            //else, open the free worker list and initialize them
            else
            {
                freeWorkerAnim.SetTrigger("open");
                //prompt all workers with status "free" to select (FindGameobjectswithTag("Worker"))
                GameObject[] tempObj = GameObject.FindGameObjectsWithTag("worker");
                WorkerBehaviour[] tempWorkerList = new WorkerBehaviour[tempObj.Length];

                //if player dont have any worker, instantiate error message prefabs
                if (tempObj.Length == 0)
                {
                    Instantiate(noFreeWorkerTxtPrefab, contentList);
                }
                else
                {
                    int isAllUnavailable = 0;
                    //go through the full worker list, get their WorkerBehaviour scripts and check whether they are avaible
                    for (int x = 0; x < tempObj.Length; x++)
                    {                        
                        tempWorkerList[x] = tempObj[x].GetComponent<WorkerBehaviour>();
                        //if the worker is available (free), instantiate a free worker prefab in FreeWorkerListPanel, and initialize its text
                        if (tempWorkerList[x].isAvailable)
                        {
                            FreeWorkerReference temp = Instantiate(freeWorkerPrefab, contentList).GetComponent<FreeWorkerReference>();

                            //initialize worker photo
                            Image freeWorkerPhoto = temp.transform.Find("WorkerPhoto").GetComponent<Image>();
                            Image workerPhoto = tempObj[x].transform.Find("WorkerPhoto").GetComponent<Image>();
                            freeWorkerPhoto.sprite = workerPhoto.sprite;

                            //if this free worker is Businessman-type worker, calculate his unlocked skill to init the extra rewards Txt in FreeWorkerReference script
                            if (tempWorkerList[x].type == "Businessman")
                            {
                                float extraReward = 0;

                                for (int y = 0; y < tempWorkerList[x].workerSkills.Length; y++)
                                {
                                    if (tempWorkerList[x].workerSkills[y].isUnlocked)
                                        extraReward += tempWorkerList[x].workerSkills[y].skillEffect;
                                }

                                temp.initFreeWorker(tempWorkerList[x].workerID, tempWorkerList[x].name, extraReward);
                            }
                            else
                            {
                                temp.initFreeWorker(tempWorkerList[x].workerID, tempWorkerList[x].name, 0);
                            }
                        }
                        else
                            isAllUnavailable++;
                    }

                    //if all worker are not free, instantate error message prefab as well
                    if (isAllUnavailable == tempObj.Length)
                        Instantiate(noFreeWorkerTxtPrefab, contentList);
                }
            }
        }
    }    

    //all free worker list panel will close upon closing firework service panel
    public void OnDisable()
    {      

        if (freeWorkerListPanel.localScale.y > 0)
        {
            freeWorkerAnim.SetTrigger("close");
            freeWorkerListPanel.localScale = new Vector3(1, 0, 1);
        }        

        for (int x = 0; x < contentList.childCount; x++)
        {
            Destroy(contentList.GetChild(x).gameObject);
        }      

        PlayerPrefs.SetString("ServiceTimer", System.DateTime.Now.ToBinary().ToString()); //save last service panel disable time
    }

    private void OnEnable()
    {
        System.DateTime currentTime;
        System.DateTime oldTime;
        System.TimeSpan timeDifference;

        currentTime = System.DateTime.Now;
        if (PlayerPrefs.HasKey("ServiceTimer"))
        {
            //Grab the old time from the player prefs as a long        
            long temp = System.Convert.ToInt64(PlayerPrefs.GetString("ServiceTimer"));

            //Convert the old time from binary to a DataTime variable
            oldTime = System.DateTime.FromBinary(temp);

            //Use the Subtract method and store the result as a timespan variable
            timeDifference = currentTime.Subtract(oldTime);

            if (isProcessing)
            {
                durationLeft -= (float)(timeDifference.TotalSeconds);
            }
        }        
    }

    IEnumerator confirmReceiveReward()
    {
        while (serviceRewardPanel.result == -1)
            yield return null;

        WorkerBehaviour tempWorker = inchargeWorker.gameObject.GetComponent<WorkerBehaviour>();

        if (serviceRewardPanel.result == (int)confirmEnum.confirm)
        {
            //watch ads
            if (Advertisement.IsReady())
            {
                Advertisement.Show("rewardedVideo", new ShowOptions() { resultCallback = HandleAdResult });

                //finishing a firework service
                isEmpty = true;
                string serviceIsEmptyKey = name + "IsEmpty";
                PlayerPrefs.SetInt(serviceIsEmptyKey, 1);

                isFinished = false;
                fireworkServiceImg.sprite = emptyBg;
                statusBtn.gameObject.SetActive(false);
                iconBorder.gameObject.SetActive(false);
                nameTxt.gameObject.SetActive(false);
                durationTxt.gameObject.SetActive(false);
                statusTxt.gameObject.SetActive(false);

                tempWorker.isAvailable = true;
                tempWorker.workerStatusTxt.SetText("Free");
                tempWorker.workerActionTxt.SetText("Assign to shop >>>");
            }
            else
                shop.internetErrorMsg.SetTrigger("gemError");

            serviceRewardPanel.transform.localPosition = new Vector2(5000, 0);
            serviceRewardPanel.resetResult();

        }
        else if (serviceRewardPanel.result == (int)confirmEnum.cancel)
        {
            //DO SOMETHING WHEN CANCEL BUTTON IS CLICKED            
            //finishing a firework service
            shop.indieCash += cashReward;
            ShopRevenue.gem += gemReward;
            isEmpty = true;
            isFinished = false;
            fireworkServiceImg.sprite = emptyBg;
            statusBtn.gameObject.SetActive(false);
            iconBorder.gameObject.SetActive(false);
            nameTxt.gameObject.SetActive(false);
            durationTxt.gameObject.SetActive(false);
            statusTxt.gameObject.SetActive(false);

            tempWorker.isAvailable = true;
            tempWorker.workerStatusTxt.SetText("Free");
            tempWorker.workerActionTxt.SetText("Assign to shop >>>");

            serviceRewardPanel.transform.localPosition = new Vector2(5000, 0);
            serviceRewardPanel.resetResult();
        }                
    }

    IEnumerator InstantComplete()
    {
        while (instantCompletePanel.result == -1)
            yield return null;

        if (instantCompletePanel.result == (int)confirmEnum.confirm)
        {
            //DO SOMETHING WHEN CONFIRM BUTTON IS CLICKED
            if (shop.useGem(instantPrice))
            {
                durationLeft = 0; //finihsing a service instantly                
            }

            instantCompletePanel.gameObject.SetActive(false);
            instantCompletePanel.resetResult();

        }
        else if (instantCompletePanel.result == (int)confirmEnum.cancel)
        {
            //DO SOMETHING WHEN CANCEL BUTTON IS CLICKED

            instantCompletePanel.gameObject.SetActive(false);
            instantCompletePanel.resetResult();
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
            string serviceNameKey = name + "Name";
            string serviceIsEmptyKey = name + "IsEmpty";
            string serviceIsProcessKey = name + "IsProcess";
            string serviceDurationReqKey = name + "DurationReq";
            string serviceDurationKey = name + "DurationLeft";
            string serviceInchargeWorker = name + "InchargeWorker";

            if (!isEmpty)
            {
                PlayerPrefs.SetInt(serviceIsEmptyKey, 0);
                PlayerPrefs.SetString(serviceNameKey, serviceName);
                PlayerPrefs.SetFloat(serviceDurationReqKey, durationRequirement);
                PlayerPrefs.SetFloat(serviceDurationKey, durationLeft);
                if (isProcessing || isFinished)
                {
                    if (inchargeWorker != null)
                    {
                        PlayerPrefs.SetInt(serviceIsProcessKey, 1);
                        PlayerPrefs.SetInt(serviceInchargeWorker, inchargeWorker.GetComponent<WorkerBehaviour>().workerID);
                    }

                    if (SettingsConfig.hasNotification)
                    {
                        //set completion notification for this firework services
                        float delayTime = durationLeft;
                        NotificationManager.SendWithAppIcon(System.TimeSpan.FromSeconds(delayTime), "Your worker is back!", "Log in to claim your firework service's rewards!", new Color(1, 1, 1));

                    }

                }
                else
                {
                    PlayerPrefs.SetInt(serviceIsProcessKey, 0);
                }
            }
            else
                PlayerPrefs.SetInt(serviceIsEmptyKey, 1);

            PlayerPrefs.Save();
        }
    }

    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished: //DO SOMETHING WHEN ADS IS PLAYED
                cashReward *= 2;
                gemReward *= 2;
                shop.indieCash += cashReward;
                ShopRevenue.gem += gemReward;
                break;
            case ShowResult.Skipped: print("player didnt finish the ad"); break;
            case ShowResult.Failed: shop.internetErrorMsg.SetTrigger("gemError"); break;
        }
    }
}
