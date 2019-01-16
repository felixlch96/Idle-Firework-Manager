using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public FadeManager fadeManager;
    
    //time variables (for last login and current login time)
    public static DateTime currentDate;
    public static DateTime oldDate;
    public static TimeSpan dateDifference;

    public GameObject[] objToInit;

    //load workers variables
    public GameObject beggarPrefab;
    public GameObject FOTGPrefab;
    public GameObject businessmanPrefab;
    public GameObject robotPrefab;
    public Transform WorkerListParent; //to refer to worker list content, to instantiate worker under this parent
    public FireworkServiceBehaviour[] fireworkServiceList;
    public GameObject serviceIndicator1;
    public GameObject serviceIndicator2;

    //quit panel
    public GameObject quitPanel;
    ConfirmationDialog quitPanelSelection;
    enum confirmEnum { confirm, cancel }

    public Canvas mainUI; //to disable the canvas renderer before quitting


    private void Awake()
    {
        PlayerPrefs.DeleteAll();

        //Store the current time when it starts        
        currentDate = DateTime.Now;      

        if (PlayerPrefs.HasKey("lastTime"))
        {
            //Grab the old time from the player prefs as a long        
            long temp = Convert.ToInt64(PlayerPrefs.GetString("lastTime"));

            //Convert the old time from binary to a DataTime variable
            oldDate = DateTime.FromBinary(temp);

            //Use the Subtract method and store the result as a timespan variable
            dateDifference = currentDate.Subtract(oldDate);
        }

        //set certain gameobjects to active before entering to the game, so that their variables are loaded and initialized
        for (int x = 0; x < objToInit.Length; x++)
            objToInit[x].SetActive(true);       
        

        if (PlayerPrefs.HasKey("minigameAutoCustBoost"))
            MinigameManager.totalAutoCustBoost = PlayerPrefs.GetFloat("minigameAutoCustBoost");     
    }

    private void Start()
    {        
        //update in-game clock based on current system time
        DayNightCycle.time = (currentDate.Hour * 3600) + ((currentDate.Minute * 60)) + currentDate.Second;

        //check if there are pending firework services, remind player to send workers
        for (int x = 0; x < fireworkServiceList.Length; x++)
        {
            if (fireworkServiceList[x].isEmpty == false && fireworkServiceList[x].isProcessing == false)
            {                
                serviceIndicator1.SetActive(true);
                serviceIndicator2.SetActive(true);
                break;
            }
        }       

        quitPanelSelection = quitPanel.GetComponent<ConfirmationDialog>();
        //cancel all notification if player log in to game
        NotificationManager.CancelAll();

        string filePath = Application.persistentDataPath + "/workers.txt";

        //load workers
        if (File.Exists(filePath))
        {
            string allWorkers = File.ReadAllText(filePath);
            string[] workers = allWorkers.Split(';');

            for (int x = 0; x < workers.Length; x++)
            {
                if (workers[x] == "") { }
                else
                {
                    JSONworker workerToLoad = JsonUtility.FromJson<JSONworker>(workers[x]);
                    GameObject workerObj;

                    if (workerToLoad.type == "Beggar")
                        workerObj = Instantiate(beggarPrefab, WorkerListParent);
                    else if (workerToLoad.type == "Businessman")
                        workerObj = Instantiate(businessmanPrefab, WorkerListParent);
                    else if (workerToLoad.type == "Face Of The Group")
                        workerObj = Instantiate(FOTGPrefab, WorkerListParent);
                    else if (workerToLoad.type == "Robot")
                        workerObj = Instantiate(robotPrefab, WorkerListParent);
                    else
                        break;

                    WorkerBehaviour tempWorker = workerObj.GetComponent<WorkerBehaviour>();
                    SkillBehaviour[] tempSkills = new SkillBehaviour[3];
                    tempSkills[0] = tempWorker.workerSkills[0];
                    tempSkills[1] = tempWorker.workerSkills[1];
                    tempSkills[2] = tempWorker.workerSkills[2];

                    tempWorker.workerID = workerToLoad.workerID;
                    tempWorker.name = workerToLoad.workerName;
                    tempWorker.isAvailable = workerToLoad.isAvailable;
                    tempWorker.rarity = workerToLoad.rarity;
                    tempWorker.type = workerToLoad.type;
                    tempSkills[0].isUnlocked = workerToLoad.skill1Unlock;
                    if (tempSkills[0].isUnlocked)
                    {
                        tempSkills[0].skillEffect = workerToLoad.skill1Effect;
                        tempSkills[0].initializeSkill();
                    }
                    tempSkills[1].isUnlocked = workerToLoad.skill2Unlock;
                    if (tempSkills[1].isUnlocked)
                    {
                        tempSkills[1].skillEffect = workerToLoad.skill2Effect;
                        tempSkills[1].initializeSkill();
                    }
                    tempSkills[2].isUnlocked = workerToLoad.skill3Unlock;
                    if (tempSkills[2].isUnlocked)
                    {
                        tempSkills[2].skillEffect = workerToLoad.skill3Effect;
                        tempSkills[2].initializeSkill();
                    }

                    //if this worker is not free from the last login, now check what he is working on (either shop or services) and set the details
                    if (!tempWorker.isAvailable)
                    {
                        //check if the worker is working at shop
                        if (ShopRevenue.hasWorker)
                        {
                            if (tempWorker.workerID == PlayerPrefs.GetInt("InchargeWorker"))
                            {
                                tempWorker.AssignWorker(true);
                                continue;
                            }
                        }

                        for (int y = 0; y < fireworkServiceList.Length; y++)
                        {
                            string serviceKey = fireworkServiceList[y].name + "IsProcess";
                            string serviceWorkerKey = fireworkServiceList[y].name + "InchargeWorker";
                            //if this service is currently in process, check if the sent worker is this loaded worker
                            if (PlayerPrefs.HasKey(serviceKey))
                            {
                                if (PlayerPrefs.GetInt(serviceKey) == 1)
                                {
                                    //if the service's incharge worker ID match this worker's ID, set details
                                    if (tempWorker.workerID == PlayerPrefs.GetInt(serviceWorkerKey))
                                    {
                                        fireworkServiceList[y].inchargeWorker = tempWorker.gameObject;
                                        tempWorker.workerStatusTxt.SetText("<color=red>Busy at firework service...</color>");
                                        tempWorker.workerActionTxt.SetText("<s>Assign to shop >>></s>");                                        
                                        break;
                                    }
                                }                                                             
                            }
                        }
                    }
                }
            }
        }

        for (int x = 0; x < objToInit.Length; x++)
            objToInit[x].SetActive(false);

    }

    private void Update()
    {        
        //android "back" button will prompt or hide the quitPanel
        if (Input.GetKeyDown(KeyCode.Escape))
            if (!quitPanel.activeSelf)
            {
                quitPanel.SetActive(true);
                StartCoroutine(confirmQuit());
            }                
            else
            {
                quitPanel.SetActive(false);
                StopCoroutine(confirmQuit());
            }                       
    }

    public void ToDisplayFirework()
    {
        //deactivate all child before into scene
        for (int x = 0; x < transform.childCount; x++)
        {
            transform.GetChild(x).gameObject.SetActive(false);
        }

        //check mission
        if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.fwDisplay)
            MissionManager.UpdateMission(1);

        SceneManager.LoadScene("Firework Display scene", LoadSceneMode.Additive);
    }

    private void OnApplicationQuit()
    {
        OnApplicationPause(true);    
    }  

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            //disable canvas renderer first before setting all those panel to true    
            mainUI.enabled = false;

            for (int x = 0; x < objToInit.Length; x++)
                objToInit[x].SetActive(true);

            WriteWorkerToFile();

            PlayerPrefs.SetString("lastTime", DateTime.Now.ToBinary().ToString()); //save last login time                
        }        
        else
        {           
            //set certain gameobjects to active before entering to the game, so that their variables are loaded and initialized
            for (int x = 0; x < objToInit.Length; x++)
                objToInit[x].SetActive(false);

            //disable canvas renderer first before setting all those panel to true    
            mainUI.enabled = true;
        }
    }    
    
    IEnumerator confirmQuit()
    {
        while (quitPanelSelection.result == -1)
            yield return null;

        if (quitPanelSelection.result == (int)confirmEnum.confirm)
        {
            //DO SOMETHING WHEN CONFIRM BUTTON IS CLICKED
            fadeManager.fadeOut();            
            Application.Quit();
        }
        else if (quitPanelSelection.result == (int)confirmEnum.cancel)
        {
            //DO SOMETHING WHEN CANCEL BUTTON IS CLICKED

            quitPanelSelection.resetResult();
        }
        quitPanel.SetActive(false);
    }

    void WriteWorkerToFile()
    {
        string textToWrite = "";
        string filePath = Application.persistentDataPath + "/workers.txt";

        //save workers
        GameObject[] workersObj = GameObject.FindGameObjectsWithTag("worker");
        if (workersObj.Length == 0)
            File.WriteAllText(filePath, ""); //if there are no workers, remember to reset the workers.txt
        else
        {
            for (int x = 0; x < workersObj.Length; x++)
            {
                WorkerBehaviour tempWorker = workersObj[x].GetComponent<WorkerBehaviour>();
                SkillBehaviour[] tempSkills = new SkillBehaviour[3];
                tempSkills[0] = tempWorker.workerSkills[0];
                tempSkills[1] = tempWorker.workerSkills[1];
                tempSkills[2] = tempWorker.workerSkills[2];

                JSONworker workerToSave = new JSONworker { };
                workerToSave.workerID = tempWorker.workerID;
                workerToSave.workerName = tempWorker.name;                

                workerToSave.isAvailable = tempWorker.isAvailable;
                workerToSave.rarity = tempWorker.getRarity();
                workerToSave.type = tempWorker.type;
                workerToSave.skill1Unlock = tempSkills[0].isUnlocked;
                workerToSave.skill1Effect = tempSkills[0].skillEffect;
                workerToSave.skill2Unlock = tempSkills[1].isUnlocked;
                workerToSave.skill2Effect = tempSkills[1].skillEffect;
                workerToSave.skill3Unlock = tempSkills[2].isUnlocked;
                workerToSave.skill3Effect = tempSkills[2].skillEffect;

                textToWrite += JsonUtility.ToJson(workerToSave) + ";";
                File.WriteAllText(filePath, textToWrite);                
            }
        }
    }

    //=====================================================================================================================================
    private class JSONworker
    {
        public int workerID;
        public string workerName;
        public bool isAvailable;
        public int rarity;
        public string type;
        public bool skill1Unlock;
        public float skill1Effect;
        public bool skill2Unlock;
        public float skill2Effect;
        public bool skill3Unlock;
        public float skill3Effect;
    }
}
