using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelButtonState : MonoBehaviour
{    
    public Sprite normalState;
    public Sprite selectedState;
    public Button button;
    public bool isSelected = false;
    public GameObject panelToActive; //to activate it
    public UIManager UIManager;         //to access UIManager script's function, to de-activate all panel before activating respective panel

    Animator cameraAnim; //to move camera up and down when certain panels opened

    private void Start()
    {
        cameraAnim = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
    }

    public void ButtonClicked()
    {        
        if (!isSelected)
        {
            //hide all panels first before activating the panel
            UIManager.hideAllPanel();
            setState(true);        
        }
        else
        {
            setState(false);            
        }
                
    }

    public void setState(bool state)
    {
        isSelected = state;
        if (state)
        {
            //open up the panel
            UIManager.playBtnSFX(true);
            button.image.sprite = selectedState;
            if (panelToActive != null)
                panelToActive.SetActive(true);            
        }
        else
        {
            //close the panel
            UIManager.playBtnSFX(false);
            button.image.sprite = normalState;
            if (panelToActive != null)
                panelToActive.SetActive(false);            
        }
    }  

    //some panel need to trigger camera's animator 
    public void moveCamera()
    {
        if (!isSelected)
        {
            //if player is opening a new panel up, and the camera hasn't move up, move up
            if (cameraAnim.GetBool("up") == false)
                cameraAnim.SetBool("up", true);            
        }
        else
        {
            //if player is closing an opened panel down, move the camera down
            cameraAnim.SetBool("up", false);
        }
    }

    public void moveDownCamera()
    {
        //if player is closing an opened panel down, move the camera down
        cameraAnim.SetBool("up", false);
    }
}
