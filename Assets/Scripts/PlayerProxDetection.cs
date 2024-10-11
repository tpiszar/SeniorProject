using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProxDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Root"))
        {
            EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
            if (enemy)
            {
                enemy.isClose();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Root"))
        {
            EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
            if (enemy)
            {
                enemy.isFar();
            }
        }
    }
}
