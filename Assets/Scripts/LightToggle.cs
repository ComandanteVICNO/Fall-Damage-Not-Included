using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightToggle : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.T; // Key to toggle the light
    public KeyCode toggleColor = KeyCode.F;
    private Light lightSource; // Reference to the Light component
    public Color lightColor1 = new Color(255f/255f, 248f / 255f, 235f / 255f);
    public Color lightColor2 = new Color(123f / 255f, 53f / 255f, 241f / 255f);
    bool colorWhite = true;
    public PlayerMovement pm;
    public AudioClip flashLightOn;
    public AudioClip flashLightOff;

    private void Start()
    {
        // Get the Light component from the GameObject
        lightSource = GetComponent<Light>();
        colorWhite = true;
    }

    private void Update()
    {
        // Check for input to toggle the light
        if (Input.GetKeyDown(toggleKey))
        {
            lightSource.enabled = !lightSource.enabled; // Toggle the light on/off
            if(lightSource.enabled == true) 
            {
                pm.playerAudioSource.PlayOneShot(flashLightOff);
            }
            else if(lightSource.enabled == false)
            {
                pm.playerAudioSource.PlayOneShot(flashLightOn);
            }
        }
        
        if(Input.GetKeyDown(toggleColor) && colorWhite == true)
        {

            lightSource.color = lightColor2;
            colorWhite = !colorWhite;

        }
        else if(Input.GetKeyDown(toggleColor) && colorWhite == false)
        {
            lightSource.color = lightColor1;
            colorWhite = !colorWhite;
        }

    }
}
