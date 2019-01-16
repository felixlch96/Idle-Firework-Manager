using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleThePrice : MonoBehaviour
{
    public ShopRevenue shop; //to access useGem function
    public static bool isDoublePrice = false;
    public static float doublePriceTimer = 0f; //used for counting down double price effect
    
    public GameObject doubleIndicator;
    public ConfirmationDialog purchaseInvalid; //the pop-up box that indicate user that player already has a doublePrice purchased, another purchase is not allowed
    enum confirmEnum { confirm, cancel }
    AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        //init doublePriceStat with player pref
        if (PlayerPrefs.HasKey("isDoublePrice"))
        {
            if (PlayerPrefs.GetInt("isDoublePrice") == 1)
            {
                if (GameManager.dateDifference.TotalSeconds >= PlayerPrefs.GetFloat("doublePriceTimer"))
                {
                    isDoublePrice = false;
                    doublePriceTimer = 0;
                }
                else
                {
                    isDoublePrice = true;
                    doublePriceTimer = PlayerPrefs.GetFloat("doublePriceTimer") - (float)GameManager.dateDifference.TotalSeconds;
                }                
            }
            else
            {
                isDoublePrice = false;
                doublePriceTimer = 0;
            }                
        }                         

        if (isDoublePrice)
            doubleIndicator.SetActive(true);
        else
            doubleIndicator.SetActive(false);
    }   

    public void BuyDoublePrice(int gemToBuy)
    {
        //if player has already purchased a double the price and currently in effect,
        //prompt message "purchase unsuccessful"
        if (isDoublePrice)
        {
            purchaseInvalid.transform.localPosition = new Vector2(0, 0);
            StartCoroutine(confirmDoubleInvalid());
        }
        else if (shop.useGem(gemToBuy))
        {
            audio.Play();
            isDoublePrice = true;
            doubleIndicator.SetActive(true);
            shop.revPerCustTXT.color = new Color(255, 246, 118);
            ShopRevenue.revPerCustUpdateReq++;
            switch (gemToBuy)
            {
                case 150: doublePriceTimer = 86400f; break;
                case 500: doublePriceTimer = 604800f; break;
            }
        }
    }

    IEnumerator confirmDoubleInvalid()
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
            if (isDoublePrice)
                PlayerPrefs.SetInt("isDoublePrice", 1);
            else
                PlayerPrefs.SetInt("isDoublePrice", 0);

            PlayerPrefs.SetFloat("doublePriceTimer", doublePriceTimer);
        }
    }
}
