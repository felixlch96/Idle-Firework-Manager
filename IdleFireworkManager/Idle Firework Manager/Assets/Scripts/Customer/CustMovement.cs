using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustMovement : MonoBehaviour
{
    float moveSpeed = 3f; //customer's movement speed
    public const float officialWaitTime = 2.0f; //international customer transaction time
    float waitCountTimer; //used for counting down customer transaction time
    bool isReachDoor = false; //is customer already reached the door?
    bool isLeaving = false; //is customer done transaction and leaving?

    Transform offscreenPoint; //to kill customer object back into pool after out of screen
    Transform shop; //to refer the actual shop object's position
    ShopRevenue revDetails; //to refer and call shopRevenue script's function - SuccessCust 
    Animator anim; //to trigger different animation of customer 
    SpriteRenderer custSprite;

    // Use this for initialization
    void Start ()
    {
        custSprite = gameObject.GetComponent<SpriteRenderer>();
        waitCountTimer = officialWaitTime;
        anim = GetComponent<Animator>();       
        shop = GameObject.FindGameObjectWithTag("Shop").transform;
        offscreenPoint = GameObject.FindGameObjectWithTag("KillPoint").transform;
        revDetails = shop.GetComponent<ShopRevenue>();        
    }
	
	// Update is called once per frame
	void Update ()
    {
        //ensure customer sprite alpha value will revert back to 1 after leaving shop
        if (isLeaving)
        {
            if (custSprite.color.a < 1)
                custSprite.color += new Color(0, 0, 0, Time.deltaTime); //multiply by 5 because i want the customer to revert back faster

            //set customer sprite to be behind the shop
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0.05f);
        }
            

        //if customer havent reach shop's door, walk to there
        if (isReachDoor == false)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);

            if (transform.position.x >= (shop.position.x - 0.3) && isLeaving == false)
            {                            
                isReachDoor = true;
                anim.SetBool("wait", true);    
            }
        }		

        //if customer reach shop's door, change to idle animation, and start counting down to leave
        if (isReachDoor)
        {
            waitCountTimer -= Time.deltaTime;
            if (custSprite.color.a >= 0)
                custSprite.color -= new Color(0, 0, 0, Time.deltaTime);

            //if done count down, customer leave, successful customer transaction!
            if (waitCountTimer <= 0)
            {
                waitCountTimer = officialWaitTime;
                anim.SetBool("wait", false);
                isReachDoor = false;
                isLeaving = true;
                revDetails.SucessCust();
            }
        }

        if (transform.position.x >= offscreenPoint.transform.position.x)
        {
            //reset customer stat
            isReachDoor = false; 
            isLeaving = false; 

            gameObject.SetActive(false);
        }
	}    
}
