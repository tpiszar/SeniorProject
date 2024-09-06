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

    public GameObject p;
    public Transform launchpoint;
    bool primed;
    public float force;
    float shotTime = 0;
    float shot = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        float triggerVal = overrideTrig;
        float gripVal = overrideGrip;

        if (primed)
        {
            triggerVal = triggerAction.action.ReadValue<float>();

            if (triggerVal == 0)
            {
                primed = false;
                shotTime = 0.15f;
            }
            else
            {
                return;
            }
        }
        if (shotTime > 0)
        {
            shotTime -= Time.deltaTime;
            triggerVal = Mathf.Lerp(0, 1, shotTime / 0.25f);
            gripVal = Mathf.Lerp(0, 1, shotTime / 0.15f);
            if (shotTime < 0)
            {
                shot = 1;
                GameObject hp = Instantiate(p, launchpoint.position, Quaternion.identity);
                hp.GetComponent<Rigidbody>().AddForce(launchpoint.forward * force, ForceMode.Impulse);
            }
        }

        if (triggerVal < 0)
        {
            triggerVal = triggerAction.action.ReadValue<float>();
        }
        if (gripVal < 0)
        {
            gripVal = gripAction.action.ReadValue<float>();
        }

        if (shot > 0)
        {
            shot -= Time.deltaTime;
            triggerVal = Mathf.Lerp(triggerVal, 0, shot / 1f);
            gripVal = Mathf.Lerp(gripVal, 0, shot / 1f);
        }

        if (!primed && gripVal == 1 && triggerVal == 1)
        {
            primed = true;
        }

        animator.SetFloat("Trigger", triggerVal);
        animator.SetFloat("Grip", gripVal);
    }
}
