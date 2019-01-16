using UnityEngine;
using TMPro;

public class GemsToCash : MonoBehaviour
{
    public ShopRevenue shop;
    public TextMeshProUGUI toCashTXT; //update the buy button 

    AudioSource audio;

    float cashToExchg;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }
    private void Update()
    {
        cashToExchg = ShopRevenue.highestCashRecord * 20;
        toCashTXT.SetText(BigNumManager.BigNumString(cashToExchg));
    }

    public void ExchangeBtnClicked(int gemToBuy)
    {
        if (shop.useGem(gemToBuy))
        {
            audio.Play();
            shop.indieCash += cashToExchg;
        }
    }
}
