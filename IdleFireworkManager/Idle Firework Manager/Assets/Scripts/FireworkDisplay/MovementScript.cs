using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float speed;
    public static float defaultSpeed = 2f;
    public static float normalAccSpd = 3f;
    public static float maxSpeed = 6f;

    // Use this for initialization
    void Start()
    {
        speed = defaultSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (MinigameManager.isGameStart)
        {         
             transform.Translate(0, -(speed * Time.deltaTime), 0);
        }
        
    }
}

