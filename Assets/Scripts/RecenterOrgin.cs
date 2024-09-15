using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Subsystems;
using Unity.XR.CoreUtils;
using UnityEngine.XR;

public class RecenterOrgin : MonoBehaviour
{
    public InputActionProperty recenterButton;

    public Transform head;
    //public Transform origin;
    public Transform target;

    XROrigin origin;

    void Start()
    {
        origin = GetComponent<XROrigin>();

    }

    // Update is called once per frame
    void Update()
    {

        print(head.position.y + " " + head.localPosition.y + " " + Time.time);

        if (recenterButton.action.WasPressedThisFrame())
        {
            //origin.MoveCameraToWorldLocation(target.position);
            //origin.MatchOriginUpCameraForward(target.up, target.forward);
        }
    }
}
