using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAnimationController : MonoBehaviour
{
    Animator lineAnim;

    // Use this for initialization
    void Start()
    {
        lineAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //if player hold onto screen
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(0))
        {
            lineAnim.SetTrigger("hold");
        }
	}
}
