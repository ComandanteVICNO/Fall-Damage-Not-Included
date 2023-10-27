using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    float savedVolumeValue;

    public AudioMixer audiomixer;
    public AudioSource ButtonAudio;
    public AudioClip mouseHover;
    public AudioClip mousePressed;

    public void Awake()
    {
        savedVolumeValue = PlayerPrefs.GetFloat("Volume");
      
        audiomixer.SetFloat("volume", savedVolumeValue);
        
    }

    private void Update()
    {
        savedVolumeValue = PlayerPrefs.GetFloat("Volume");
        audiomixer.SetFloat("volume", savedVolumeValue);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Jogo");
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
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
