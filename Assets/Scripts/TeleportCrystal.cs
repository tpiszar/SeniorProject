using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportCrystal : MonoBehaviour
{
    public int baseNum;

    public float useRate;
    float timeToUse = -1;
    //public bool startOff = false;

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

    public GameObject timerUI;
    public TextMeshProUGUI timerText;

    public ParticleSystem shatterParticle;

    // Start is called before the first frame update
    void Start()
    {
        grabbable = GetComponent<XRGrabInteractable>();
        grabbable.selectEntered.AddListener(Hold);
        grabbable.selectExited.AddListener(Drop);
        grabbable.activated.AddListener(Use);

        crystalMat = mesh.material;

        dropReturn = GetComponent<DropReturn>();
        if (Teleport.Instance.startingBase == baseNum)
        {
            timeToUse = useRate;
            off = true;
            grabbable.enabled = false;
        }
        else
        {
            timerUI.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (off)
        {
            timeToUse -= Time.deltaTime;

            if (timerText)
            {
                timerText.text = timeToUse.ToString("F1");
            }

            crystalMat.color = new Color(crystalMat.color.r, crystalMat.color.g, crystalMat.color.b, Mathf.Lerp(maxFade, minFade, timeToUse / useRate));
            mesh.material = crystalMat;
            if (timeToUse < 0)
            {
                if (shatterParticle)
                {
                    shatterParticle.transform.parent = transform;
                }

                off = false;
                grabbable.enabled = true;

                crystalMat.color = new Color(crystalMat.color.r, crystalMat.color.g, crystalMat.color.b, 1);
                mesh.material = crystalMat;

                if (timerUI)
                {
                    timerUI.SetActive(false);
                }
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
        if (timeToUse < 0 && !Teleport.Instance.isTeleporting)
        {
            if (shatterParticle)
            {
                shatterParticle.transform.parent = null;
                shatterParticle.transform.position = transform.position;
                shatterParticle.Play();
            }

            Teleport.Instance.isTeleporting = true;

            timeToUse = useRate;
            off = true;
            grabbable.enabled = false;
            dropReturn.ResetObj();

            timerUI.SetActive(true);

            Teleport.Instance.startTeleport(baseNum);

            TriggerHaptic(crushHapticIntensity, Teleport.Instance.easeInTime);
            Invoke("TeleportHaptic", Teleport.Instance.easeInTime);
        }
    }

    private void OnEnable()
    {
       
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
