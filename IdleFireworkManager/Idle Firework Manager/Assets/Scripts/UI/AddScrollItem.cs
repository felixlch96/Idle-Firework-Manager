using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddScrollItem : MonoBehaviour
{
    public GameObject itemTemplate;
    public GameObject content;

   
	public void AddButtonClicked()
    {
        var copy = Instantiate(itemTemplate);
        copy.transform.parent = content.transform;
    }
}
