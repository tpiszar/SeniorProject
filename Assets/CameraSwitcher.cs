using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera playerCam;
    public Camera zoomCam;
    public Camera action1;
    public Camera action2;

    public GameObject[] destroys;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerCam.enabled = true;
            zoomCam.enabled = false;
            action1.enabled = false;
            action2.enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerCam.enabled = false;
            zoomCam.enabled = true;
            action1.enabled = false;
            action2.enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            playerCam.enabled = false;
            zoomCam.enabled = false;
            action1.enabled = true;
            action2.enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            playerCam.enabled = false;
            zoomCam.enabled = false;
            action1.enabled = false;
            action2.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Teleport.Instance.curBase = 0;
            Teleport.Instance.testDone = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Teleport.Instance.curBase = 1;
            Teleport.Instance.testDone = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Teleport.Instance.curBase = 2;
            Teleport.Instance.testDone = false;
        }

        if (Input.GetKey(KeyCode.Alpha0))
        {
            foreach (GameObject obj in destroys)
            {
                Destroy(obj);
            }
        }
    }
}
