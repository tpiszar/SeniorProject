using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor;
using UnityEngine.SocialPlatforms;

public class BasicHaptics : MonoBehaviour
{
    public CurrentHand hand;
    bool held = false;

    public bool pickUp = false;
    [Range(0, 1)]
    public float grabHapticIntensity;
    public float grabHapticDuration;

    public bool hold = false;
    [Range(0, 1)]
    public float heldHapticIntensity;
    public float heldHapticDuration;

    public bool activate = false;
    [Range(0, 1)]
    public float activateHapticIntensity;
    public float activateHapticDuration;

    public bool disable = false;
    [Range(0, 1)]
    public float disableHapticIntensity;
    public float disableHapticDuration;

    public bool destroy = false;
    [Range(0, 1)]
    public float destroyHapticIntensity;
    public float destroyHapticDuration;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.selectEntered.AddListener(Hold);
        grabbable.selectExited.AddListener(Drop);
        grabbable.activated.AddListener(Activate);
    }

    // Update is called once per frame
    void Update()
    {
        if (held && !hand.noHand)
        {
            TriggerHaptic(heldHapticIntensity, heldHapticDuration);
        }
    }

    public void Hold(SelectEnterEventArgs args)
    {
        if (pickUp)
        {
            TriggerHaptic(grabHapticIntensity, grabHapticDuration);
        }
        if (hold)
        {
            held = true;
        }
    }

    public void Drop(SelectExitEventArgs args)
    {
        if (hold)
        {
            held = false;
        }
    }

    void Activate(ActivateEventArgs args)
    {
        if (activate)
        {
            TriggerHaptic(activateHapticIntensity, activateHapticDuration);
        }
    }

    public void TriggerHaptic(float intensity, float duration)
    {
        hand.controller.SendHapticImpulse(intensity, duration);
    }

    private void OnDisable()
    {
        if (disable && !hand.noHand)
        {
            hand.controller.SendHapticImpulse(destroyHapticIntensity, destroyHapticDuration);
        }
    }

    private void OnDestroy()
    {
        if (destroy && !hand.noHand)
        {
            hand.controller.SendHapticImpulse(destroyHapticIntensity, destroyHapticDuration);
        }
    }
}
