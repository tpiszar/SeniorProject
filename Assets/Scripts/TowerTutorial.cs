using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTutorial : Tutorial
{
    public Enemytype[] enemyTypes;
    int enemyCount = 0;
    public int[] enemyCounts;
    public float enemyDelay = 3;

    bool active = false;

    int startKills;

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
            if (WaveManager.kills - startKills == enemyCount)
            {
                Complete();
            }
            else if (enemyCount > manager.EnemyCount())
            {
                manager.Spawn(enemyTypes);
            }
        }
    }

    public override void Activate()
    {
        base.Activate();

        manager.Spawn(enemyTypes, enemyCounts, enemyDelay);
        Invoke("isActive", enemyDelay * enemyCount + 3);
    }

    void isActive()
    {
        active = true;
    }

    public override void Complete()
    {
        base.Complete();

        active = false;

        CreateTower[] minis = FindObjectsOfType<CreateTower>();
        foreach (CreateTower mini in minis)
        {
            if (mini.gameObject.activeSelf)
            {
                Destroy(mini.gameObject);
            }
        }

        TowerDetection[] towers = FindObjectsOfType<TowerDetection>();
        foreach (TowerDetection tower in towers)
        {
            Destroy(tower.gameObject);
        }
    }
}
