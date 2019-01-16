using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WorkerBehaviour : MonoBehaviour
{    
    [HideInInspector] public int workerID; //used to keep track worker info (database)
    public TextMeshProUGUI workerNameTxt;
    public string[] namePool;
    public TextMeshProUGUI workerRarityTxt;
    public TextMeshProUGUI workerTypeTxt;
    public TextMeshProUGUI workerStatusTxt;
    public TextMeshProUGUI workerActionTxt;
    static Animator cantFireAnim;
    public Image workerActionBtnImg; //to change color

    ShopRevenue shop;
    public bool isAvailable; //is the worker currently available to be assign to shop? if yes, status = "free", else, status = proper description of unavailability  
    [HideInInspector] public int rarity;
    enum rarityEnum { common, rare, epic, legendary }
    [HideInInspector] public string type; //fixed, should be pre-defined in prefab
    enum typeEnum { FOTG, beggar, businessman, robot }
    public SkillBehaviour[] workerSkills;

    ConfirmationDialog fireWorkerPanel;
    int fireGemReturn;
    TextMeshProUGUI fireGemValueTxt;

    ConfirmationDialog assignWorkerPanel;
    enum confirmEnum { confirm, cancel }

    AudioSource audio;

    // Use this for initialization
    void Start ()
    {
        audio = GetComponent<AudioSource>();
        shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<ShopRevenue>();
        cantFireAnim = GameObject.FindGameObjectWithTag("cantfire").GetComponent<Animator>();

        if (isAvailable)
        {
            workerStatusTxt.SetText("Free");
            workerActionTxt.SetText("Assign to shop >>>");
        }

        fireWorkerPanel = GameObject.FindGameObjectWithTag("FireWorkerPanel").GetComponent<ConfirmationDialog>();
        fireGemValueTxt = fireWorkerPanel.transform.Find("GemTxt").transform.Find("GemValueTxt").GetComponent<TextMeshProUGUI>();
        assignWorkerPanel = GameObject.FindGameObjectWithTag("AssignWorkerPanel").GetComponent<ConfirmationDialog>();

        //set textssss
        //determine the rarity of the worker and set text with diff colours
        if (rarity == (int)rarityEnum.common)
        {
            workerRarityTxt.SetText("Rarity: <color=#6CF996FF>Common</color>");
            fireGemReturn = 20;
        }
        else if (rarity == (int)rarityEnum.rare)
        {
            workerRarityTxt.SetText("Rarity: <color=#7B71F0FF>Rare</color>");
            fireGemReturn = 40;
        }
        else if (rarity == (int)rarityEnum.epic)
        {
            workerRarityTxt.SetText("Rarity: <color=#B552FFFF>Epic</color>");
            fireGemReturn = 60;
        }
        else //rarity == legendary
        {
            workerRarityTxt.SetText("Rarity: <color=#FFD800FF>Legendary</color>");
            fireGemReturn = 80;
        }

        //set textssss
        workerNameTxt.SetText(name);
        workerTypeTxt.SetText("Type: <i>" + type + "</i>");
    }

    public void InitializeWorker(int tempType, int tempRarity)
    {
        workerID = Random.Range(1000, 9999);

        rarity = tempRarity;
        int rand = Random.Range(0, namePool.Length - 1);
        //if the worker is Face of the group
        if (tempType == (int)typeEnum.FOTG)
        {
            //random name            
            name = namePool[rand];
            
            //type initialization
            type = "Face Of The Group";
        }
        else if (tempType == (int)typeEnum.beggar)
        {
            //random name
            name = namePool[rand];
            
            //type initialization
            type = "Beggar";
        }
        else if (tempType == (int)typeEnum.businessman)
        {
            //random name
            name = namePool[rand];
            
            //type initialization
            type = "Businessman";
        }
        else //type == robot
        {
            //random name
            name = "No. " + Random.Range(0, 100).ToString();
            
            //type initialization
            type = "Robot";
        }               
    }

    public int getRarity()
    {
        return rarity;
    }

    public void fireBtnClicked()
    {
        //if the worker that player wanted to fire is currently working at store, OR
        //if the worker that player wanted to fire is currently at outstation (service)
        //basically, IF THE WORKER IS NOT AVAILABLE ( NOT FREE )
        if (!isAvailable)
        {
            //prompt error msg
            cantFireAnim.SetTrigger("cantFire");
        }
        else
        {
            StartCoroutine(confirmFireWorker());
            fireGemValueTxt.SetText(fireGemReturn.ToString());
            fireWorkerPanel.transform.localPosition = new Vector2(0, 0);
        }        
    }

    IEnumerator confirmFireWorker()
    {        
        while (fireWorkerPanel.result == -1)    
            yield return null;                

        if (fireWorkerPanel.result == (int)confirmEnum.confirm)
        {
            ShopRevenue.gem += fireGemReturn;
            Destroy(this.gameObject);
            fireWorkerPanel.transform.localPosition = new Vector2(5000, 0);
            fireWorkerPanel.resetResult();

        }
        else if (fireWorkerPanel.result == (int)confirmEnum.cancel)
        {
            fireWorkerPanel.transform.localPosition = new Vector2(5000, 0);
            fireWorkerPanel.resetResult();
        }
    }  

    public void statusBtnClicked()
    {
        //if the worker is free,
        if (isAvailable)
        {
            //check if the shop currently has worker, if no
            if (!ShopRevenue.hasWorker)
            {
                //directly assign this worker to the shop
                this.AssignWorker(true);
            }
            else //if the shop currently has other worker
            {
                //prompt and read confirmation on assigning this worker to shop
                StartCoroutine(confirmAssignWorker());
                assignWorkerPanel.transform.localPosition = new Vector2(0, 0);
            }
        }        
        //if the worker is currently unavailable (either working at shop, or went to services)
        else
        {
            //TO-DO: if worker at firework services, just prompt a floating error msg


            //else if this worker is currently working in the shop
            //directly set this worker to free
            if (this.gameObject == shop.InchargeWorker)            
                this.AssignWorker(false);            
        }        
    }

    IEnumerator confirmAssignWorker()
    {
        while (assignWorkerPanel.result == -1)
            yield return null;

        if (assignWorkerPanel.result == (int)confirmEnum.confirm)
        {
            //DO SOMETHING WHEN CONFIRM BUTTON IS CLICKED
            //kick the current worker away
            if (shop.InchargeWorker != null)
            {
                WorkerBehaviour tempWorker = shop.InchargeWorker.GetComponent<WorkerBehaviour>();
                tempWorker.AssignWorker(false);
            }            
            //assign this new worker
            this.AssignWorker(true);

            assignWorkerPanel.transform.localPosition = new Vector2(5000, 0);
            assignWorkerPanel.resetResult();

        }
        else if (assignWorkerPanel.result == (int)confirmEnum.cancel)
        {
            //DO SOMETHING WHEN CANCEL BUTTON IS CLICKED
            assignWorkerPanel.transform.localPosition = new Vector2(5000, 0);
            assignWorkerPanel.resetResult();
        }
    }

    public void AssignWorker(bool work)
    {
        //set worker and shop state
        isAvailable = !work;
        Image tempStatusBtn = workerStatusTxt.GetComponentInParent<Image>();
        shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<ShopRevenue>();

        //if is to set this worker to work at shop, set these properties
        if (work)
        {
            audio = GetComponent<AudioSource>();
            audio.Play();   
            workerStatusTxt.SetText("<color=red>Working at shop...</color>");
            workerActionTxt.SetText("Stop working <<<");
            workerActionBtnImg.color = new Color(1.0f, 0.25f, 0.25f);
            shop.InchargeWorker = this.gameObject;
            shop.workerPhotoInBubble.sprite = transform.Find("WorkerPhoto").GetComponent<Image>().sprite;

            for (int x = 0; x < workerSkills.Length; x++)
                if (workerSkills[x].isUnlocked)
                    workerSkills[x].activateEffect();

            ShopRevenue.hasWorker = work;

            //check mission
            if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.workShop)
                MissionManager.UpdateMission(1);
        }
        else //if to kick away this worker from working at shop, set these properties
        {
            shop.workerPhotoInBubble.sprite = null;
            WorkerBehaviour tempWorkerRefer = shop.InchargeWorker.GetComponent<WorkerBehaviour>();
            for (int x = 0; x < tempWorkerRefer.workerSkills.Length; x++)
                if (tempWorkerRefer.workerSkills[x].isUnlocked)
                    tempWorkerRefer.workerSkills[x].deactivateEffect();

            workerStatusTxt.SetText("Free");
            workerActionTxt.SetText("Assign to shop >>>");
            workerActionBtnImg.color = new Color(1f, 1f, 1f);
            ShopRevenue.hasWorker = false;
            shop.InchargeWorker = null;
        }                    
    }      
    
}
