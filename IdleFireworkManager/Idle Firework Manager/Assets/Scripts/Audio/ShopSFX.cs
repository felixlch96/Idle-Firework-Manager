using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSFX : MonoBehaviour
{
    AudioSource audio;
    
    public AudioClip tipsSFX;
    public AudioClip shopOpenSFX;

    // Use this for initialization
    void Start ()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = shopOpenSFX;
        audio.Play();
        audio.volume = 0.1f;
	}	

    public void playTips()
    {
        audio.clip = tipsSFX;
        audio.Play();
    }
}
