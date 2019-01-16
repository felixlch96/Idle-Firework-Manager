using System.Collections;
using UnityEngine;
using System;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    public static float time;
    public Transform sunTransform; //referring sun's position (we need to rotate it) THE DIRECTIONAL LIGHT!
    public Light sun; //ALSO THE DIRECTIONAL LIGHT! we need to adjust the intensity of it
    public int days; //to keep track how many days has passed
    public TextMeshProUGUI clockTxt; //to constantly update the clock text

    public float intensity; //calculation purpose variable
    public int speed = 1; //speed of the day changing (e.g. speed 3 = 8 hours of a complete day)
	
    //night activities variables
	public GameObject fireworkDisplayBtn;
    Animator fireworkDisplayAnim;

    //game object activation based on different time (day or night)
    public Transform SunMoonObj; //rotate the sunmoon sprite
    public GameObject cloudsObj; //clouds appear at day time only
    public ParticleSystem stars; //stars appear at night time only
    public Light shopNightLight;
    

    //notification large icon images
    public Texture2D nightTimeIcon;
    

    private void Awake()
    {
        //initialize the time (require GameManager time variables to simulate a real "time" system)
        if (PlayerPrefs.HasKey("time"))
        {
            float timeToAdd = (float)GameManager.dateDifference.TotalSeconds * speed;

            if (timeToAdd > 20000)
                MinigameManager.isPlayed = false;

            while (timeToAdd > 86400)
                timeToAdd -= 86400;

            time = PlayerPrefs.GetFloat("time") + timeToAdd;
        }
    }

    private void Start()
    {
        fireworkDisplayAnim = fireworkDisplayBtn.GetComponent<Animator>();

        if (PlayerPrefs.HasKey("fwDisplayIsPlayed"))
        {
            if (PlayerPrefs.GetInt("fwDisplayIsPlayed") == 1)
                MinigameManager.isPlayed = true;
            if (PlayerPrefs.GetInt("fwDisplayIsPlayed") == 0)
                MinigameManager.isPlayed = false;
        }

        //if player login at day-time, straight away start the shop light with 0 intensity
        if (time > 15000 && time < 70000) //day time
        {
            shopNightLight.intensity = 0;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        ChangeTime();
        ChangeEnvironment();

        //calculate time format for clock
        int hour = (int)time / 3600;        
        int minutes = (int)(time % 3600) / 60;

        //update clock txt
        if (hour < 12) //12am~12:59pm
        {
            if (hour == 0) { hour = 12; } //do not display as 00:xx am, instead, display 12:xx am
                clockTxt.SetText(hour.ToString() + ":" + minutes.ToString("00") + "am");
        }            
        else //hour > 12, 12pm ~ 11:59pm
        {
            if (hour >= 13)
                hour -= 12; //convert hour to am pm format instead of 0000hrs
            clockTxt.SetText(hour.ToString() + ":" + minutes.ToString("00") + "pm");
        }

        //PC input to simulate faster day and night cycle
        if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.P))
            time += 500;
        if (Input.GetKey(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.O))
            time -= 500;
            
    }

    void ChangeTime()
    {
        time += Time.deltaTime;
        if (time > 86400) //86400 seconds = 24 hours (1day)
        {
            days += 1;
            time = 0;
        }

        //rotate the directional light based on time
        //90 = indicate the actual rotating speed of your directional light object.
        //i found that 360 gave too much "dark" time to the game, 3D games need 360 to simulate real day night cycle, however 90 works perfect for my 2D game
        sunTransform.rotation = Quaternion.Euler(new Vector3((time - 21600) / 86400 * 90, 0, 0));
        
        //adjust sun's light intensity based on the time
        if (time < 43200)
            intensity = 1 - (43200 - time) / 43200;
        else
            intensity = 1 - ((43200 - time) / 43200 * -1);

        sun.intensity = intensity;

        //DAY TIME, CAT CAT CAT!
        //spawning cat methods will be done at CatSpawnManager script

        //NIGHT TIME, FIREWORK DISPLAY MINI-TIME!
        if (!MinigameManager.isPlayed)
        {
            if ((time >= 0 && time <= 25199) || (time >= 68400 && time <= 86400)) //7pm ~ 7am
            {
                //set active(true) for firework display button
                if (!fireworkDisplayBtn.activeSelf)
                {
                    fireworkDisplayBtn.SetActive(true);
                    fireworkDisplayAnim.SetTrigger("available");
                }
            }
            else //if it is not at night time
            {
                if (fireworkDisplayBtn.activeSelf)
                {
                    fireworkDisplayAnim.SetTrigger("unavailable");
                    StartCoroutine(hideObj(fireworkDisplayBtn));
                }
            }
        }
        else if (MinigameManager.isPlayed)
        {
            if (fireworkDisplayBtn.activeSelf)
                StartCoroutine(hideObj(fireworkDisplayBtn));

            if (time > 25200 && time < 68400) //day time, reset mini game availability, prompt again on next night time
                MinigameManager.isPlayed = false;
        }
    }

    void ChangeEnvironment()
    {
        //for calculation reference: 3600seconds = 1 hour, 21600 = 6 hours*********       
        //==================================================================================

        //change background image (clouds at day? stars at night?)
        if (time > 15000 && time < 68000) //day time
        {
            cloudsObj.SetActive(true);
            stars.Stop();

            if (shopNightLight.intensity > 0)
                shopNightLight.intensity -= 0.1f;
        }

        else
        {
            cloudsObj.SetActive(false);
            stars.Play();

            if (shopNightLight.intensity < 6)
                shopNightLight.intensity += 0.1f;
        }

        //change sun&moon position
        //86400 / 240 = 360. subtract 90 is because starting point 
        //-90 = moon right on top of the background, -270 = sun right on top of the background
        SunMoonObj.rotation = Quaternion.Euler(new Vector3(0, 0, (-(time / 240) - 90)));        
    }    

    IEnumerator hideObj(GameObject obj)
    {
        yield return new WaitForSeconds(1);
        obj.SetActive(false);
    }
    
    
    private void OnApplicationQuit()
    {
        OnApplicationPause(true);


    }
    private void OnDestroy()
    {
        OnApplicationPause(true);

    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PlayerPrefs.SetFloat("time", time);
            if (MinigameManager.isPlayed)
                PlayerPrefs.SetInt("fwDisplayIsPlayed", 1);
            else
                PlayerPrefs.SetInt("fwDisplayIsPlayed", 0);

            if (SettingsConfig.hasNotification)
            {
                //if it is day time currently when player log off, set notification for night time later
                if (time > 25200 && time < 68399)
                {
                    float delayTime = 68400 - time;
                    NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds(delayTime), "It's night time!", "Display firework and gain more customers!", new Color(1, 1, 1));                                       
                }
            }                        
        }
    }
}

