using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonState : MonoBehaviour
{
    public Sprite normalState;
    public Sprite insufficientState;   
    public bool isSufficient = false;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //if player's available cash flow can afford this upgrade, set this upgrade button to available to click.
		if (isSufficient)
        {
            this.GetComponent<Button>().image.sprite = normalState;
        }
        else
        {
            this.GetComponent<Button>().image.sprite = insufficientState;
        }
	}  
}
