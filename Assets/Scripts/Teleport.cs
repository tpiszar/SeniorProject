using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Teleport : MonoBehaviour
{
    public static Teleport Instance;

    public event Action<float> onTeleport;

    public float teleportTime;
    public void doTeleport()
    {
        if (onTeleport != null)
        {
            onTeleport(teleportTime);
        }
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
    }

    bool testDone = true;

    // Update is called once per frame
    void Update()
    {
        if (!testDone && Time.time > 15)
        {
            doTeleport();
            testDone = true;
        }
    }
}
