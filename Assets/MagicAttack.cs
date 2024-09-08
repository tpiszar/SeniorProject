using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class MagicAttack : MonoBehaviour
{
    public HandAnimator hand;
    public GameObject fireBall;
    public GameObject blast;
    public Transform launchPoint;
    public int attkType;
    public float force;
    bool primed = false;
    public float shotTime = 0.15f;
    float nextShotTime = 0;
    bool firing = false;
    public float fireRate = 1f;
    float nextFire = 0;
    bool cancelOverride = false;
    public float overrideTime = 0.15f;
    float overrideAnim = 0;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float triggerVal = hand.triggerAction.action.ReadValue<float>();
        float gripVal = hand.gripAction.action.ReadValue<float>();

        if (nextFire <= Time.time)
        {
            if (!primed && gripVal == 1 && triggerVal == 1)
            {
                primed = true;
                hand.overrideTrig = 1;
                hand.overrideGrip = 1;
            }
        }

        if (primed)
        {
            if (triggerVal == 0)
            {
                primed = false;
                firing = true;
                nextShotTime = Time.time + shotTime;
            }
            else
            {
                return;
            }

        }

        if (nextShotTime > Time.time)
        {
            //Lerp into firing hand motion

            float alpha = (nextShotTime - Time.time) / shotTime;
            hand.overrideTrig = Mathf.Lerp(0, 1, alpha);
            hand.overrideGrip = Mathf.Lerp(0, 1, alpha);
        }
        else if (firing)
        {
            firing = false;
            cancelOverride = true;
            nextFire = Time.time + fireRate;
            overrideAnim = Time.time + overrideTime;
            switch(attkType)
            {
                case 0:
                    //FIREBALL

                    break;

                case 1:
                    GameObject eBlast = Instantiate(blast, launchPoint.position, Quaternion.identity);
                    eBlast.GetComponent<Rigidbody>().AddForce(launchPoint.forward * force, ForceMode.Impulse);
                    break;

                case 2:
                    //LIGHTNING

                    break;
            }
        }

        if (overrideAnim > Time.time)
        {
            //Lerp back into basic hand action
            float alpha = (overrideAnim - Time.time) / overrideTime;
            hand.overrideTrig = Mathf.Lerp(triggerVal, 0, alpha);
            hand.overrideGrip = Mathf.Lerp(gripVal, 0, alpha);
        }
        else if (cancelOverride)
        {
            cancelOverride = false;
            hand.overrideTrig = -1;
            hand.overrideGrip = -1;
        }
    }
}
