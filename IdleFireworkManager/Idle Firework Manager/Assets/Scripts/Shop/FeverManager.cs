using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeverManager : MonoBehaviour
{
    public static float FeverBoostEffect;
    bool isFever = false;
    public Image feverGauge;
    public int[] tapThreshold;
    public float[] boostEffect;
    int feverLevelIndex;
    public int feverDuration;    
    public GameObject[] feverEffectObj;
    public TextMeshProUGUI feverTxt;
    ShopRevenue shop;
    AudioSource audio;
    // Use this for initialization
    void Start()
    {
        shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<ShopRevenue>();
        audio = GetComponent<AudioSource>();
        feverLevelIndex = 0;
        FeverBoostEffect = 0;
    }

    private void Update()
    {
        if (isFever)
        {
            feverGauge.fillAmount -= Time.deltaTime / feverDuration;

            if (feverGauge.fillAmount <= 0)
            {
                isFever = false;
                audio.Stop();
                FeverBoostEffect = 0;
                ShopRevenue.revPerCustUpdateReq++;
                for (int x = 0; x < feverEffectObj.Length; x++)
                    feverEffectObj[x].SetActive(false);

                //if the current fever level is already last level, repeat the same fever level
                if (feverLevelIndex == tapThreshold.Length - 1) { }
                else feverLevelIndex++;
            }
        }
    }

    public void TapCharge()
    {
        if (!isFever)
        {
            feverGauge.fillAmount += (1.0f / tapThreshold[feverLevelIndex]); //e.g. threshold = 200 (1st level), then 1 tap will fill 0.01 amount
            if (feverGauge.fillAmount == 1)
            {
                //trigger fever! 
                isFever = true;
                audio.Play();
                ShopRevenue.revPerCustUpdateReq++;
                FeverBoostEffect = boostEffect[feverLevelIndex] / 100;
                feverTxt.SetText("Fever Boost!\n<size=100>" + ((int)boostEffect[feverLevelIndex]).ToString() + "%</size>");
                for (int x = 0; x < feverEffectObj.Length; x++)
                    feverEffectObj[x].SetActive(true);
            }
        }    
    }
}
