using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndlessManager : WaveManager
{
    public TextMeshProUGUI curWaveTxt;
    public TextMeshProUGUI finalWaveTxt;

    [SerializeField]
    RandomWave wave;

    bool nextInitialized = false;

    [SerializeField]
    static int spawnPoints = 30;
    [SerializeField]
    static float percIncr = 0.4f;
    int waveStartMax = 500;
    float waveOverflowDelay = 10;

    // Start is called before the first frame update
    protected override void Start()
    {
        ShadeAI.setter = false;

        foreach (Transform playerBase in Teleport.Instance.bases)
        {
            maxDistances.Add(CalculatePathLength(spawnPoint.position, playerBase.position));
        }

        curWave = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Time.time > nextWave && !LevelEnd)
        {
            CleanUp();
            if (enemies.Count > waveStartMax)
            {
                nextWave = Time.time + waveOverflowDelay;
                return;
            }

            print("Preparing wave " + (curWave + 1) + " with " + spawnPoints + " spawn points");

            nextWave = Time.time + wave.GenerateRandomWave(spawnPoints) + 0.2f;

            curWave++;
            float newPoints = (1 + percIncr) * spawnPoints;
            spawnPoints = (int)newPoints;

            curWaveTxt.text = curWave.ToString();

            finalWaveTxt.text = "You survived " + (curWave - 1).ToString() + " waves!";
        }
    }
}
