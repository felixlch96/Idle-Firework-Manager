using UnityEngine;

public class IntroduceService : MonoBehaviour
{
    //creating firework services variables
    public FireworkServiceBehaviour[] fireworkServices; //to refer to the 3 services slot
    public Animator servicesCreatedAnim;

    //when a new firework service is introduced by a customer, 
    public GameObject serviceIndicator1;
    public GameObject serviceIndicator2;

    public void ToSpawnService()
    {
        //random a number and determine whether player get a firework service from a customer
        float tempService = Random.Range(0f, 100.0f);
        if (tempService <= 0.2) //0.2% to get a service
        {
            for (int x = 0; x < fireworkServices.Length; x++)
            {
                if (fireworkServices[x].isEmpty)
                {
                    serviceIndicator1.SetActive(true);
                    serviceIndicator2.SetActive(true);
                    fireworkServices[x].newService();
                    servicesCreatedAnim.SetTrigger("new");
                    break;
                }
            }
        }
    }
}
