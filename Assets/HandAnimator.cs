using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimator : MonoBehaviour
{
    public InputActionProperty triggerAction;

    public InputActionProperty gripAction;

    public Animator animator;

    public float overrideTrig = -1;
    public float overrideGrip = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float triggerVal = overrideTrig;
        float gripVal = overrideGrip;
        if (triggerVal < 0)
        {
            triggerVal = triggerAction.action.ReadValue<float>();
        }
        if (gripVal < 0)
        {
            gripVal = gripAction.action.ReadValue<float>();
        }

        animator.SetFloat("Trigger", triggerVal);
        animator.SetFloat("Grip", gripVal);
    }
}
