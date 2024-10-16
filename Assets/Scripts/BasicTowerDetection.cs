using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTowerDetection : MonoBehaviour
{
    public List<EnemyAI> targets = new List<EnemyAI>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Root"))
        {
            EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
            if (enemy)
            {
                //if (targets.Contains(enemy))
                //{
                //    return;
                //}
                targets.Add(enemy);
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
                //if (!targets.Contains(enemy))
                //{
                //    return;
                //}
                targets.Remove(enemy);
            }
        }
    }

    public void CleanUp()
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (!targets[i])
            {
                targets.RemoveAt(i);
            }
        }
    }

    public bool isEmpty()
    {
        CleanUp();

        return targets.Count == 0;
    }

    public Transform GetTarget()
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (!targets[i])
            {
                targets.RemoveAt(i);

            }
        }

        if (targets.Count == 0)
        {
            return null;
        }

        float minDist = targets[0].GetDistance();
        EnemyAI closestTarget = targets[0];

        foreach(EnemyAI en in targets)
        {
            if (en == closestTarget)
            {
                continue;
            }

            float nextDist = en.GetDistance();
            if (nextDist < minDist)
            {
                minDist = nextDist;
                closestTarget = en;
            }
        }

        return closestTarget.transform;
    }
}
