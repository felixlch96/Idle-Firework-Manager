using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CatSpawnManager : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject[] catPrefab; //Others
    public GameObject specialCatPrefab; //WhiteCat
    public static float specialCatTimer = 0; //special cat cooldown timer after 1 special cat has been dismissed
    static float normalCatTimer = 150f; //normal cats cooldown timer during daytime
    public Transform parentObj;    

    bool isSpawning; //indicate whether this spawn manager is currently counting down to spawn a cat. Will be manipulated upon changing of day and night cycle
    public DayNightCycle daynightManager;

    public static TextMeshProUGUI catIndicatorTXT; //to update the specialCatIndicator's duration left TXT

	// Use this for initialization
	void Start ()
    {
        catIndicatorTXT = GameObject.FindGameObjectWithTag("CatIndicator").GetComponent<TextMeshProUGUI>();
        catIndicatorTXT.transform.parent.gameObject.SetActive(false);

        if (PlayerPrefs.HasKey("specialCatTimer"))
            specialCatTimer = PlayerPrefs.GetFloat("specialCatTimer");

        //immediately spawn a special cat if last online has a special cat blessing triggered
        //however, if the timer has run off while player offline, then do not spawn the special cat     
        if (PlayerPrefs.HasKey("hasSpecialCat"))
        {
            if (PlayerPrefs.GetInt("hasSpecialCat") == 1)
            {
                if (GameManager.dateDifference.TotalSeconds > specialCatTimer)
                {
                    PlayerPrefs.SetInt("hasSpecialCat", 0);
                    catIndicatorTXT.transform.parent.gameObject.SetActive(false);
                    specialCatTimer = 0;
                    WhiteCatBlessing.isInCatBlessingPackage = false; //reset the market purchased cat blessing state. Which enabling player to purchase next blessing
                }
                else
                {
                    //spawn a special cat, and continue the timer to spawn the next specialCat and dismiss the old cat
                    catIndicatorTXT.transform.parent.gameObject.SetActive(true);
                    specialCatTimer -= (float)GameManager.dateDifference.TotalSeconds;
                    CatBehaviour tempCat = spawnSpecialCat().GetComponent<CatBehaviour>();
                    tempCat.isBlessingTriggered = true;
                    tempCat.specialCatBlessingParticle.Play();
                    CatBehaviour.specialCatBlessing += ShopRevenue.autoCustRate * 0.3f; //- 30% time taken for one customer to auto spawn
                }
            }
            else
                specialCatTimer -= (float)GameManager.dateDifference.TotalSeconds;
        }

        //if it is day time, start spawn cats in timely manner (7am ~ 7pm)
        if (DayNightCycle.time > 25200 && DayNightCycle.time < 68400)
        {
            isSpawning = true;
        }
        else
            isSpawning = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        if (specialCatTimer > 0)
            specialCatTimer -= Time.deltaTime;

        //if is day time
        if (DayNightCycle.time > 25200 && DayNightCycle.time < 68400)
        {
            //start spawning cat
            if (!isSpawning)
            {
                isSpawning = true;    
            }

            //and spawn special cat as well if currently there are no special cat in the scene yet
            if (specialCatTimer <= 0)
            {
                WhiteCatBlessing.isInCatBlessingPackage = false; //reset the market purchased cat blessing state. Which enabling player to purchase next blessing
                specialCatTimer = 10800f; //3 hours cooldown
                spawnSpecialCat();
            }
        }
        else //if is night time
        {
            //stop spawning cat
            if (isSpawning)
            {
                isSpawning = false;
                normalCatTimer = 150f; //reset the normal cats timer
            }
        }
        
        if (isSpawning)
        {
            if (normalCatTimer >= 0)
            {
                normalCatTimer -= Time.deltaTime;
            }
            else
            {
                normalCatTimer = 150f;
                int CatToSpawn = Random.Range(0, catPrefab.Length - 1);
                GameObject tempCat = Instantiate(catPrefab[CatToSpawn], parentObj);
                tempCat.transform.position = spawnPoint.position;
            }
        }


        //update the catIndicatorTxt if has specialCat
        if (catIndicatorTXT.transform.parent.gameObject.activeSelf)
        {            
            if (specialCatTimer == Mathf.Infinity)
            {
                catIndicatorTXT.SetText("Infinity");
            }
            else if (specialCatTimer >= 86400)
            {
                int day = (int)specialCatTimer / 86400;
                catIndicatorTXT.SetText(day + "d");
            }
            else
            {
                int hours = (int)specialCatTimer / 3600;
                int minutes = (int)(specialCatTimer % 3600) / 60;
                int seconds = (int)(specialCatTimer % 3600) % 60;
                catIndicatorTXT.SetText(hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00"));
            }                       
        }
    }

    public GameObject spawnSpecialCat()
    {
        GameObject tempCat = Instantiate(specialCatPrefab, parentObj);
        tempCat.transform.position = spawnPoint.position;
        return tempCat;
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
            PlayerPrefs.SetFloat("specialCatTimer", specialCatTimer);
        }
    }
}
