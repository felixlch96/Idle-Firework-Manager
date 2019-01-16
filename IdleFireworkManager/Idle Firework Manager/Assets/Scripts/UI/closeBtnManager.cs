using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class closeBtnManager : MonoBehaviour
{
    public GameObject objectToClose;
    ConfirmationDialog assignWorkerPanel;
    ConfirmationDialog fireWorkerPanel;
    ConfirmationDialog serviceRewardPanel;
    ConfirmationDialog skillUnlockPanel;


    // Use this for initialization
    void Start ()
    {
        fireWorkerPanel = GameObject.FindGameObjectWithTag("FireWorkerPanel").GetComponent<ConfirmationDialog>();
        assignWorkerPanel = GameObject.FindGameObjectWithTag("AssignWorkerPanel").GetComponent<ConfirmationDialog>();
        serviceRewardPanel = GameObject.FindGameObjectWithTag("ServiceReward").GetComponent<ConfirmationDialog>();
        skillUnlockPanel = GameObject.FindGameObjectWithTag("SkillUnlockPanel").GetComponent<ConfirmationDialog>();

    }

    // Update is called once per frame
    void Update () {
		
	}

    public void closeBtnClicked()
    {
        objectToClose.SetActive(false);
    }

    public void closeWorkerConfirmPanels()
    {
        assignWorkerPanel.transform.localPosition = new Vector2(5000, 0);
        fireWorkerPanel.transform.localPosition = new Vector2(5000, 0);
        skillUnlockPanel.transform.localPosition = new Vector2(5000, 0);
    }

    public void closeServiceConfirmPanels()
    {
        serviceRewardPanel.transform.localPosition = new Vector2(5000, 0);
    }
}
