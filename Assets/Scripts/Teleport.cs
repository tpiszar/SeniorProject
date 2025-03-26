using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Teleport : MonoBehaviour
{
    public static Teleport Instance;

    public event Action<float> onTeleport;

    public Transform[] bases;
    public GameObject[] crystals;
    public Transform playerBase;

    public int startingBase = 0;
    public int curBase = 0;
    int lastBase = -1;

    public float easeInTime = 0.3f;
    public float teleportTime;

    public CustomProvider vignetteProvider;

    public bool isTeleporting = false;

    public AudioSource teleportSound;

    public ParticleSystem teleportParticle;

    public void startTeleport(int baseNum)
    {
        lastBase = curBase;
        curBase = baseNum;

        if (onTeleport != null)
        {
            onTeleport(easeInTime);
        }

        vignetteProvider.setLocomotion(LocomotionPhase.Moving);


        teleportSound.Play();

        //teleportParticle.Play();

        Invoke("doTeleport", easeInTime);
    }

    public void doTeleport()
    {
        playerBase.position = bases[curBase].position;
        playerBase.rotation = bases[curBase].rotation;

        crystals[curBase].SetActive(false);
        if (lastBase >= 0)
        {
            crystals[lastBase].SetActive(true);
        }

        //teleportParticle.Play();

        Invoke("endTeleport", easeInTime);
    }

    bool firstPlay = true;
    public void endTeleport()
    {
        vignetteProvider.setLocomotion(LocomotionPhase.Done);

        if (!firstPlay)
        {
            teleportParticle.Play();
        }
        firstPlay = false;

        isTeleporting = false;
    }

    public void Awake()
    {
        //if (!Instance)
        //{
        //    Instance = this;
        //}
        //else
        //{
        //    Destroy(this);
        //}

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        startingBase = curBase;

        Teleport.Instance.onTeleport += onTeleport;

        doTeleport();
    }

    public bool testDone = true;

    // Update is called once per frame
    void Update()
    {
        if (!testDone)
        {
            startTeleport(curBase);
            testDone = true;
        }
    }
}
