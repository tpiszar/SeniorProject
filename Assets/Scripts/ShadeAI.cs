using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ShadeAI : EnemyAI
{
    public static bool setter = false;

    public Transform root;
    public Transform shadeRoot;
    [Range(0, 1)]
    public float minFade = 0.2f;
    [Range(0, 1)]
    public float maxFade = 0.8f;
    [Range(0, 1)]
    public float distanceActivateRatio = 0.5f;
    public bool active = false;

    public SkinnedMeshRenderer mesh;

    Color matColor;

    float curMaxDist;

    // Start is called before the first frame update
    protected override void Start()
    {
        root.gameObject.SetActive(false);

        active = false;

        //if (WaveManager.maxDistances.Count == 0 && !setter)
        //{

        //    WaveManager.maxDistances.Add(0);
        //    print("RUN");
        //    setter = true;
        //    active = true;
        //    StartCoroutine(SetMaxDistances());
        //    //foreach (Transform playerBase in Teleport.Instance.bases)
        //    //{
        //    //    nextDist = 0;
        //    //    agent.SetDestination(playerBase.position);
        //    //    WaveManager.maxDistances.Add(GetDistance());
        //    //    print(GetDistance());
        //    //}
        //}

        //curMaxDist = 10000;

        curMaxDist = WaveManager.maxDistances[Teleport.Instance.curBase];

        matColor = mesh.materials[1].color;

        base.Start();
    }

    //IEnumerator SetMaxDistances()
    //{
    //    foreach (Transform playerBase in Teleport.Instance.bases)
    //    {
    //        float maxDist = 0;
    //        while (maxDist == 0)
    //        {
    //            nextDist = 0;
    //            agent.SetDestination(playerBase.position);
    //            maxDist = GetDistance();
    //            yield return null;
    //        }

    //        WaveManager.maxDistances.Add(maxDist);
    //    }

    //    agent.speed = speed;

    //    curMaxDist = WaveManager.maxDistances[Teleport.Instance.curBase];

    //    Locate();

    //    active = false;
    //}

    protected override void Update()
    {
        if (!active)
        {
            float ratioDist = curMaxDist * distanceActivateRatio;
            if (GetDistance() > 5 && distance < ratioDist)
            {
                shadeRoot.gameObject.SetActive(false);
                root.gameObject.SetActive(true);

                matColor.a = 1;
                mesh.materials[1].color = matColor;

                active = true;

                MiniMapTracker.instance.AddMapTracker(transform, Enemytype.Shade);
            }
            else
            {
                float alpha = Mathf.Lerp(maxFade, minFade, (distance - ratioDist) / ratioDist);

                matColor.a = alpha;
                mesh.materials[1].color = matColor;
            }

            if (active)
            {
                WaveManager.Instance.enemies.Add(this);
                WaveManager.Instance.healths.Add(GetComponent<BasicHealth>());
            }
        }

        base.Update();
    }

    //void SetAlphas(float alpha)
    //{
    //    foreach (SkinnedMeshRenderer mesh in meshs)
    //    {
    //        Color col = mesh.material.color;
    //        col.a = alpha;
    //        mesh.material.color = col;
    //    }
    //}

    protected override void OnTeleport(float teleportTime)
    {
        curMaxDist = WaveManager.maxDistances[Teleport.Instance.curBase];

        base.OnTeleport(teleportTime);
    }
}
