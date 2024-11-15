using System.Collections;
using System.Collections.Generic;
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
        GameObject newEn = Instantiate(enemyPrefabs[(int)enemy], spawnPoint.position, Quaternion.identity);
        newEn.GetComponent<EnemyAI>().player = player;
    }

    public void Spawn(Enemytype enemy, int num, float interval)
    {
        for (int i = 0; i < num; i++)
        {
            DelaySpawn(enemy, interval * i);
        }
    }


    int cur = 0;
    public void Spawn(Enemytype[] enemies)
    {
        GameObject newEn = Instantiate(enemyPrefabs[(int)enemies[cur]], spawnPoint.position, Quaternion.identity);
        newEn.GetComponent<EnemyAI>().player = player;
        cur++;
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
            DelaySpawn(enemies[cur], interval * i);
            cur++;
            if (cur == enemies.Length)
            {
                cur = 0;
            }
        }
        cur = 0;
    }

    IEnumerator DelaySpawn(Enemytype enemy, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject newEn = Instantiate(enemyPrefabs[(int)enemy], spawnPoint.position, Quaternion.identity);

        newEn.GetComponent<EnemyAI>().player = player;
    }
}
