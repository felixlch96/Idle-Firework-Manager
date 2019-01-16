using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceBars : MonoBehaviour
{
    public GameObject chargeablePrefab;
    public GameObject obstaclePrefab;

	// Use this for initialization
	void Awake()
    {       
        //at first, start construct the whole playBar route
        for (int x = 0; x < 2; x++)
        {
            spawnBars(true);
        }

        //then, spawn at least 4~7 chargeable (white) bars 
        for (int x = 0; x < Random.Range(4,7); x++)
        {
            Instantiate(chargeablePrefab, this.transform);
        }
    }

    public void spawnBars(bool isInitialSpawn)
    {
        //when we initially spawn bars into the route when starting the mini-game, we want them to start at bottom
        if (isInitialSpawn)
        {
            int tempChargeable = Random.Range(4, 7);
            int tempObstacles = Random.Range(1, 3);
            while (tempChargeable > 0)
            {
                Instantiate(chargeablePrefab, this.transform);
                tempChargeable--;
            }
            while (tempObstacles > 0)
            {
                GameObject obj = Instantiate(obstaclePrefab, this.transform);
                //add all obstacles into the 'obstacleList' to check position and distance with consumingLine.
                tempObstacles--;
            }
        }
        //while when instantiate new bars at run-time, we want to instantiate them into first child in the hierarchy (top)
        else
        {
            int tempChargeable = Random.Range(3, 5);
            int tempObstacles = Random.Range(1, 3);
            while (tempObstacles > 0)
            {
                GameObject bar = Instantiate(obstaclePrefab, this.transform);
                bar.transform.SetAsFirstSibling();
                //add all obstacles into the 'obstacleList' to check position and distance with consumingLine.
                tempObstacles--;
            }

            while (tempChargeable > 0)
            {
                GameObject bar = Instantiate(chargeablePrefab, this.transform);
                bar.transform.SetAsFirstSibling();
                tempChargeable--;
            }            
        }        
    }
}
