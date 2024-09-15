using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DropReturn : MonoBehaviour
{
    public Transform resetPoint;
    Vector3 basicPoint;
    Quaternion basicRot;

    public float resetTime = 5;
    float nextReset = 0;
    public float lostResetTime = 15;
    float nextLostReset = 0;

    Rigidbody rig;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.selectEntered.AddListener(Grabbed);
        grabbable.selectExited.AddListener(Dropped);

        rig = GetComponent<Rigidbody>();

        if (!resetPoint)
        {
            basicPoint = transform.position;
            basicRot = transform.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (nextLostReset > 0)
        {
            nextLostReset -= Time.deltaTime;
            if (nextLostReset < 0)
            {
                rig.velocity = Vector3.zero;
                ResetObj();
            }

            float horizontalMove = Mathf.Abs(rig.velocity.x) + Mathf.Abs(rig.velocity.y);
            if (horizontalMove < 0.1f)
            {
                nextReset -= Time.deltaTime;
                if (nextReset < 0)
                {
                    ResetObj();
                }
            }
        }
    }

    void ResetObj()
    {
        if (resetPoint)
        {
            transform.position = resetPoint.position;
            transform.rotation = resetPoint.rotation;
        }
        else
        {
            transform.position = basicPoint;
            transform.rotation = basicRot;
        }
    }

    public void Grabbed(SelectEnterEventArgs args)
    {
        nextLostReset = 0;
        nextReset = 0;
    }

    public void Dropped(SelectExitEventArgs args)
    {
        nextLostReset = lostResetTime;
        nextReset = resetTime;
    }
}
