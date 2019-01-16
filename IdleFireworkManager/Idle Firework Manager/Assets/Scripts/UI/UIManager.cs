using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PanelButtonState[] panelBtnState;
    public GameObject[] panelToHide;    

    AudioSource audio;
    public AudioClip positiveBtnSFX;
    public AudioClip negativeBtnSFX;    

    // Use this for initialization
    void Start ()
    {
        audio = GetComponent<AudioSource>();
    }	

    //to hide all active panel, this function will be invoked if player clicked else where while a panel is active, or clicking another panelBtn
    public void hideAllPanel()
    {
        //reset all panelButton state
        for (int x = 0; x < panelBtnState.Length; x++)
        {            
            if (panelBtnState[x].isSelected)            
                panelBtnState[x].setState(false);
        }

        //hide all panels
        for (int x = 0; x < panelToHide.Length; x++)
        {
            if (panelToHide[x].activeSelf)
                panelToHide[x].SetActive(false);
        }
    }

    public void playBtnSFX(bool open)
    {
        if (open)
        {
            audio.clip = positiveBtnSFX;
        }
        else
        {
            audio.clip = negativeBtnSFX;
        }
        audio.Play();
    }
       
}
