using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.EventSystems.PointerEventData;

public class HandLaser : MonoBehaviour
{
    public float duration;
    public float timer = 0;

    public XRDirectInteractor handGrab;
    public XRBaseController controller;
    public XRNode handNode;
    public InputHelpers.Button inputButton;
    public float inputThreshold = 0.1f;

    public Animator animator;

    public float mapScale;
    public Transform miniReference;

    public GameObject laserObj;
    public Laser laserPoint;
    public LineRenderer smallLaser;

    public float range;

    [Range(0, 1)]
    public float baseHapticIntensity;
    public float baseHapticDuration;
    [Range(0, 1)]
    public float fireHapticIntensity;
    public float fireHapticDuration;

    public AudioSource startSound;
    public AudioSource shootSound;
    public AudioSource beginShootSound;

    public ParticleSystem laserHitParticle;

    // Start is called before the first frame update
    void Start()
    {
        if (handNode == XRNode.RightHand)
        {
            startSound.Play();

            WallSwitcher.Instance.SetSolid();
        }
    }

    bool wasPressed = false;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            gameObject.SetActive(false);

            WallSwitcher.Instance.SetFade();

            return;
        }

        TriggerHaptic(baseHapticIntensity, baseHapticDuration);

        Debug.DrawRay(transform.position, transform.forward, Color.red);

        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(handNode), inputButton, out bool isPressed, inputThreshold);

        if (isPressed)
        {

            TriggerHaptic(fireHapticIntensity, fireHapticDuration);

            if (!wasPressed)
            {
                beginShootSound.Play();
            }
            wasPressed = true;

            if (!shootSound.isPlaying)
            {
                shootSound.Play();
            }

            if (!laserHitParticle.isPlaying)
            {
                laserHitParticle.Play();
            }

            Vector3 groundPos;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, range))
            {
                groundPos = hit.point;

                Quaternion directionShift = miniReference.rotation;

                laserPoint.SetLaser(miniReference.InverseTransformPoint(groundPos) * mapScale, miniReference.InverseTransformPoint(transform.position) * mapScale);//directionShift * (transform.position - groundPos));

                //print(miniReference.InverseTransformPoint(groundPos) * mapScale + " " + miniReference.InverseTransformPoint(transform.position) * mapScale);

                //print((miniReference.InverseTransformPoint(transform.position) * mapScale - miniReference.InverseTransformPoint(groundPos) * mapScale).normalized);
            }
            else
            {
                groundPos = transform.position + transform.forward * range;

                laserPoint.SetLaser(Vector3.down * 30, Vector3.down * 30 + Vector3.right);
            }

            smallLaser.SetPosition(0, transform.position);
            smallLaser.SetPosition(1, groundPos);


            laserHitParticle.transform.position = groundPos;
            laserHitParticle.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));

        }
        else
        {
            wasPressed = false;

            shootSound.Stop();

            laserHitParticle.Stop();

            smallLaser.SetPosition(0, transform.position);
            smallLaser.SetPosition(1, transform.position);

            laserPoint.SetLaser(Vector3.down * 30, Vector3.down * 30 + Vector3.right);
        }
    }

    public void TriggerHaptic(float intensity, float duration)
    {
        controller.SendHapticImpulse(intensity, duration);
    }

    private void OnEnable()
    {
        timer = duration;

        handGrab.enabled = false;

        laserObj.SetActive(true);

        smallLaser.enabled = true;

        animator.SetBool("Laser", true);
    }

    private void OnDisable()
    {
        handGrab.enabled = true;

        laserObj.SetActive(false);

        smallLaser.enabled = false;

        animator.SetBool("Laser", false);

        laserHitParticle.Stop();
    }
}
