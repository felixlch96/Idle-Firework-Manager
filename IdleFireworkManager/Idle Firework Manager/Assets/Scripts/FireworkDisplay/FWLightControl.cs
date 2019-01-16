using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FWLightControl : MonoBehaviour
{
    ParticleSystem particles;
    Light fwLight;

	// Use this for initialization
	void Start () {
        particles = GetComponent<ParticleSystem>();
        fwLight = particles.lights.light;
	}
	
	// Update is called once per frame
	void Update ()
    {        
        if (fwLight != null)
        {
            if (particles.isPlaying)
            {
                if (fwLight.intensity > 0)
                {
                    fwLight.intensity -= 0.2f;
                }
            }
            else
                fwLight.intensity = 10;
        }        
	}
}
