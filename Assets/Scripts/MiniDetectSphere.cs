using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniDetectSphere : MonoBehaviour
{
    public float radius;

    public float mapScale = 0.03f;

    Transform lastParent;

    // Start is called before the first frame update
    void Start()
    {
        lastParent = transform.parent;

        SetGlobalScale();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent != lastParent)
        {
            SetGlobalScale();
            lastParent = transform.parent;
        }
    }

    void SetGlobalScale()
    {
        transform.localScale = new Vector3(
            radius * 2 / transform.parent.lossyScale.x * mapScale,
            radius * 2 / transform.parent.lossyScale.y * mapScale,
            radius * 2 / transform.parent.lossyScale.z * mapScale);
    }
}
