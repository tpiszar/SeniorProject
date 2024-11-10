using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource soundObj;

    public AudioMixer audioMixer;

    public void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public void Start()
    {
        SetMasterVolume(SaveLoad.masterVolume);
        SetMusicVolume(SaveLoad.musicVolume);
        SetFXVolume(SaveLoad.sounfFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetFXVolume(float volume)
    {
        audioMixer.SetFloat("FXVolume", Mathf.Log10(volume) * 20f);
    }

    public void PlayClip(AudioClip clip, Vector3 point, float volume)
    {
        if (!clip)
        {
            print("An empty clip was passed");
            return;
        }

        AudioSource audioSource = Instantiate(soundObj, point, Quaternion.identity);

        audioSource.clip = clip;

        audioSource.volume = volume;

        audioSource.Play();

        Destroy(audioSource, audioSource.clip.length);
    }

    public void PlayRandomClip(AudioClip[] clips, Vector3 point, float volume)
    {
        int rand  = Random.Range(0, clips.Length);

        PlayClip(clips[rand], point, volume);
    }
}
