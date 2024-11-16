using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandRay : MonoBehaviour
{
    bool holding = false;
    int holdCount = 0;
    public bool overrideOn = false;

    public static bool activeHandRays = false;

    bool started = false;

    public bool left = false;

    public static HandRay LeftHandRay;
    public static HandRay RightHandRay;

    // Start is called before the first frame update
    void Start()
    {
        activeHandRays = overrideOn;

        started = true;

        if (!activeHandRays || holding)
        {
            gameObject.SetActive(false);
        }

        if (left)
        {
            LeftHandRay = this;
        }
        else
        {
            RightHandRay = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ForceOff()
    {
        gameObject.SetActive(false);
    }

    public void ForceOn()
    {
        gameObject.SetActive(true);
    }

    public void IncrementHolding()
    {
        holdCount++;
        print(holdCount);
        isHolding(true);
    }

    public void DecrementHolding()
    {
        holdCount--;
        print(holdCount);
        isHolding(holdCount == 0);
    }

    public void isHolding(bool hold)
    {
        holding = hold;
        if (holding)
        {
            gameObject.SetActive(false);
        }
        else if (activeHandRays)
        {
            gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (!started)
        {
            return;
        }

        if (!activeHandRays || holding)
        {
            gameObject.SetActive(false);
        }
    }
}
