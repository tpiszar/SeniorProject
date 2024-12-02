using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.CoreUtils;

public class RandomWave : Wave
{
    static int[] pointCounts = new int[7] { 1, 3, 5, 9, 6, 7, 20 };

    public GameObject[] enemyPrefabs = new GameObject[7];

    float[,] spawnChanceMatrix = new float[7, 7] {
     //                       Slime       Skeleton      Zombie       RedOrb       Shade        Ghoul       Greater

     /*Slime*/        {       0.400f,      0.100f,      0.100f,      0.200f,      0.050f,      0.100f,      0.050f       },

     /*Skeleton*/     {       0.100f,      0.400f,      0.100f,      0.200f,      0.050f,      0.050f,      0.100f       },

     /*Zombie*/       {       0.050f,      0.100f,      0.400f,      0.200f,      0.100f,      0.100f,      0.050f       },

     /*RedOrb*/       {       0.000f,      0.000f,      0.000f,      0.000f,      0.000f,      0.000f,      0.000f       },

     /*Shade*/        {       0.040f,      0.040f,      0.040f,      0.040f,      0.700f,      0.100f,      0.040f       },

     /*Ghoul*/        {       0.000f,      0.000f,      0.000f,      0.000f,      0.000f,      0.000f,      0.000f       },

     /*Greater*/      {       0.000f,      0.000f,      0.000f,      0.000f,      0.000f,      0.000f,      0.000f       },
    };

    Enemytype lastEnemy = Enemytype.Slime;

    static int[] enemyCutoffs = new int[7] { 7, 10, 10, 20, 15, 25, 500 };

    public Vector2 sectionBounds = new Vector2(5, 20);
    public Vector2 delayBounds = new Vector2(3, 10);

    // 0-25  26-50  51-75  76-100
    // 10%    30%    20%    40%

    int excessPoints = 0;

    public float GenerateRandomWave(int spawnPoints)
    {
        int sectionCount = UnityEngine.Random.Range((int)sectionBounds.x, (int)sectionBounds.y);

        int[] pointDistribution = DistributeSpawnPoints(sectionCount, spawnPoints);

        spawns = new SpawnGroup[sectionCount];
        int previousSpawnPoints = 0;

        print("Sections: " + sectionCount + " " + pointDistribution);

        for (int i = 0; i < sectionCount; i++)
        {
            // Initialize new spawn group
            SpawnGroup group = new SpawnGroup();

            // Populate enemies and counts
            group.counts = new int[enemyPrefabs.Length];
            group.enemies = new GameObject[enemyPrefabs.Length];

            int remainingSpawnPoints = pointDistribution[i] + excessPoints;
            excessPoints = 0;

            print("Section " + i + " " + remainingSpawnPoints);

            while (remainingSpawnPoints > 0)
            {
                Enemytype chosenEnemy = ChooseEnemy(lastEnemy);

                if (WaveManager.Instance.curWave > enemyCutoffs[(int)chosenEnemy])
                {
                    continue;
                }

                int enemyIndex = (int)chosenEnemy;
                int pointsCost = pointCounts[enemyIndex];
                if (pointsCost <= remainingSpawnPoints)
                {
                    group.counts[enemyIndex]++;
                    group.enemies[enemyIndex] = enemyPrefabs[enemyIndex];
                    remainingSpawnPoints -= pointsCost;

                    if (chosenEnemy != Enemytype.GreaterSkeleton && chosenEnemy != Enemytype.Ghoul && chosenEnemy != Enemytype.RedOrb)
                    {
                        lastEnemy = chosenEnemy;
                    }

                    if (lastEnemy != Enemytype.Slime && pointsCost < pointCounts[1])
                    {
                        excessPoints = remainingSpawnPoints;
                        remainingSpawnPoints = 0;
                    }
                }
            }

            // Calculate delay based on the change in spawn points
            group.delay = CalculateDelay(previousSpawnPoints, pointDistribution[i], delayBounds.x, delayBounds.y);

            // Store current spawn group
            spawns[i] = group;

            previousSpawnPoints = pointDistribution[i];
        }

        return InitializeWave();
    }

    private Enemytype ChooseEnemy(Enemytype lastEnemy)
    {
        float[] probabilities = new float[spawnChanceMatrix.GetLength(1)];
        for (int i = 0; i < probabilities.Length; i++)
        {
            probabilities[i] = spawnChanceMatrix[(int)lastEnemy, i];
        }

        return (Enemytype)WeightedRandom(probabilities);
    }

    private int WeightedRandom(float[] weights)
    {
        float totalWeight = 0f;
        foreach (var weight in weights)
        {
            totalWeight += weight;
        }

        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        for (int i = 0; i < weights.Length; i++)
        {
            if (randomValue < weights[i])
                return i;
            randomValue -= weights[i];
        }

        return weights.Length - 1; // Fallback
    }

    private float CalculateDelay(int previousPoints, int currentPoints, float min, float max)
    {
        if (previousPoints == 0)
        {
            return 0;
        }

        float difference = Mathf.Abs(currentPoints - previousPoints);
        float maxDifference = Mathf.Max(previousPoints, currentPoints);

        float delayFactor = difference / (maxDifference == 0 ? 1 : maxDifference);
        return Mathf.Lerp(min, max, delayFactor);
    }

    public static int[] DistributeSpawnPoints(int sections, int spawnPoints)
    {
        // Percentages for each quarter
        float[] curvePercentages = { 0.1f, 0.3f, 0.2f, 0.4f };

        if (sections < 4)
        {
            throw new ArgumentException("Sections must be at least 4 to align with curve quarters.");
        }

        // Calculate the total spawn points for each quarter
        int[] quarterPoints = new int[4];
        int totalAssigned = 0;
        for (int i = 0; i < 4; i++)
        {
            quarterPoints[i] = Mathf.RoundToInt(spawnPoints * curvePercentages[i]);
            totalAssigned += quarterPoints[i];
        }

        // Adjust for rounding differences to match total spawn points
        int leftoverPoints = spawnPoints - totalAssigned;
        for (int i = 0; i < Math.Abs(leftoverPoints); i++)
        {
            int index = i % 4;
            quarterPoints[index] += Math.Sign(leftoverPoints);
        }

        // Distribute quarter points into sections
        int[] sectionPoints = new int[sections];
        int sectionsPerQuarter = sections / 4;
        for (int quarter = 0; quarter < 4; quarter++)
        {
            int startSection = quarter * sectionsPerQuarter;
            int endSection = (quarter == 3) ? sections : startSection + sectionsPerQuarter;

            int points = quarterPoints[quarter];
            int pointsPerSection = points / (endSection - startSection);
            int extraPoints = points % (endSection - startSection);

            for (int i = startSection; i < endSection; i++)
            {
                sectionPoints[i] = pointsPerSection;
                if (extraPoints > 0)
                {
                    sectionPoints[i]++;
                    extraPoints--;
                }
            }
        }

        return sectionPoints;
    }
}
