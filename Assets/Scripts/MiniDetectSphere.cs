using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniDetectSphere : MonoBehaviour
{
    public float radius;

    public float mapScale = 0.03f;

    public Transform follow;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(
            radius * 2 * mapScale,
            radius * 2 * mapScale,
            radius * 2 * mapScale);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = follow.position;
    }
}
