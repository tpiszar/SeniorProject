using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTowerDetection : MonoBehaviour
{
    List<Transform> targets = new List<Transform>();
    public Transform player;

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
        BasicHealth enemy = other.GetComponent<BasicHealth>();
        if (enemy)
        {
            targets.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BasicHealth enemy = other.GetComponent<BasicHealth>();
        if (enemy)
        {
            targets.Remove(other.transform);
        }
    }

    public Transform GetTarget()
    {
        foreach (Transform t in targets)
        {
            if (!t)
            {
                targets.Remove(t);
            }
        }

        if (targets.Count == 0)
        {
            return null;
        }
        
        float minDist = (player.position - targets[0].position).sqrMagnitude;
        Transform closestTarget = targets[0];

        foreach(Transform t in targets)
        {
            if (t == closestTarget)
            {
                continue;
            }

            float nextDist = (player.position - t.position).sqrMagnitude;
            if (nextDist < minDist)
            {
                minDist = nextDist;
                closestTarget = t;
            }
        }

        return closestTarget;
    }
}
