using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleManager : WaveManager
{
    public GameObject[] enemyPrefabs;

    // Start is called before the first frame update
    protected override void Start()
    {
        ShadeAI.setter = false;

        foreach (Transform playerBase in Teleport.Instance.bases)
        {
            maxDistances.Add(CalculatePathLength(spawnPoint.position, playerBase.position));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Spawn(Enemytype enemy)
    {
        EnemyAI newEn = Instantiate(enemyPrefabs[(int)enemy], spawnPoint.position, Quaternion.identity).GetComponent<EnemyAI>();
        newEn.player = player;
        enemies.Add(newEn);
        healths.Add(newEn.GetComponent<BasicHealth>());
    }

    public void Spawn(Enemytype enemy, int num, float interval)
    {
        for (int i = 0; i < num; i++)
        {
            StartCoroutine(DelaySpawn(enemy, interval * i));
        }
    }


    int cur = 0;
    public void Spawn(Enemytype[] enemies)
    {
        Spawn(enemies[cur]);

        cur++;
        if (cur >= enemies.Length)
        {
            cur = 0;
        }
    }

    public void Spawn(Enemytype[] enemies, int[] nums, float interval)
    {
        if (enemies.Length != nums.Length)
        {
            print("Simple Spawn was given an incorrect ratio of enemies to counts");
            return;
        }

        int total = 0;

        for (int i = 0; i < nums.Length; i++)
        {
            total += nums[i];
        }

        cur = 0;
        for (int i = 0; i < total; i++)
        {
            StartCoroutine(DelaySpawn(enemies[cur], interval * i));
            cur++;
            if (cur >= enemies.Length)
            {
                cur = 0;
            }
        }
        cur = 0;
    }

    IEnumerator DelaySpawn(Enemytype enemy, float delay)
    {
        yield return new WaitForSeconds(delay);
        Spawn(enemy);
    }

    public void CompleteTutorial()
    {
        switch (level)
        {
            case 0:
                SaveLoad.level1TutorialDone = true;
                break;
            case 1:
                SaveLoad.level2TutorialDone = true;
                break;
            case 2:
                SaveLoad.level3TutorialDone = true;
                break;
        }

        winSound.Play();
    }
}
