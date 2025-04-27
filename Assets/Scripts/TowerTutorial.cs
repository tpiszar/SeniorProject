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

    public GameObject[] forceDisable;

    // Start is called before the first frame update
    void Start()
    {
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
            //print("KILLS: " + WaveManager.kills + " - START:" + startKills + " =? COUNT" + enemyCount);
            if (WaveManager.kills - startKills == enemyCount)
            {
                Complete();
            }
            else if (enemyCount - (WaveManager.kills - startKills) > manager.EnemyCount())
            {
                manager.Spawn(enemyTypes);
            }
        }
    }

    public override void Activate()
    {
        startKills = WaveManager.kills;

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
        CancelInvoke();

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

        foreach (GameObject go in forceDisable)
        {
            if (go)
            {
                go.SetActive(false);
            }
        }

        //ADDED
        EnemyAI[] survivors = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI enemy in survivors)
        {
            Destroy(enemy.gameObject);
        }

        Hands();
    }
}
