using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.XR.Interaction.Toolkit;

public class PickUpUseSound : MonoBehaviour
{
    public CurrentHand hand;

    public AudioSource pickupSound;
    public AudioClip useSound;
    [Range(0.0001f, 1f)]
    public float useVolume = 1;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.selectEntered.AddListener(Hold);
        grabbable.selectExited.AddListener(Drop);
        grabbable.activated.AddListener(Use);
    }

    public void Hold(SelectEnterEventArgs args)
    {
        if (Time.timeSinceLevelLoad < 2)
        {
            return;
        }

        pickupSound.Play();
        //if (!hand.noHand)
        //{
        //    pickupSound.Play();
        //}

    }

    public void Drop(SelectExitEventArgs args)
    {
        
    }

    void Use(ActivateEventArgs args)
    {
        if (!useSound)
        {
            return;
        }
        SoundManager.instance.PlayClip(useSound, transform.position, useVolume);
    }
}
