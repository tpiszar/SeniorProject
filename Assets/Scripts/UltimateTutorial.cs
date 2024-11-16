using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateTutorial : Tutorial
{
    public Enemytype[] enemyTypes;
    int enemyCount = 0;
    public int[] enemyCounts;
    public float enemyDelay = 1;

    public float remainTime = 7;

    bool active = false;

    int startKills;

    public DivineBlock leftBlock;
    public DivineBlock rightBlock;
    bool done = false;

    public GameObject[] destroys;

    // Start is called before the first frame update
    void Start()
    {
        startKills = WaveManager.kills;

        for (int i = 0; i < enemyCounts.Length; i++)
        {
            enemyCount += enemyCounts[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (enemyCount > manager.EnemyCount())
            {
                manager.Spawn(enemyTypes);
            }

            if (!leftBlock && !rightBlock)
            {
                print("DOne");
                done = true;
                active = false;
            }
        }
        else if (done)
        {
            remainTime -= Time.deltaTime;
            if (remainTime < 0)
            {
                Complete();
            }
        }
    }

    public override void Activate()
    {
        base.Activate();

        manager.Spawn(enemyTypes, enemyCounts, enemyDelay);
        Invoke("isActive", 1);
    }

    void isActive()
    {
        active = true;
    }

    public override void Complete()
    {
        base.Complete();

        done = false;

        foreach (GameObject go in destroys)
        {
            if (go)
            {
                Destroy(go);
            }
        }

        BasicHealth[] enemies = FindObjectsOfType<BasicHealth>();
        foreach (BasicHealth enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
    }
}
