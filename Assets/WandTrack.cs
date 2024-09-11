using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandTrack : MonoBehaviour
{
    public Transform hipPos;
    public Rigidbody rig;
    public float velocityThreshold = 5f;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent == hipPos)
        {
            transform.position = hipPos.position;
            transform.rotation = hipPos.rotation;
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if ((Mathf.Abs(rig.velocity.x) + Mathf.Abs(rig.velocity.z)) < velocityThreshold)
        //{
        //    transform.position = hipPos.position;
        //    transform.rotation = hipPos.rotation;
        //    transform.parent = hipPos;
        //    this.enabled = false;
        //}
        print(Mathf.Abs(rig.velocity.x) + Mathf.Abs(rig.velocity.z));
    }
}
