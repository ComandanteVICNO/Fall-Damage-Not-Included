using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleFlashlight : MonoBehaviour
{
    [Header("Lanterna")]
    public Light flashlight;
    public KeyCode toggleFlashlight = KeyCode.T;
    bool flashlightOn = false;
    bool flashlightReady;

    // Start is called before the first frame update
    void Start()
    {
        flashlight = GetComponent<Light>();
        flashlight.intensity = 0f;
        flashlightOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        ToggleFlashligh();
    }
    private void ToggleFlashligh()
    {
        if (Input.GetKey(toggleFlashlight) && flashlightOn)
        {
            flashlight = GetComponent<Light>();
            flashlight.intensity = 0f;
            flashlightOn = false;
        }
        else if (Input.GetKey(toggleFlashlight))
        {
            flashlight = GetComponent<Light>();
            flashlight.intensity = 1f;
            flashlightOn = true;
        }
    }
}
