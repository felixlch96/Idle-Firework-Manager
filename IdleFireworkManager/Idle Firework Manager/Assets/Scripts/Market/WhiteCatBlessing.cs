using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteCatBlessing : MonoBehaviour
{
    public ShopRevenue shop; //to access useGem function
    public static bool isInCatBlessingPackage = false; //indicate whether the player are in a WhiteCatBlessingPackage
    AudioSource audio;

    public CatSpawnManager catSpawner; //to spawn a special cat if a blessing package is purchased and there are no special cat exist in the scene atm
    public ConfirmationDialog purchaseInvalid; //the pop-up box that indicate user that player already has a blessing purchased, another purchase is not allowed
    enum confirmEnum { confirm, cancel }

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        if(PlayerPrefs.HasKey("isInCatBlessingPackage"))
        {
            if (PlayerPrefs.GetInt("isInCatBlessingPackage") == 1)
                isInCatBlessingPackage = true;
            else
                isInCatBlessingPackage = false;
        }
    }
    public void BuyWhiteCat(int gemToBuy)
    {      
        //if player has already purchased a cat blessing and currently in effect,
        //prompt message "purchase unsuccessful"
        if (isInCatBlessingPackage)
        {
            purchaseInvalid.transform.localPosition = new Vector2(0, 0);
            StartCoroutine(confirmBlessingInvalid());
        }

        else if (shop.useGem(gemToBuy))
        {
            //check if there are already has special cat in the scene
            //if yes, straight away extend the CatSpawnmanager.specialCatTimer
            GameObject specialCatObj = GameObject.FindGameObjectWithTag("SpecialCat");
            isInCatBlessingPackage = true;
            audio.Play();

            if (specialCatObj != null)
            {
                CatBehaviour specialCatOnScene = specialCatObj.GetComponent<CatBehaviour>();

                if (!specialCatOnScene.isBlessingTriggered)
                {
                    specialCatOnScene.isBlessingTriggered = true;
                    specialCatOnScene.specialCatBlessingParticle.Play();
                    CatBehaviour.specialCatBlessing += ShopRevenue.autoCustRate * 0.3f; //+ 30% auto customer rate 
                    ShopRevenue.custPerSecUpdateReq++;
                }

                switch (gemToBuy)
                {
                    case 100: CatSpawnManager.specialCatTimer = 86400f; break;
                    case 500: CatSpawnManager.specialCatTimer = 604800f; break;
                    case 1500: CatSpawnManager.specialCatTimer = 2419200f; break;
                    case 3000: CatSpawnManager.specialCatTimer = Mathf.Infinity; break;
                }
                
            }
            //if there are no special cat at the moment, spawn one and set trigger of the special cat's blessing
            //as well as the CatSpawnmanager.specialCatTimer
            else
            {
                CatBehaviour tempCat = catSpawner.spawnSpecialCat().GetComponent<CatBehaviour>();
                tempCat.isBlessingTriggered = true;
                tempCat.specialCatBlessingParticle.Play();
                CatBehaviour.specialCatBlessing += ShopRevenue.autoCustRate * 0.3f; //+ 30% auto customer rate 
                ShopRevenue.custPerSecUpdateReq++;

                switch (gemToBuy)
                {
                    case 100: CatSpawnManager.specialCatTimer = 86400f; break;
                    case 500: CatSpawnManager.specialCatTimer = 604800f; break;
                    case 1500: CatSpawnManager.specialCatTimer = 2419200f; break;
                    case 3000: CatSpawnManager.specialCatTimer = Mathf.Infinity; break;
                }
            }
            CatSpawnManager.catIndicatorTXT.transform.parent.gameObject.SetActive(true);
        }
    }

    IEnumerator confirmBlessingInvalid()
    {
        while (purchaseInvalid.result == -1)
            yield return null;

        if (purchaseInvalid.result == (int)confirmEnum.confirm)
        {
            //DO SOMETHING WHEN CONFIRM BUTTON IS CLICKED
            purchaseInvalid.resetResult();

        }
        else if (purchaseInvalid.result == (int)confirmEnum.cancel)
        {
            //DO SOMETHING WHEN CANCEL BUTTON IS CLICKED

            purchaseInvalid.resetResult();
        }
        purchaseInvalid.transform.localPosition = new Vector2(5000, 0);
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
            if (isInCatBlessingPackage)
                PlayerPrefs.SetInt("isInCatBlessingPackage", 1);
            else
                PlayerPrefs.SetInt("isInCatBlessingPackage", 0);
        }
    }

}
