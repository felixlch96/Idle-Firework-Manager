using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using EZCameraShake;

public class MinigameManager : MonoBehaviour
{
    public static bool isPlayed = false; //to allow other scripts to refer, stating that whether player had displayed fireworks this particular night 
    public static bool isGameStart = false;
    float timer = 30.0f;
    float score = 0;
    public static float totalAutoCustBoost = 0;
    public static int totalFireworkUnlocked = 1;

    //Objects references-------------------------------------------------
    public MovementScript playBar;
    public GameObject consumingLine;
    public Animator dangerIndicator;
    public Animator greatIndicator;
    public Animator perfectIndicator;
    Animator obstacleBlinking;
    public TextMeshProUGUI timerTxt;
    public TextMeshProUGUI timesUpTxt;
    public TextMeshProUGUI endPanelTxt;
    GameObject endPanel;
    Animator timesUpAnim;

    public GameObject[] fireworkParticles; //storing all fireworks prefab object
    public ParticleSystem[] accelerateParticles;
    //--------------------------------------------------------------------

    enum accelerateParticleToPlay { normal, great, perfect, danger }
    enum distanceRating { normal, perfect, great, danger }
    enum fingerAction { release, hold }    

    public FadeManager fadeManager;

    private void Awake()
    {
        for (int x = 0; x < accelerateParticles.Length; x++)
            if (accelerateParticles[x].isPlaying)
                accelerateParticles[x].Stop();

        if (PlayerPrefs.HasKey("minigameAutoCustBoost"))
            totalAutoCustBoost = PlayerPrefs.GetFloat("minigameAutoCustBoost");
    }

    // Use this for initialization
    void Start()
    {
        endPanel = endPanelTxt.transform.parent.gameObject;
        timesUpAnim = timesUpTxt.gameObject.GetComponent<Animator>();        
    }

    // Update is called once per frame
    void Update()
    {
        timerTxt.SetText(timer.ToString("F2").Replace(".", ":"));
        if (isGameStart)
        {
            //display any performance indicator (e.g. Perfect! Great! Oh no! if there is any)
            userInput();

            //if there are still time left, countdown, and continue play mini-game
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            //else if times up, game over and return reward back to main gameplay scene
            else
            {
                timer = 0;
                isGameStart = false;
                timesUpTxt.gameObject.SetActive(true);
                timesUpAnim.SetTrigger("timesUp");
                StartCoroutine(displayEndPanel());
            }                     
        }        
    }
    
    //this function is to check if the consumingLine is actually very near one of the obstacleList's obj
    public int checkNearDistance(int action)
    {
        //calculate whether consumingLine is within the obstacle first        
        if (LineCollision.isCollidingObs)        
        {
            return (int)distanceRating.danger;
        }
        else
        {
            //PERFECT DISTANCE (very very near)!
            //if (Mathf.Abs(Mathf.Abs(pos1) - pos2) <= 20)
            if (LineCollision.isCollidingPerfect)
            {
                return (int)distanceRating.perfect;
            }

            //GREAT DISTANCE (near)!
            //else if (Mathf.Abs(Mathf.Abs(pos1) - pos2) <= 40)
            else if (LineCollision.isCollidingGreat)
            {
                return (int)distanceRating.great;
            }
            else
            {
                score += Time.deltaTime;
                return (int)distanceRating.normal;
            }
        }
    }

