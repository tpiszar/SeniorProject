using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource soundObj;

    public AudioMixer audioMixer;

    public AudioClip energyDeath;
    [Range(0.0001f, 1f)]
    public float energyVolume = 1;
    public AudioClip fireDeath;
    [Range(0.0001f, 1f)]
    public float fireVolume = 1;
    public AudioClip lightningDeath;
    [Range(0.0001f, 1f)]
    public float lightningVolume = 1;

    public AudioClip annihilateAudio;
    [Range(0.0001f, 1f)]
    public float annihilateVolume = 1;

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

        Destroy(audioSource.gameObject, audioSource.clip.length);
    }

    public void SpawnClip(AudioSource source, Vector3 point)
    {
        GameObject newSound = Instantiate(source.gameObject, point, Quaternion.identity);
        Destroy(newSound, source.clip.length);
    }

    public void PlayRandomClip(AudioClip[] clips, Vector3 point, float volume)
    {
        int rand  = Random.Range(0, clips.Length);

        PlayClip(clips[rand], point, volume);
    }

    public void PlayDeathClip(DamageType damageType, Vector3 point)
    {
        switch(damageType)
        {
            case DamageType.energy:
                PlayClip(energyDeath, point, energyVolume);
                break;
            case DamageType.fire:
                PlayClip(fireDeath, point, fireVolume);
                break;
            case DamageType.lightning:
                PlayClip(lightningDeath, point, lightningVolume);
                break;
        }
    }

    public void Annihilate(Vector3 point)
    {
        PlayClip(annihilateAudio, point, annihilateVolume);
    }
}
