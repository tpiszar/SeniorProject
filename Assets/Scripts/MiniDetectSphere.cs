using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniDetectSphere : MonoBehaviour
{
    public float radius;

    public float mapScale = 0.03f;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(
            radius * 2 / transform.parent.lossyScale.x * mapScale,
            radius * 2 / transform.parent.lossyScale.y * mapScale,
            radius * 2 / transform.parent.lossyScale.z * mapScale);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
