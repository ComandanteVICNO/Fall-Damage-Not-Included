using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour
{

    public TMP_InputField keybindingInputField;


    [Header("Audio")]
    public AudioMixer audiomixer;
    public Slider volumeSlider;

    [Header("Fov")]
    public Slider fovSlider;
    public TMP_Text uiFovValue;

    [Header("GraphicalSettings")]

    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    private int quality;

    Resolution[] resolutions;
    [Header("InverMouse")]
    public int invertedValue;
    

    [Header("Sensitivity")]
    public Slider horizontalSensitivitySlider;
    public Slider verticalSensitivitySlider;
    public TMP_Text uiHSensitivityValue;
    public TMP_Text uiVSensitivityValue;

    

    private void Start()
    {

        if (PlayerPrefs.HasKey("InvertMouse"))
        {
            invertedValue = PlayerPrefs.GetInt("InvertMouse");
        }
        else
        {
            invertedValue = 0;
        }


        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualitySettings"));

        //Load the saved volumeValue from PlayerPrefs
        float savedVolumeValue = PlayerPrefs.GetFloat("Volume");
        volumeSlider.value = savedVolumeValue;
        audiomixer.SetFloat("volume", savedVolumeValue);

        // Load the saved fovValue from PlayerPrefs
        float savedFovValue = PlayerPrefs.GetFloat("Fov");
        if (savedFovValue == 0)
        {
            savedFovValue = 70;
        }
        fovSlider.value = savedFovValue;
        uiFovValue.text = savedFovValue.ToString();

        // Load the saved hSensitivity and vSensitivity from PlayerPrefs
        float savedHSensitivity = PlayerPrefs.GetFloat("HorizontalSensitivity");
        float savedVSensitivity = PlayerPrefs.GetFloat("VerticalSensitivity");
        horizontalSensitivitySlider.value = savedHSensitivity;
        verticalSensitivitySlider.value = savedVSensitivity;
        uiHSensitivityValue.text = savedHSensitivity.ToString();
        uiVSensitivityValue.text = savedVSensitivity.ToString();

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> resolutionOptions = new List<string>();

        int currentResoluctionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resOption = resolutions[i].width + " x " + resolutions[i].height;
            resolutionOptions.Add(resOption);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResoluctionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResoluctionIndex;
        resolutionDropdown.RefreshShownValue();

    }
    void OnKeybindingChanged(string newValue)
    {
        // Handle the changed keybinding
        Debug.Log("New keybinding: " + newValue);
        // Update the keybinding data or perform any other necessary actions
    }
    private void Update()
    {
        UpdateFovValue();
        SetVolume();
        SetSensitivity();
    }

    public void UpdateFovValue()
    {
        float fovValue = Mathf.RoundToInt(fovSlider.value);
        PlayerPrefs.SetFloat("Fov", fovValue);
        uiFovValue.text = fovValue.ToString();
    }

    public void SetVolume()
    {
        float volumeValue = volumeSlider.value;
        PlayerPrefs.SetFloat("Volume", volumeValue);
        audiomixer.SetFloat("volume", volumeValue);
    }

    public void SetSensitivity()
    {
        float hSensitivityValue = horizontalSensitivitySlider.value;
        float vSensitivityValue = verticalSensitivitySlider.value;
        PlayerPrefs.SetFloat("HorizontalSensitivity", hSensitivityValue);
        PlayerPrefs.SetFloat("VerticalSensitivity", vSensitivityValue);
        uiHSensitivityValue.text = (Mathf.RoundToInt((hSensitivityValue * 100) / 1)).ToString() + "%";
        uiVSensitivityValue.text = (Mathf.RoundToInt((vSensitivityValue * 100) / 1)).ToString() + "%";


    }

    public void SetInvertedControlls(bool inverted)
    {
        if (inverted)
        {
            PlayerPrefs.SetInt("InvertMouse", 1);
        }
        else
        {
            PlayerPrefs.SetInt("InvertMouse", 0);
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualitySettings", qualityIndex);

    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }







}