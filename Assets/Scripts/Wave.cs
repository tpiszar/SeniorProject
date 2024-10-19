using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    WaveManager manager;

    bool instantiatedThisFrame = false;

    [System.Serializable]
    public class SpawnGroup
    {
        [SerializeField]
        public GameObject[] enemies;
        [SerializeField]
        public int[] counts;
        public float delay;
    }

    public int startDelay;
    public SpawnGroup[] spawns;

    float teleportDelayOver;
    float teleportDelay;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponentInParent<WaveManager>();
    }

    // Update is called once per frame
    void Update()
    {
        instantiatedThisFrame = false;
    }

    public float InitializeWave()
    {
        float rampingDelay = startDelay;
        foreach (SpawnGroup spawn in spawns)
        {
            rampingDelay += spawn.delay;
            int groupCount = 1;
            for (int i = 0; i < spawn.counts.Length; i++)
            {
                groupCount += spawn.counts[i];
            }

            int curEnemy = 0;
            for (int i = 0; i < spawn.enemies.Length; i++)
            {
                for (int j = 0; j < spawn.counts[i]; j++)
                {
                    curEnemy++;
                    StartCoroutine(PreSpawn(spawn.enemies[i], (rampingDelay / groupCount) * curEnemy, rampingDelay));
                }
            }
        }
        return rampingDelay;
    }

    IEnumerator PreSpawn(GameObject enemy, float preDelay, float totalDelay)
    {
        yield return new WaitForSeconds(preDelay);
        while (instantiatedThisFrame)
        {
            yield return null;
        }
        instantiatedThisFrame = true;
        GameObject newEnemy = Instantiate(enemy, manager.spawnPoint.position, Quaternion.identity);
        newEnemy.SetActive(false);
        //print("PreSpawn: " + Time.time);
        StartCoroutine(Spawn(newEnemy, totalDelay - preDelay));
    }

    IEnumerator Spawn(GameObject enemy, float delay)
    {
        yield return new WaitForSeconds(delay);
        enemy.SetActive(true);
        enemy.name = enemy.name + " " + Time.time.ToString();
        //manager.enemies.Add(enemy.GetComponent<EnemyAI>());
        //manager.healths.Add(enemy.GetComponent<BasicHealth>());
        enemy.GetComponent<EnemyAI>().player = manager.player;

        //MiniMapTracker.instance.AddMapTracker(enemy.transform);
    }
}
