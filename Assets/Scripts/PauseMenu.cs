using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public AudioSource ButtonAudio;
    public AudioMixer audiomixer;
    public Slider volumeSlider;
    public GameObject menu;
    public PlayerCamera playerCamera;
    public AudioClip mouseHover;
    public AudioClip mousePressed;

    public KeyCode mainMenuKey = KeyCode.Escape;

    public static bool GameIsPaused = false;

    private void Awake()
    {
        Resume();
    }
    // Start is called before the first frame update
    void Start()
    {
        float savedVolumeValue = PlayerPrefs.GetFloat("Volume");
        volumeSlider.value = savedVolumeValue;
        audiomixer.SetFloat("volume", savedVolumeValue);
    }

    // Update is called once per frame
    void Update()
    {
        SetVolume();
        if (Input.GetKeyDown(mainMenuKey))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

    }

    public void SetVolume()
    {
        float volumeValue = volumeSlider.value;
        PlayerPrefs.SetFloat("Volume", volumeValue);
        audiomixer.SetFloat("volume", volumeValue);
    }

    public void Resume()
    {
        menu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        playerCamera.sensX = PlayerPrefs.GetFloat("HorizontalSensitivity");
        playerCamera.sensY = PlayerPrefs.GetFloat("VerticalSensitivity");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ButtonAudio.PlayOneShot(mousePressed);
    }

    public void Pause()
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        playerCamera.sensX = playerCamera.sensY = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ButtonAudio.PlayOneShot(mousePressed);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayHoverSound()
    {
        ButtonAudio.PlayOneShot(mouseHover);
    }
    public void PlayPressSound()
    {
        ButtonAudio.PlayOneShot(mousePressed);
    }
}
