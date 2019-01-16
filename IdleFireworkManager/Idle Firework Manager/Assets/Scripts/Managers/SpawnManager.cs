using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    PoolManager poolManager;            //pool manager to store all the pooling enemies  

    public GameObject[] gameObjects;         // The game objects prefab to be spawned.
    public Transform spawnPoint;         // An array of the spawn points this enemy can spawn from.
    protected int gameObjectIndex;          // determine which game object to spawn based on the gameObject array length
    public static float numberToSpawn = 1;            //allow lucky tap facility to update this value

    private void Start()
    {
        poolManager = GetComponent<PoolManager>();

        for (int x = 0; x < gameObjects.Length; x++)
        {
            poolManager.CreatePool(gameObjects[x], 500);
        }     
    }

    public void TapSpawn()
    {
        //how many absolute customer should i spawn? (absolute numbers) e.g. 3
        int tempAbsCust = (int)numberToSpawn;
        //how possible can 1 extra customer be spawn? (float/decimal numbers) e.g. 0.86
        float tempExtraCust = (numberToSpawn % 1) * 100; //multiple 100 to convert to an absolute number for comparison later (e.g. 0.86 to 86)

        //spawn absolute customers first
        for (int x = 0; x < tempAbsCust; x++)
        {
            // Find a random customer object to spawn  
            gameObjectIndex = Random.Range(0, gameObjects.Length);
            //spawn multiple customer at once with slightly different x position (for visibility)
            Vector3 tempPos = spawnPoint.position;                
            tempPos.Set(spawnPoint.position.x - (x / 2.0f), spawnPoint.position.y, spawnPoint.position.z);
            poolManager.ReuseObject(gameObjects[gameObjectIndex], tempPos, spawnPoint.rotation);            
        }

        //random a number then determine whether to spawn the EXTRA customer
        int tempRandom = Random.Range(0, 100);
        if (tempRandom <= tempExtraCust)
        {
            // Find a random customer object to spawn  
            gameObjectIndex = Random.Range(0, gameObjects.Length);
            Vector3 tempPos = spawnPoint.position;
            tempPos.Set(spawnPoint.position.x - (tempAbsCust / 2.0f), spawnPoint.position.y, spawnPoint.position.z);
            poolManager.ReuseObject(gameObjects[gameObjectIndex], tempPos, spawnPoint.rotation);
        }       
    }

    public void AutoSpawn(float custRate)
    {
        int numToSpawn = (int)custRate;
        float extraCust = (custRate % 1) * 100;

        //spawn absolute numbers from the current shop's finalAutoCustRate, 
        //e.g. 2.1 -> 2 is the absolute numbers, thus spawn 2 customers
        for (int x = 0; x < numToSpawn; x++)
        {
            // Find a random customer object to spawn  
            gameObjectIndex = Random.Range(0, gameObjects.Length);
            //spawn multiple customer at once with slightly different x position (for visibility)
            Vector3 tempPos = spawnPoint.position;
            tempPos.Set(spawnPoint.position.x - (x / 2.0f), spawnPoint.position.y, spawnPoint.position.z);
            poolManager.ReuseObject(gameObjects[gameObjectIndex], tempPos, spawnPoint.rotation);
        }

        //random a number then determine whether to spawn the EXTRA customer
        int tempRandom = Random.Range(0, 100);
        if (tempRandom <= extraCust)
        {
            // Find a random customer object to spawn  
            gameObjectIndex = Random.Range(0, gameObjects.Length);
            Vector3 tempPos = spawnPoint.position;
            tempPos.Set(spawnPoint.position.x - (numToSpawn / 2.0f), spawnPoint.position.y, spawnPoint.position.z);
            poolManager.ReuseObject(gameObjects[gameObjectIndex], tempPos, spawnPoint.rotation);
        }

    }
}