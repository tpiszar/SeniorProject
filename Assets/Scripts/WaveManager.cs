using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class WaveManager : MonoBehaviour
{
    public int level = 0;

    public int curWave = 0;

    Wave[] waves;

    protected float nextWave;

    public Transform player;
    public Transform spawnPoint;
    public List<EnemyAI> enemies = new List<EnemyAI>();
    public List<BasicHealth> healths = new List<BasicHealth>();

    public float winDelay;

    public UIScript mainUI;
    public GameObject leftRayInteractor;
    public GameObject rightRayInteractor;
    public GameObject barrierSphere;

    public static bool LevelEnd = false;
    public static List<float> maxDistances;

    public static WaveManager Instance;

    public static int kills;

    public AudioSource winSound;

    private void Awake()
    {
        LevelEnd = false;

        Instance = this;

        maxDistances = new List<float>();

        kills = 0;

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
    protected virtual void Start()
    {
        ShadeAI.setter = false;
        waves = GetComponentsInChildren<Wave>();
        nextWave = 0;
        //nextWave = waves[0].InitializeWave();

        foreach (Transform playerBase in Teleport.Instance.bases)
        {
            maxDistances.Add(CalculatePathLength(spawnPoint.position, playerBase.position));
        }
    }

    protected float CalculatePathLength(Vector3 start, Vector3 end)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path))
        {
            float length = 0f;
            for (int i = 1; i < path.corners.Length; i++)
            {
                length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            return length;
        }
        return 0f;
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

    public int EnemyCount()
    {
        CleanUp();
        return enemies.Count;
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
    protected virtual void Update()
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

                    winSound.Play();

                    switch(level)
                    {
                        case 0:
                            SaveLoad.level1Done = true;
                            break;
                        case 1:
                            SaveLoad.level2Done = true;
                            break;
                        case 2:
                            SaveLoad.level3Done = true;
                            break;
                    }

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
        //winScreen.SetActive(true);

        mainUI.SetScreen("Win");
        HandRay.activeHandRays = true;
        //leftRayInteractor.SetActive(true);
        //darightRayInteractor.SetActive(true);
    }
}