    void userInput()
    {
        if (Input.GetMouseButton(0))
            //DANGER! player hold within the obstacle
            if (checkNearDistance((int)fingerAction.hold) == (int)distanceRating.danger)
            {
                for (int x = 0; x < accelerateParticles.Length; x++)
                    if (accelerateParticles[x].isPlaying)
                        accelerateParticles[x].Stop();

                playBar.speed = MovementScript.defaultSpeed;
                accelerateParticles[(int)accelerateParticleToPlay.danger].Play();
                accelerateParticles[(int)accelerateParticleToPlay.danger].GetComponent<AudioSource>().Play();
                if (score > 10)
                    score -= 10;
                CameraShaker.Instance.ShakeOnce(2.5f, 1f, .1f, .1f);
                LineCollision.latestCollidedObs.SetTrigger("danger");
                dangerIndicator.SetTrigger("performance");
                return;
            }

        //if player is HOLDING mouse/finger onto the screen, 
        if (Input.GetMouseButtonDown(0))
        {
            for (int x = 0; x < accelerateParticles.Length; x++)
                if (accelerateParticles[x].isPlaying)
                    accelerateParticles[x].Stop();

            //PERFECT HOLD AFTER AN OBSTACLE!
            if (checkNearDistance((int)fingerAction.hold) == (int)distanceRating.perfect)
            {
                //increase speed by 40%
                if (playBar.speed <= MovementScript.maxSpeed)
                    playBar.speed *= 1.4f;

                score += 20;
                accelerateParticles[(int)accelerateParticleToPlay.perfect].Play();
                accelerateParticles[(int)accelerateParticleToPlay.perfect].GetComponent<AudioSource>().Play();
                perfectIndicator.SetTrigger("performance");

            }

            //GREAT HOLD AFTER AN OBSTACLE!
            else if (checkNearDistance((int)fingerAction.hold) == (int)distanceRating.great)
            {
                //increase speed by 20%
                if (playBar.speed <= MovementScript.maxSpeed)
                    playBar.speed *= 1.2f;

                accelerateParticles[(int)accelerateParticleToPlay.great].Play();
                accelerateParticles[(int)accelerateParticleToPlay.great].GetComponent<AudioSource>().Play();
                score += 10;
                greatIndicator.SetTrigger("performance");

            }
            else
            {
                //normal charging will slightly slow down the playBar if the speed is higher than default speed
                if (playBar.speed > MovementScript.normalAccSpd)
                    playBar.speed *= 0.97f;
                else
                    playBar.speed = MovementScript.normalAccSpd;
                accelerateParticles[(int)accelerateParticleToPlay.normal].Play();
                accelerateParticles[(int)accelerateParticleToPlay.normal].GetComponent<AudioSource>().Play();
            }

        }
        //===========================================================================================================================

        //if player RELEASING their button/finger from screen,
        if (Input.GetMouseButtonUp(0))
        {
            for (int x = 0; x < accelerateParticles.Length; x++)
                if (accelerateParticles[x].isPlaying)
                    accelerateParticles[x].Stop();

            accelerateParticles[(int)accelerateParticleToPlay.normal].GetComponent<AudioSource>().Stop();
            accelerateParticles[(int)accelerateParticleToPlay.great].GetComponent<AudioSource>().Stop();
            accelerateParticles[(int)accelerateParticleToPlay.perfect].GetComponent<AudioSource>().Stop();

            if (checkNearDistance((int)fingerAction.release) == (int)distanceRating.danger) { } //this line is added to prevent launching any fw   
            //PERFECT RELEASE BEFORE AN OBSTACLE, SPEED UP LIL BIT
            else if (checkNearDistance((int)fingerAction.release) == (int)distanceRating.perfect)
            {
                playBar.speed *= 1.1f;
                score += 20;
                perfectIndicator.SetTrigger("performance");
                playFirework(2);
            }
            //GREAT RELEASE BEFORE AN OBSTACLE, SPEED UP SUPER LIL BIT
            else if (checkNearDistance((int)fingerAction.release) == (int)distanceRating.great)
            {
                playBar.speed *= 1.05f;
                score += 10;
                score += 10;
                greatIndicator.SetTrigger("performance");
                playFirework(1);
            }

            else
            {
                if (playBar.speed > MovementScript.normalAccSpd)
                    playBar.speed *= 0.97f;
                else
                    playBar.speed = MovementScript.defaultSpeed;
            }
        }
    }

    IEnumerator displayEndPanel()
    {
        yield return new WaitForSeconds(2f);

        float reward = score / 1000;
        if (reward <= 0)
            reward = 0;

        if (score < 100)
            endPanelTxt.SetText("Try harder next time! You've earned " + reward.ToString("F2") + " auto customer rate");
        else if (score < 300)
            endPanelTxt.SetText("Not bad! You've earned " + reward.ToString("F2") + " auto customer rate");
        else if (score < 500)
            endPanelTxt.SetText("Well Played! You've earned " + reward.ToString("F2") + " auto customer rate");
        else if (score >= 500)
            endPanelTxt.SetText("Perfect! You've earned " + reward.ToString("F2") + " auto customer rate");

        endPanel.SetActive(true);
        totalAutoCustBoost += reward;
        ShopRevenue.custPerSecUpdateReq++;
        isPlayed = true;
    }

    public void backToMain()
    {        
        PlayerPrefs.SetFloat("minigameAutoCustBoost", totalAutoCustBoost);        

        GameObject objToDestroy = GameObject.FindGameObjectWithTag("Mini GM");
        Destroy(objToDestroy);

        GameObject obj = GameObject.FindGameObjectWithTag("Main GM");
        //deactivate all child before into scene
        for (int x = 0; x < obj.transform.childCount; x++)
            obj.transform.GetChild(x).gameObject.SetActive(true);
    }

    void playFirework(int count)
    {
        for (int x = 0; x < count; x++)
        {
            //play random fireworks
            int temp = Random.Range(0, totalFireworkUnlocked - 1);
            GameObject fwObject = Instantiate(fireworkParticles[temp], this.transform);
            ParticleSystem fwParticle = fwObject.GetComponent<ParticleSystem>();
            fwParticle.Play();
        }
    }
}
