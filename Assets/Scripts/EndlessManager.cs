using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessManager : WaveManager
{
    [SerializeField]
    RandomWave wave;

    bool nextInitialized = false;

    [SerializeField]
    int spawnPoints;
    [SerializeField]
    float percIncr = 0.5f;
    public int waveStartMax = 500;
    public float waveOverflowDelay = 10;

    // Start is called before the first frame update
    protected override void Start()
    {
        ShadeAI.setter = false;

        foreach (Transform playerBase in Teleport.Instance.bases)
        {
            maxDistances.Add(CalculatePathLength(spawnPoint.position, playerBase.position));
        }

        curWave = 1;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Time.time > nextWave)
        {
            print("Wave " + curWave + " Start");
            CleanUp();
            if (enemies.Count > waveStartMax)
            {
                nextWave = Time.time + waveOverflowDelay;
                return;
            }
            nextWave = Time.time + wave.GenerateRandomWave(spawnPoints) + 0.2f;
            curWave++;
            float newPoints = (1 + percIncr) * spawnPoints;
            spawnPoints = (int)newPoints;
        }
    }
}
