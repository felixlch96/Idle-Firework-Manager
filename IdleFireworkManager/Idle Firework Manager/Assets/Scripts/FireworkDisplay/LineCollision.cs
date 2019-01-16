using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LineCollision : MonoBehaviour
{
    public static bool isCollidingObs;
    public static bool isCollidingPerfect;
    public static bool isCollidingGreat;
    public static Animator latestCollidedObs;

    public ProduceBars produceBar;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "perfectZone")
        {
            isCollidingPerfect = true;
            isCollidingGreat = false;
            isCollidingObs = false;
        }
        else if (collision.gameObject.tag == "greatZone")
        {
            isCollidingPerfect = false;
            isCollidingGreat = true;
            isCollidingObs = false;
        }
        else if (collision.gameObject.tag == "obstacle")
        {
            isCollidingPerfect = false;
            isCollidingGreat = false;
            isCollidingObs = true;
            latestCollidedObs = collision.transform.Find("ObstacleBlink").GetComponent<Animator>();

            //instantiate new set of route
            produceBar.spawnBars(false);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "perfectZone")
            isCollidingPerfect = false;
        else if (collision.gameObject.tag == "greatZone")
            isCollidingGreat = false;
        else if (collision.gameObject.tag == "obstacle")
            isCollidingObs = false;      
    }
    
}
