using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlameTower : MonoBehaviour
{
    public BasicTowerDetection detector;
    public Transform flameBlast;
    TowerFlame flame;
    public float fireRate;
    [Range(0f, 1f)]
    public float expandMod; // Should be less than fire rate
    float nextShot = 0;

    float blastRadius;
    float expandRate;

    public AudioSource blastSound;

    // Start is called before the first frame update
    void Start()
    {
        blastRadius = GetComponent<SphereCollider>().radius * 2;
        expandRate = fireRate * expandMod;

        flame = flameBlast.GetComponent<TowerFlame>();
    }

    // Update is called once per frame
    void Update()
    {
        nextShot -= Time.deltaTime;
        if (nextShot <= 0f)
        {
            if (!detector.isEmpty())
            {
                blastSound.Play();

                StartCoroutine(ExpandFlame());
                nextShot = fireRate;
            }
        }
    }

    IEnumerator ExpandFlame()
    {
        flame.hitThisCycle.Clear();
        flame.soundCount = 0;
        float timer = 0;
        while (timer < expandRate)
        {
            timer += Time.deltaTime;

            flameBlast.localScale = Vector3.one * Mathf.Lerp(0, blastRadius, timer / expandRate);

            yield return null;
        }
        flameBlast.localScale = Vector3.zero;
    }
}
