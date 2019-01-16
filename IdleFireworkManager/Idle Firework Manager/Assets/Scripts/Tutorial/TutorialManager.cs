using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static bool isFirstTime = true;
    public GameObject tapTutorialImg;
    Animator tapTutorialAnim;

    //update help description box and prompt
    public TextMeshProUGUI helpTxt;

	// Use this for initialization
	void Start ()
    {
        //initialize whether player is first time login to the game, if no, destroy the tapTutorialImg
        if (PlayerPrefs.HasKey("isFirstTime") && PlayerPrefs.GetInt("isFirstTime") == 0)
            isFirstTime = false;
        
        if (isFirstTime)
        {
            tapTutorialAnim = tapTutorialImg.GetComponent<Animator>();            
        }

        else
            tapTutorialImg.SetActive(false);
	}    

    private void Update()
    {        
        if (tapTutorialImg != null && tapTutorialImg.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                tapTutorialAnim.SetTrigger("close");
                StartCoroutine(deleteObjAfterSec(tapTutorialImg, 1));
            }
        }        
    }

    IEnumerator deleteObjAfterSec(GameObject obj, float sec)
    {
        yield return new WaitForSeconds(sec);
        Destroy(obj);
    }

    private void OnApplicationQuit()
    {
        OnApplicationPause(true);
    }

    private void OnApplicationPause(bool pause)
    {        
        if (pause)
        {
            if (isFirstTime)
                PlayerPrefs.SetInt("isFirstTime", 0);            
        }
    }

    //ALL pre-defined functions for setting helpTxt, invoked by UI from all over the world
    public void facilityHelp()
    {
        helpTxt.SetText("<size=40><color=black><u>Facilities</u></color></size>\nUpgrade your shop's facilities to maximize revenue generation!\n\n<color=#C8473FFF>Lucky Tap</color> - increase number of customer invitied per tap.\n<color=#C8473FFF>Signboard</color> - decrease the amount of time taken to spawn customer automatically.\n<color=#C8473FFF>Cashier Register</color> - increase tips percentage\n\nEach 100lv will increase facilities' <color=#FF8F00FF>TIER</color>, shop's tier will upgrade if all facilities reach the same tier.");
    }

    public void fireworkHelp()
    {
        helpTxt.SetText("<size=40><color=black><u>Fireworks</u></color></size>\nUnlock and upgrade firework products to maximize revenue per customer!\n\nAll <color=orange>unlocked</color> fireworks will have the chance to appear during <color=#00B4ADFF>Firework Display</color> at night time.\n\n\n<color=black><u>Firework Display</color></u>\nFirework Display mini-game will be available during <color=red>8pm~6am</color> everyday.");
    }

    public void workerHelp()
    {
        helpTxt.SetText("<size=40><color=black><u>Worker</u></color></size>\n<size=20>You can assign worker to shop, and firework services.\nWorker working in the shop can help to generate <color=red>offline revenue</color> while player is not in the game.</size>\n\nThere are 4 <u>types</u> of worker you can get:\n<size=20><color=orange>Beggar</color>, <color=orange>Face of the Group</color>, <color=orange>Businessman</color>, <color=orange>Robot</color></size>\nThey come in 4 different <u>ranks</u>:\n<color=#6CF996FF>Common</color>, <color=#7B71F0FF>Rare</color>, <color=#B552FFFF>Epic</color>, <color=#FFD800FF>Legendary</color>\n\nAnd they has unique passive skill to maximize your shop revenues!\n");
    }

    public void serviceHelp()
    {
        helpTxt.SetText("<size=40><color=black><u>Firework Services</u></color></size>\nServices takes time to complete, and big rewards in return. You have to send one of your worker to do the job!\n<color=#1D5815FF><size=20>The longer duration of the service, the better rewards!</size></color>\n\n<color=orange>Businessman</color> type worker can boost the service completion rewards tremendously.\n\n<color=black><u>How to get services?</u></color>\nTap tap tap until one of the customer introduce to you.");
    }

    public void timeHelp()
    {
        helpTxt.SetText("<size=40><color=black><u>Time</u></color></size>\nThe day and night cycle follows your phone system time.\n<color=#15580CFF><size=20>Different activities available in differerent period.</size></color>\n\n<color=#333CEEFF>7:00PM - 6:59AM: </color><color=black>Firework Display</color>\n<size=20> play a firework display mini-game to gain auto customer rate permanently.</size>\n\n<color=#FF5F33FF>7:00AM - 6:59PM:</color><color=black> Cats Spawning</color>\n<size=20>Cats will spawn randomly and give blessing to player upon interaction.</size>\n<color=#858585FF><size=20>Note: Cats purchased from market will ignore time. </size></color>");
    }

    public void HowToPlay()
    {
        helpTxt.SetText("<size=40><color=black><u>How To Play?</u></color></size>\n\n<color=#15580CFF><size=20>ULTIMATE GOAL: Grow your shop, make money!</size></color>\n\n<color=red>Tap Tap Tap!</color> Invite as many customer as you can to your shop to buy your fireworks! Some of them give you <color=orange>Tips</color> or even <color=#FF0045FF>Gems</color>!\n<color=red>Upgrade</color> your shop's facilities and fireworks to maximize the revenue.\n<color=red>Hire</color> the best worker to help in your shop and firework services.\n<color=red>Pay attention to time!</color> Some activity only available at day or night time.");
    }
}
