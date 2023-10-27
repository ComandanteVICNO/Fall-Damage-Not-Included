using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update

    public KeyCode flashlight = KeyCode.T;
    public GameObject tutorial_Text;
    public GameObject reticleUI;
    public GameObject graplingHook;
    public GameObject flashlightTutorial;

    void Start()
    {
        tutorial_Text.SetActive(false);
        reticleUI.SetActive(false);
        graplingHook.SetActive(false);
        flashlightTutorial.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(flashlight))
        {
            tutorial_Text.SetActive(true);
            reticleUI.SetActive(true);
            graplingHook.SetActive(true);
            flashlightTutorial.SetActive(false);
        }
    }
}
