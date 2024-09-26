using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Teleport : MonoBehaviour
{
    public static Teleport Instance;

    public event Action<float> onTeleport;

    public Transform[] bases;
    public Transform playerBase;

    public int curBase = 0;

    public float teleportTime;

    public void doTeleport(int baseNum)
    {
        if (onTeleport != null)
        {
            onTeleport(teleportTime);
        }

        playerBase.position = bases[baseNum].position;
        playerBase.rotation = bases[baseNum].rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        doTeleport(curBase);
    }

    public bool testDone = true;

    // Update is called once per frame
    void Update()
    {
        if (!testDone)
        {
            doTeleport(curBase);
            testDone = true;
        }
    }
}
