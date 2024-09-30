using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandTrack : MonoBehaviour
{
    public Transform hipPos;
    public Rigidbody rig;

    float peakHeight;
    public float trackPrecent = 0.5f;

    public float hoverHeight = 0.3f;

    bool dropped = false;
    float dropVelocity;
    //float timeDropped = 0;
    //float timeReturning = 0;

    bool returnToSender = false;

    // Start is called before the first frame update
    void Start()
    {
        //if (transform.parent == hipPos)
        //{
        //    transform.position = hipPos.position;
        //    transform.rotation = hipPos.rotation;
        //    this.enabled = false;
        //}

        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dropped)
        {
            if (!returnToSender)
            {
                //timeDropped += Time.deltaTime;
                if (transform.position.y > peakHeight)
                {
                    peakHeight = transform.position.y;
                    dropVelocity = rig.velocity.y;
                    rig.useGravity = false;
                }
                else if (transform.position.y <= peakHeight / 2)
                {
                    returnToSender = true;

                    Vector3 newVel = rig.velocity;

                    newVel.y = Mathf.Lerp(0, dropVelocity, peakHeight - transform.position.y + hoverHeight / peakHeight + hoverHeight);
                    rig.velocity = newVel;
                }
            }
            else
            {
                //+9.81 * 2
            }

        }
    }

    public void Drop()
    {
        dropped = true;
        peakHeight = transform.position.y;
    }

    public void Collect()
    {
        dropped = false;
        //timeDropped = 0;
        returnToSender = false;
    }
}
