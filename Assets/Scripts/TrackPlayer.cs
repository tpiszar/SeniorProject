using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPlayer : MonoBehaviour
{
    public float height;
    public Transform xrOrigin;
    public Transform player;

    Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        position = player.position;
        position.y = xrOrigin.position.y + height;
        transform.position = position;
    }
}
