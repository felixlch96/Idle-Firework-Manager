using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CountDownTimer : MonoBehaviour
{
    public Transform loadingBar;
    public TextMeshProUGUI countdownTxt;
    public float currentAmount;
    Animator timerAnim;

    bool startCountdown = false;
	// Use this for initialization
	void Start ()
    {
        timerAnim = gameObject.GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
        if (startCountdown)
        {
            if (currentAmount > 1)
            {
                currentAmount -= Time.deltaTime;
                countdownTxt.SetText(((int)currentAmount).ToString());
            }
            else
            {
                timerAnim.SetTrigger("hide");
                MinigameManager.isGameStart = true;
                StartCoroutine(killSelf());
            }
            loadingBar.GetComponent<Image>().fillAmount += Time.deltaTime;
            if (loadingBar.GetComponent<Image>().fillAmount >= 1)
                loadingBar.GetComponent<Image>().fillAmount = 0;
        }        
    }

    IEnumerator killSelf()
    {
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }

    public void setCountdown()
    {
        startCountdown = true;
    }
}
