using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WardTower : MonoBehaviour
{
    Transform player;
    public int healAmount;
    public List<BarrierSpawn> spawns = new List<BarrierSpawn>();
    public int barrierCount = 3;
    public float burySize = 0.25f;
    public GameObject barrierPrefab;
    public float distanceThreshold;
    public float maxDistance;

    public float chargeRate;
    float nextCharge;

    int used = 0;
    float spawnHeight = 0;

    public bool prioGenerate = false;

    public class BarrierSpawn : IComparable
    {
        Vector3 spawnPoint;
        public Barrier barrier;
        public float distance;

        public BarrierSpawn(Vector3 spawnPoint, float distance)
        {
            this.spawnPoint = spawnPoint;
            this.distance = distance;
        }

        public bool HasActiveBarrier()
        {
            return barrier;
        }

        public void Spawn(GameObject prefab)
        {
            GameObject newBarrier = Instantiate(prefab, spawnPoint, Quaternion.identity);
            barrier = newBarrier.GetComponent<Barrier>();
        }

        public void Boost(int amount)
        {
            barrier.GainHealth(amount);
        }

        public bool IsOptimal(int amount)
        {
            return barrier.CanMaximizeCharge(amount);
        }

        public int CompareTo(object obj)
        {
            BarrierSpawn other = obj as BarrierSpawn;
            if (distance < other.distance)
            {
                return 1;
            }
            if (distance > other.distance)
            {
                return -1;
            }

            return 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnHeight = barrierPrefab.transform.lossyScale.y / 2 - burySize;
        nextCharge = chargeRate / 2;

        player = Camera.main.transform;

        if (Teleport.Instance)
        {
            Teleport.Instance.onTeleport += OnTeleport;
        }

        SetWardPositions();
    }

    int cycle = 0;
    // Update is called once per frame
    void Update()
    {
        nextCharge -= Time.deltaTime;
        if (nextCharge < 0)
        {
            if (prioGenerate)
            {
                for (int i = 0; i < spawns.Count; i++)
                {
                    if (!spawns[i].HasActiveBarrier())
                    {
                        spawns[i].Spawn(barrierPrefab);
                        nextCharge = chargeRate;
                        return;
                    }
                }
                for (int i = 0; i < spawns.Count; i++)
                {
                    if (spawns[i].IsOptimal(healAmount))
                    {
                        spawns[i].Boost(healAmount);
                        nextCharge = chargeRate;
                        return;
                    }
                }
                spawns[0].Boost(healAmount);
                nextCharge = chargeRate;
            }
            else
            {
                if (cycle >= spawns.Count)
                {
                    cycle = 0;
                }
                if (!spawns[cycle].HasActiveBarrier())
                {
                    spawns[cycle].Spawn(barrierPrefab);
                    nextCharge = chargeRate;
                    cycle++;
                }
                else
                {
                    if (spawns[cycle].IsOptimal(healAmount))
                    {
                        spawns[cycle].Boost(healAmount);
                        nextCharge = chargeRate;
                        cycle++;
                    }
                    else
                    {
                        for (int i = 0; i < spawns.Count; i++)
                        {
                            if (spawns[i].IsOptimal(healAmount))
                            {
                                spawns[i].Boost(healAmount);
                                nextCharge = chargeRate;
                                cycle++;
                                return;
                            }
                        }
                        spawns[cycle].Boost(healAmount);
                        nextCharge = chargeRate;
                        cycle++;
                    }
                }

            }
        }
    }

    float CalculatePathLength(Vector3 start, Vector3 end)
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

    void SetWardPositions()
    {
        foreach (Vector3 pos in MapPath.Instance.GetClosestPoints(transform.position, distanceThreshold))
        {
            if (Vector3.Distance(pos, transform.position) < maxDistance)
            {
                if (used < barrierCount)
                {
                    used++;

                    Vector3 newPoint = pos;
                    newPoint.y += spawnHeight;
                    float distance = CalculatePathLength(newPoint, player.position);

                    spawns.Add(new BarrierSpawn(newPoint, distance));
                }
                else
                {
                    break;
                }
            }
        }

        if (spawns.Count == 0)
        {
            Destroy(this);
        }

        spawns.Sort();
    }

    void OnTeleport(float teleportTime)
    {
        spawns.Sort();
    }

    private void OnDestroy()
    {
        if (Teleport.Instance)
        {
            Teleport.Instance.onTeleport -= OnTeleport;
        }
    }
}
