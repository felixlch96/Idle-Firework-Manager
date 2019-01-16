using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GachaBehaviour : MonoBehaviour
{
    public int gemToGacha;
    public TextMeshProUGUI costTxt;
    public ShopRevenue shop;
    AudioSource audio; 

    //gacha items
    public GameObject[] workerPrefabs; //worker prefabs to gacha
    public Transform WorkerListParent; //to refer to worker list content, to instantiate worker under this parent

    //gacha summary display variables
    public GameObject summaryPanel;
    public Image summaryIcon;
    public TextMeshProUGUI summaryName;
    public TextMeshProUGUI summaryDesc;
    Animator summaryAnim;

	// Use this for initialization
	void Start ()
    {
        audio = GetComponent<AudioSource>();
        costTxt.SetText(gemToGacha.ToString());
        summaryAnim = summaryPanel.GetComponent<Animator>();
	}
	
    public void HireBtnClicked()
    {
        //if player has sufficient gem to gacha, proceed
        if (shop.useGem(gemToGacha))
        {
            audio.Play();
            //random worker prefab type (e.g. FOTG? Beggar? businessman? robot?)
            int tempType = Random.Range(0, workerPrefabs.Length);
            //random rarity (55% common, 30% rare, 12% epic, 3% legendary)
            int tempRarity = Random.Range(0, 100);
            if (tempRarity <= 55)
            {
                tempRarity = 0;
                summaryDesc.SetText("Rarity: <color=#6CF996FF>Common</color>");
            }                
            else if (tempRarity > 55 && tempRarity <= 85)
            {
                tempRarity = 1;
                summaryDesc.SetText("Rarity: <color=#7B71F0FF>Rare</color>");
            }                
            else if (tempRarity > 85 && tempRarity <= 97)
            {
                tempRarity = 2;
                summaryDesc.SetText("Rarity: <color=#B552FFFF>Epic</color>");
            }                
            else
            {
                tempRarity = 3; //LEGENDARY!
                summaryDesc.SetText("Rarity: <color=#FFD800FF>Legendary</color>");
            }
                
            
            //create new worker!
            GameObject newWorker = Instantiate(workerPrefabs[tempType], WorkerListParent);
            newWorker.transform.SetParent(WorkerListParent);
            WorkerBehaviour workerDetails = newWorker.GetComponent<WorkerBehaviour>();
            workerDetails.InitializeWorker(tempType, tempRarity);
            //unlocking and initializing first skill of this new worker
            SkillBehaviour skill1 = workerDetails.workerSkills[0].GetComponent<SkillBehaviour>();
            skill1.skillEffect = Random.Range(skill1.minSkillEffect, skill1.maxSkillEffect);            
            skill1.skillEffect *= (tempRarity + 1);

            //Display a gacha summary
            summaryIcon.sprite = newWorker.transform.Find("WorkerPhoto").GetComponent<Image>().sprite;
            summaryName.SetText(newWorker.name);            
            summaryPanel.SetActive(true);
            summaryAnim.SetTrigger("display");


            //check mission
            if (!MissionManager.isFinish && MissionManager.GetMissionType() == (int)MissionType.worker)
                MissionManager.UpdateMission(1);
        }        
    }
}
