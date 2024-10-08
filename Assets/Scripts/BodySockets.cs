using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public class BodySocket
{
    public Transform sTransform;
    [Range(0.01f, 1f)]
    public float heightRatio;
}

public class BodySockets : MonoBehaviour
{
    public Transform player;
    public Transform xrRig;
    public BodySocket[] sockets;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (BodySocket socket in sockets)
        {
            socket.sTransform.position = new Vector3(socket.sTransform.position.x, player.localPosition.y * socket.heightRatio + xrRig.position.y, socket.sTransform.position.z);
        }
        transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        transform.rotation = new Quaternion(transform.rotation.x, player.rotation.y, transform.rotation.z, player.rotation.w);
    }
}
