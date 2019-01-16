using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Animator cameraAnim; //to move camera up and down when certain panels opened

    // Use this for initialization
    void Start ()
    {
        cameraAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
