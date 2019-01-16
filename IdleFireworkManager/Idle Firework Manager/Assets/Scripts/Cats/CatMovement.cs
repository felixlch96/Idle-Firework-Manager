using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMovement : MonoBehaviour
{
    float moveSpeed = 0.5f;
    Transform shop; //refer the shop position as a movement turning point for cats
    Animator catAnim;

    bool isGoingRight = true;
    bool isGoingLeft = false;
    [HideInInspector] public bool isDismiss = false;
    float dismissTimer = 120f;

	// Use this for initialization
	void Start ()
    {
        catAnim = gameObject.GetComponent<Animator>();
        shop = GameObject.FindGameObjectWithTag("Shop").transform;       
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (dismissTimer >= 0)
        {
            dismissTimer -= Time.deltaTime;            
        }
        else
        {
            dismissCatInstant();
        }

		if (isGoingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            //if cat reached right turning point
            if (transform.position.x >= shop.position.x + 1)
            {
                catAnim.SetTrigger("left");
                isGoingLeft = true;
                isGoingRight = false;
            }
        }
        else if (isGoingLeft)
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            //if cat reached left turning point
            if (transform.position.x <= shop.position.x - 3)
            {
                catAnim.SetTrigger("right");
                isGoingLeft = false;
                isGoingRight = true;
            }
        }
        else if(isDismiss)
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            catAnim.SetTrigger("left");
            if (transform.position.x <= shop.position.x - 5) //offscreen jor
            {
                Destroy(gameObject);
            }
        }
    }

    //this method is invoked in dismissCat routine after the dismiss timer run off, 
    //and for catBehaviour script that upon player tap on cat and chose ignore the ads offer, the cat will dismiss rightaway
    public void dismissCatInstant()
    {
        CatBehaviour tempCat = gameObject.GetComponent<CatBehaviour>();

        if (tempCat.isSpecialCat && tempCat.isBlessingTriggered) { }
        else
        {
            moveSpeed = 1f;
            isDismiss = true;
            isGoingRight = false;
            isGoingLeft = false;
        }
    }
}
