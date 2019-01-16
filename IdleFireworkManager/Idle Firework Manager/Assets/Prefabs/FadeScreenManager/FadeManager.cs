using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public Animator fadingAnim;      //used for fading animation
    public Image fadeScreenImg;   

    public void fadeOut()
    {
        StartCoroutine(Fading());
    }

    IEnumerator Fading()
    {
        fadingAnim.SetBool("Fade", true);
        yield return new WaitUntil(() => fadeScreenImg.color.a == 1);
    }
}
