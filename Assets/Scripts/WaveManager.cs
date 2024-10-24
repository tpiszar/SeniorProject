using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<BasicHealth> healths = new List<BasicHealth>();

    public float winDelay;

    public GameObject winScreen;
    public GameObject leftRayInteractor;
    public GameObject rightRayInteractor;
    public GameObject barrierSphere;

    public static bool LevelEnd = false;

    public static WaveManager Instance;

    private void Awake()
    {
        LevelEnd = false;

        Instance = this;

        //if (!Instance)
        //{
        //    Instance = this;
        //}
        //else
        //{
        //    Destroy(this);
        //}
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
                healths.RemoveAt(i);
            }
        }
    }

    public Transform GetTarget(DetectionType detectionType)
    {
        CleanUp();

        if (enemies.Count == 0)
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
                int maxIndex = 0;

                for (int i = 0; i < healths.Count; i++)
                {
                    if (healths[i] == healths[maxIndex])
                    {
                        continue;
                    }

                    float nextHealth = healths[i].GetHealth();
                    if (nextHealth > maxHealth)
                    {
                        maxHealth = nextHealth;
                        maxIndex = i;
                    }
                    else if (nextHealth == maxHealth)
                    {
                        // Resorts to closet if enemies have same health

                        if (enemies[i].GetDistance() < enemies[maxIndex].GetDistance())
                        {
                            maxHealth = nextHealth;
                            maxIndex = i;
                        }
                    }
                }

                return healths[maxIndex].transform;
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (waves.Length == 0)
        {
            return;
        }

        if (Time.time > nextWave)
        {
            if (curWave < waves.Length)
            {
                nextWave = Time.time + waves[curWave].InitializeWave() + 0.2f;
                CleanUp();
                curWave++;

                if (curWave == waves.Length)
                {
                    nextWave += 10;
                }
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

    public static int totalDamage = 0;

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
