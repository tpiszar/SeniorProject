using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WaveManager : MonoBehaviour
{
    int curWave = 0;

    Wave[] waves;

    float nextWave;

    public Transform player;
    public Transform spawnPoint;
    public List<EnemyAI> enemies = new List<EnemyAI>();

    public float winDelay;

    public GameObject winScreen;
    public GameObject leftRayInteractor;
    public GameObject rightRayInteractor;
    public GameObject barrierSphere;

    public static bool LevelEnd = false;

    private void Awake()
    {
        LevelEnd = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        waves = GetComponentsInChildren<Wave>();
        nextWave = 0;
        //nextWave = waves[0].InitializeWave();
    }

    public void CleanUp()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (!enemies[i])
            {
                enemies.RemoveAt(i);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextWave)
        {
            if (curWave < waves.Length)
            {
                nextWave = Time.time + waves[curWave].InitializeWave() + 0.2f;
                CleanUp();
                curWave++;
            }
            else
            {
                CleanUp();
                if (enemies.Count == 0)
                {
                    //VICTORY
                    Invoke("VictoryScreen", winDelay);
                    this.enabled = false;
                }
            }
        }
    }

    public void VictoryScreen()
    {
        LevelEnd = true;

        if (barrierSphere)
        {
            barrierSphere.SetActive(false);
        }
        winScreen.SetActive(true);

        HandRay.activeHandRays = true;
        leftRayInteractor.SetActive(true);
        rightRayInteractor.SetActive(true);
    }
}
