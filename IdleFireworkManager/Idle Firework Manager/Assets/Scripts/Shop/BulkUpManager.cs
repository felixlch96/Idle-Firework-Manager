using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BulkUpManager : MonoBehaviour
{
    public static int[] bulkValue = { 1, 10, 100 };
    static int bulkIndex = 0;
    public static int currentBulk;

    public TextMeshProUGUI bulkTxt;

    private void Awake()
    {
        //init the bulk status from last setting
        if (PlayerPrefs.HasKey("bulkIndex"))
        {
            bulkIndex = PlayerPrefs.GetInt("bulkIndex");            
        }
        currentBulk = bulkValue[bulkIndex];
    }

    private void Update()
    {
        bulkTxt.SetText(currentBulk.ToString());
    }

    public void BulkBtnClicked()
    {
        bulkIndex++;
        if (bulkIndex == bulkValue.Length)
            bulkIndex = 0;

        currentBulk = bulkValue[bulkIndex];
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
            PlayerPrefs.SetInt("bulkIndex", bulkIndex);
        }
    }

    public static float bulkMultiplier(float costToUpgrade, float increment, int num)
    {
        //calc finalCostToUpgrade based on currentBulk value
        float finalCostToUpgrade = 0;
        for (int x = 0; x < num; x++)
        {
            finalCostToUpgrade += costToUpgrade;
            costToUpgrade *= increment;        
        }

        return finalCostToUpgrade;
    }
}
