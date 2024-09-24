using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProxDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy)
        {
            enemy.isClose();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy)
        {
            enemy.isFar();
        }
    }
}
