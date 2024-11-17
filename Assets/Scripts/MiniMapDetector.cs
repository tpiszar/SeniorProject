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

        Vector3 newPos = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);

        //Vector3 checkPos = newPos;
        //checkPos.y += 1.123f / mapScale;
        //Vector3 halves = new Vector3(1f / mapScale, 2.27f / mapScale, 1f / mapScale);
        //Vector3 rot = Vector3.zero;
        //rot.y = tower.holderTrans.rotation.eulerAngles.y;


        Vector3 scaledCenterOffset = Vector3.Scale(tower.overlapObj.center, tower.overlapObj.transform.lossyScale);

        Vector3 checkPos = newPos + scaledCenterOffset;

        Vector3 scaledSize = Vector3.Scale(tower.overlapObj.size, tower.overlapObj.transform.lossyScale);
        Vector3 halfExtents = scaledSize / 2;

        Quaternion yRotation = Quaternion.Euler(0, tower.overlapObj.transform.eulerAngles.y, 0);

        foreach (Collider col in Physics.OverlapBox(checkPos, halfExtents, yRotation))
        {
            
            if (col.transform.CompareTag("Blocker") && col.GetComponentInParent<CreateTower>() != tower)
            {
                return;
            }
        }

        //if (tower.overlap == 0)
        //{
            //Vector3 newPos = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);
            tower.createPoint = newPos;

            float baseAttach = newPos.y;
            newPos += tower.holderTrans.forward * tower.attachPoint.localPosition.z * tower.transform.localScale.z;

            //Possibly needed for future attach point issues
            //newPos += tower.holderTrans.right * tower.attachPoint.localPosition.x * tower.transform.localScale.x;

            float attachHeightAdjust = baseAttach + (tower.attachPoint.localPosition.y * tower.transform.localScale.y);
            newPos.y = attachHeightAdjust;

            tower.socket.position = newPos;

            // TEST TO TRY AND BETTER RANGED PLACE
            tower.socket.position = tower.transform.position;

            Transform socketAttach = tower.socket.GetChild(0);

            //TEST TO TRY AND BETTER RANGED PLACE
            socketAttach.position = newPos;

            Vector3 yRotChange = new Vector3(socketAttach.rotation.eulerAngles.x, tower.holderTrans.rotation.eulerAngles.y, socketAttach.rotation.eulerAngles.z);
            socketAttach.rotation = Quaternion.Euler(yRotChange);

            tower.createRot.y = tower.holderTrans.rotation.eulerAngles.y;
        //}

        //REMOVE FOR SHOWING WITHOUT SELECTING
        //if (Mana.Instance.CheckMana(tower.manaCost))
        //{

        //}
        //else
        //{
        //    tower.socket.position = Vector3.down * 1000;
        //}
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
