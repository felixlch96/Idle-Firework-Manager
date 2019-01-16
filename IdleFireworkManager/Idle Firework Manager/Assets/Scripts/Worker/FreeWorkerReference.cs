using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FreeWorkerReference : MonoBehaviour
{
    [HideInInspector] public int freeWorkerID;


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void initFreeWorker(int workerID, string workerName, float workerSkill)
    {
        freeWorkerID = workerID;
        TextMeshProUGUI tempName = transform.Find("WorkerName").GetComponent<TextMeshProUGUI>();
        tempName.SetText(workerName);
        TextMeshProUGUI extraRewardTxt = transform.Find("ExtraRewardsTxt").GetComponent<TextMeshProUGUI>();

        if (workerSkill == 0)
            extraRewardTxt.SetText("Extra Reward: -");
        else
            extraRewardTxt.SetText("Extra Reward: " + workerSkill.ToString("F2") + "%");
    }

    public void sendWorkerBtnClicked()
    {
        GameObject[] tempObj = GameObject.FindGameObjectsWithTag("worker");
        WorkerBehaviour[] tempWorkerList = new WorkerBehaviour[tempObj.Length];
        for (int x = 0; x < tempObj.Length; x++)
            tempWorkerList[x] = tempObj[x].GetComponent<WorkerBehaviour>();

        //traverse up the hierarchy of this free worker reference to find the firework service to send to
        GameObject tempServiceObj = FindParentWithTag(this.gameObject, "Services");
        FireworkServiceBehaviour tempService = tempServiceObj.GetComponent<FireworkServiceBehaviour>();

        for (int x = 0; x < tempWorkerList.Length; x++)
        {
            //match this free worker ID with all existing worker's ID
            if (tempWorkerList[x].workerID == freeWorkerID)
            {
                //send worker!
                //set the worker and service details
                tempService.isProcessing = true;
                tempService.inchargeWorker = tempWorkerList[x].gameObject;
                tempService.OnDisable();
                tempWorkerList[x].workerStatusTxt.SetText("<color=red>Working at firework service...</color>");
                tempWorkerList[x].workerActionTxt.SetText("<s>Assign to shop >>></s>");
                tempWorkerList[x].isAvailable = false;

                //check mission
                if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.service)
                    MissionManager.UpdateMission(1);
            }
        }
    }

    GameObject FindParentWithTag(GameObject childObject, string tag)
    {
        Transform t = childObject.transform;
        while (t.parent != null)
        {
            if (t.parent.CompareTag(tag))
            {
                return t.parent.gameObject;
            }
            t = t.parent;
        }
        return null; // Could not find a parent with given tag.
    }
}
