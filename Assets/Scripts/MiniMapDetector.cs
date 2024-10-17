using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MiniMapDetector : MonoBehaviour
{
    public Transform miniReference;
    public float mapScale;
    float colliderHeight;

    void Start()
    {
        colliderHeight = miniReference.InverseTransformPoint(transform.position).y;
    }

    private void OnTriggerStay(Collider other)
    {
        CreateTower tower = other.transform.parent.GetComponent<CreateTower>();


        //REMOVE FOR SHOWING WITHOUT SELECTING
        if (Mana.Instance.CheckMana(tower.manaCost))
        {
            if (tower.overlap == 0)
            {
                Vector3 newPos = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);
                tower.createPoint = newPos;

                float baseAttach = newPos.y;
                newPos += tower.holderTrans.forward * tower.attachPoint.localPosition.z * tower.transform.localScale.z;

                //Possibly needed for future attach point issues
                //newPos += tower.holderTrans.right * tower.attachPoint.localPosition.x * tower.transform.localScale.x;

                float attachHeightAdjust = baseAttach + (tower.attachPoint.localPosition.y * tower.transform.localScale.y);
                newPos.y = attachHeightAdjust;

                tower.socket.position = newPos;
                Transform socketAttach = tower.socket.GetChild(0);
                Vector3 yRotChange = new Vector3(socketAttach.rotation.eulerAngles.x, tower.holderTrans.rotation.eulerAngles.y, socketAttach.rotation.eulerAngles.z);
                socketAttach.rotation = Quaternion.Euler(yRotChange);

                tower.createRot.y = tower.holderTrans.rotation.eulerAngles.y;
            }
        }
        else
        {
            tower.socket.position = Vector3.down * 1000;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CreateTower tower = other.transform.parent.GetComponent<CreateTower>();
        tower.miniReference = miniReference;
        tower.mapScale = mapScale;
        tower.socket.parent = transform;

        //tower.ToggleVisible();
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    CreateTower tower = other.transform.parent.GetComponent<CreateTower>();
    //    tower.ToggleVisible();
    //}
}
