using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light targetLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 2f;

    private float originalIntensity;
    private float randomFactor;

    private void Start()
    {
        if (targetLight == null)
        {
            targetLight = GetComponent<Light>();
        }

        originalIntensity = targetLight.intensity;
    }

    private void Update()
    {
        // Generate a random factor between -1 and 1
        randomFactor = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) * 2f - 1f;

        // Apply the random factor to the light intensity
        float flickerIntensity = originalIntensity + randomFactor * (maxIntensity - minIntensity);

        // Set the light intensity
        targetLight.intensity = flickerIntensity;
    }
}