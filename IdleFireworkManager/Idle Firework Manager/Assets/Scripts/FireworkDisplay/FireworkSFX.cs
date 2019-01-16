using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkSFX : MonoBehaviour
{
    ParticleSystem particles;
    AudioSource[] audio;

	// Use this for initialization
	void Start ()
    {
        particles = GetComponent<ParticleSystem>();
        audio = GetComponents<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //if the particles is being played, but the sound havent, then play the sound        
		if (particles.isPlaying)
        {
            if (audio.Length > 0)
            {
                if (!audio[0].isPlaying)
                {
                    for (int x = 0; x < audio.Length; x++)
                        audio[x].Play();
                }
            }            
        }
	}
}
