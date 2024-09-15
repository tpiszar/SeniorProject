using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MiniMapDetector : MonoBehaviour
{
    public Transform miniReference;
    public float mapScale;

    private void OnTriggerStay(Collider other)
    {
        CreateTower tower = other.transform.parent.GetComponent<CreateTower>();
        if (tower.overlap == 0)
        {
            Vector3 newPos = new Vector3(other.transform.position.x, transform.position.y + (tower.attachPoint.localPosition.y * tower.transform.localScale.y), other.transform.position.z); //transform.position.y
            newPos += tower.holderTrans.forward * tower.attachPoint.localPosition.z * tower.transform.localScale.z;

            //Possibly needed for future attach point issues
            //newPos += tower.holderTrans.right * tower.attachPoint.localPosition.x * tower.transform.localScale.x;

            tower.socket.position = newPos;
            Transform socketAttach = tower.socket.GetChild(0);
            Vector3 yRotChange = new Vector3(socketAttach.rotation.eulerAngles.x, tower.holderTrans.rotation.eulerAngles.y, socketAttach.rotation.eulerAngles.z);
            socketAttach.rotation = Quaternion.Euler(yRotChange);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CreateTower tower = other.transform.parent.GetComponent<CreateTower>();
        tower.miniReference = miniReference;
        tower.mapScale = mapScale;

        //tower.ToggleVisible();
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    CreateTower tower = other.transform.parent.GetComponent<CreateTower>();
    //    tower.ToggleVisible();
    //}
}
