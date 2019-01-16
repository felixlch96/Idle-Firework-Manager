using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingManager : MonoBehaviour
{
    public float scrollSpeed = 1;
    Vector2 startPos;

	// Use this for initialization
	void Start ()
    {
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float newPos = Mathf.Repeat(Time.time * scrollSpeed, 35);
        transform.position = startPos + Vector2.right * newPos;
	}
}
