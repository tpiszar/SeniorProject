using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRay : MonoBehaviour
{
    bool holding = false;
    public bool overrideOn = false;

    public static bool activeHandRays = false;

    bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        activeHandRays = overrideOn;

        started = true;

        if (!activeHandRays || holding)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
