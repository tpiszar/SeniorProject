using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleTower : MonoBehaviour
{
    public Rigidbody[] pieces;

    public GameObject[] destroys;

    public float minForce = 1;
    public float maxForce = 10;

    public float destroyMin = 2;
    public float destroyMax = 4;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Rigidbody rig in pieces)
        {
            rig.AddForce(Random.onUnitSphere * Random.Range(minForce, maxForce), ForceMode.Impulse);
            Destroy(rig.gameObject, Random.Range(destroyMin, destroyMax));
        }

        foreach (GameObject obj in destroys)
        {
            Destroy(obj);
        }

        Destroy(gameObject, destroyMax + 1);
    }
}
