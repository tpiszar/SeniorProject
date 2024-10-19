using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTowerDetection : TowerDetection
{
    public List<EnemyAI> enemies = new List<EnemyAI>();
    public List<BasicHealth> healths = new List<BasicHealth>();

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
                enemies.Add(enemy);
                healths.Add(enemy.GetComponent<BasicHealth>());
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
                enemies.Remove(enemy);
                healths.Remove(other.GetComponent<BasicHealth>());
            }
        }
    }

    public void CleanUp()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (!enemies[i])
            {
                enemies.RemoveAt(i);
                healths.RemoveAt(i);
            }
        }
    }

    public bool isEmpty()
    {
        CleanUp();

        return enemies.Count == 0;
    }

    public override Transform GetTarget()
    {
        if (isEmpty())
        {
            return null;
        }

        switch (detectionType)
        {
            case DetectionType.Close:

                float minDist = enemies[0].GetDistance();
                EnemyAI closestTarget = enemies[0];

                foreach (EnemyAI en in enemies)
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

            case DetectionType.Far:

                float maxDist = enemies[0].GetDistance();
                EnemyAI furthestTarget = enemies[0];

                foreach (EnemyAI en in enemies)
                {
                    if (en == furthestTarget)
                    {
                        continue;
                    }

                    float nextDist = en.GetDistance();
                    if (nextDist > maxDist)
                    {
                        maxDist = nextDist;
                        furthestTarget = en;
                    }
                }

                return furthestTarget.transform;

            case DetectionType.Strong:

                float maxHealth = healths[0].GetHealth();
                BasicHealth maxTarget = healths[0];

                foreach (BasicHealth en in healths)
                {
                    if (en == maxTarget)
                    {
                        continue;
                    }

                    float nextHealth = en.GetHealth();
                    if (nextHealth > maxHealth)
                    {
                        maxHealth = nextHealth;
                        maxTarget = en;
                    }
                }

                return maxTarget.transform;
        }

        return null;
    }
}
