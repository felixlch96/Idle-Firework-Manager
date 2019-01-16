using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    AudioSource[] audio;
    enum audioSource { BGM, Ambient }

    public AudioClip dayBGM;
    public AudioClip birdSFX;

    public AudioClip nightBGM;
    public AudioClip cricketSFX;    

    // Use this for initialization
    void Start ()
    {
        audio = GetComponents<AudioSource>();

        //if the game is currently "looks like" night time
        if ((DayNightCycle.time >= 0 && DayNightCycle.time <= 25000) || (DayNightCycle.time >= 72000 && DayNightCycle.time <= 86400))
        {
            //play nightBGM and cricket SFX
            audio[(int)audioSource.BGM].clip = nightBGM;
            audio[(int)audioSource.Ambient].clip = cricketSFX;            
        }
        else
        {
            //play dayBGM and bird SFX
            audio[(int)audioSource.BGM].clip = dayBGM;
            audio[(int)audioSource.Ambient].clip = birdSFX;
        }

        for (int x = 0; x < audio.Length; x++)
            audio[x].Play();
    }
	
}
