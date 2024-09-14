using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapDetector : MonoBehaviour
{
    public Transform miniReference;
    public float mapScale;

    private void OnTriggerStay(Collider other)
    {
        CreateTower tower = other.transform.parent.GetComponent<CreateTower>();
        if (tower.overlap == 0)
        {
            Vector3 newPos = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);
            tower.socket.position = newPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CreateTower tower = other.transform.parent.GetComponent<CreateTower>();
        tower.miniReference = miniReference;
        tower.mapScale = mapScale;
    }
}
