using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportCrystal : MonoBehaviour
{
    public int baseNum;

    public float useRate;
    float timeToUse = -1;

    public MeshRenderer mesh;
    [Range(0, 1)]
    public float minFade;
    [Range(0, 1)]
    public float maxFade;
    Material crystalMat;

    bool held = false;
    bool off = false;

    public CurrentHand hand;

    [Range(0, 1)]
    public float heldHapticIntensity;
    public float heldHapticDuration;
    [Range(0, 1)]
    public float crushHapticIntensity;
    public float crushHapticDuration;
    [Range(0, 1)]
    public float teleportHapticIntensity;
    public float teleportHapticDuration;

    XRGrabInteractable grabbable;

    DropReturn dropReturn;

    // Start is called before the first frame update
    void Start()
    {
        grabbable = GetComponent<XRGrabInteractable>();
        grabbable.selectEntered.AddListener(Hold);
        grabbable.selectExited.AddListener(Drop);
        grabbable.activated.AddListener(Use);

        crystalMat = mesh.material;

        dropReturn = GetComponent<DropReturn>();

        if (Teleport.Instance.curBase == baseNum)
        {
            timeToUse = useRate;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (off)
        {
            timeToUse -= Time.deltaTime;
            crystalMat.color = new Color(crystalMat.color.r, crystalMat.color.g, crystalMat.color.b, Mathf.Lerp(maxFade, minFade, timeToUse / useRate));
            mesh.material = crystalMat;
            if (timeToUse < 0)
            {
                off = false;
                grabbable.enabled = true;

                crystalMat.color = new Color(crystalMat.color.r, crystalMat.color.g, crystalMat.color.b, 1);
                mesh.material = crystalMat;
            }
        }

        if (held)
        {
            TriggerHaptic(heldHapticIntensity, heldHapticDuration);
        }
    }

    public void Hold(SelectEnterEventArgs args)
    {
        held = true;
    }

    public void Drop(SelectExitEventArgs args)
    {
        held = false;
    }

    void Use(ActivateEventArgs args)
    {
        if (timeToUse < 0)
        {
            timeToUse = useRate;
            off = true;
            grabbable.enabled = false;
            dropReturn.ResetObj();

            Teleport.Instance.startTeleport(baseNum);

            TriggerHaptic(crushHapticIntensity, Teleport.Instance.easeInTime);
            Invoke("TeleportHaptic", Teleport.Instance.easeInTime);
        }
    }

    void TeleportHaptic()
    {
        TriggerHaptic(teleportHapticIntensity, Teleport.Instance.teleportTime);
    }

    public void TriggerHaptic(float intensity, float duration)
    {
        hand.controller.SendHapticImpulse(intensity, duration);
    }
}
