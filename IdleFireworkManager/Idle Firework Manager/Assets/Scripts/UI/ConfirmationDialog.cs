using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationDialog : MonoBehaviour
{
    public int result = -1;
    enum confirmEnum { confirm, cancel }
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void confirmBtnClicked()
    {
        result = (int)confirmEnum.confirm;
    }

    public void cancelBtnClicked()
    {
        result = (int)confirmEnum.cancel;
    }

    public void resetResult()
    {
        result = -1;
    }    
}

/* HOW TO USE------------------------------------------------------------
 * ConfirmationDialog confirmation;
 * enum confirmEnum { confirm, cancel } 
 * 
 * IEnumerator WaitingConfirmationForSomething()
    {              
        while (confirmation.result == -1)                  
            yield return null;
       
        if (confirmation.result == (int)confirmEnum.confirm)
        {
            //DO SOMETHING WHEN CONFIRM BUTTON IS CLICKED

            confirmation.resetResult();

        }
        else if (confirmation.result == (int)confirmEnum.cancel)
        {
             //DO SOMETHING WHEN CANCEL BUTTON IS CLICKED

            confirmation.resetResult();
        }
    }
    */
