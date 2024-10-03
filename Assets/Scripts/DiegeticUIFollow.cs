using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiegeticUIFollow : MonoBehaviour
{
    public Transform player;
    public float upClamp = 25;
    public float downClamp = 10;

    // Start is called before the first frame update
    void Start()
    {
        player = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(player.position.x, player.position.y, player.position.z));
        float preClamp = transform.rotation.eulerAngles.x;

        if(preClamp > 180)
        {
            preClamp -= 360;
        }
        transform.rotation = Quaternion.Euler(new Vector3(Mathf.Clamp(preClamp, -upClamp, downClamp), transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
    }
}
