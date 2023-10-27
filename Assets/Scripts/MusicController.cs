using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;

public class MusicController : MonoBehaviour
{
    public AudioClip audioClip1;
    public AudioClip audioClip2;
    public AudioClip audioClip3;
    AudioSource m1;
    AudioSource m2;
    AudioSource m3;

    public AudioMixerGroup mixerGroup;

    public MusicHitbox1 mh1;
    public MusicHitbox2 mh2;
    public MusicHitbox3 mh3;

    private bool musica1;
    private bool musica2;
    private bool musica3;

    // Start is called before the first frame update
    void Start()
    {

        m1 = AddAudio(true, true, false);
        m2 = AddAudio(true, true, true);
        m3 = AddAudio(true, true, true);

        StartPlayingSounds();
    }

    void Update()
    {
        whatMusicToPlay();
        musica1 = mh1.hitboxCheck;
        musica2 = mh2.hitboxCheck;
        musica3 = mh3.hitboxCheck;
    }

    public AudioSource AddAudio(bool loop, bool playAwake, bool mute)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.mute = mute;
        newAudio.outputAudioMixerGroup = mixerGroup;
        return newAudio;
    }

    void StartPlayingSounds()
    {
        m1.clip = audioClip1;
        m1.Play();
        m2.clip = audioClip2;
        m2.Play();
        m3.clip = audioClip3;
        m3.Play();

    }


    void whatMusicToPlay()
    {
        if (musica1)
        {

            m1.mute = false;
            m2.mute = true;
            m3.mute = true;
        }
        else if (musica2)
        {

            m1.mute = true;
            m2.mute = false;
            m3.mute = true;
        }
        else if (musica3)
        {

            m1.mute = true;
            m2.mute = true;
            m3.mute = false;
        }
    }
}
