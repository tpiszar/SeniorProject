using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntervalSpawner : MonoBehaviour
{
    public GameObject slimePrefab;
    public float startInterval;
    public float endInterval;
    public float rampTime;
    float nextSpawn;

    // Start is called before the first frame update
    void Start()
    {
        nextSpawn = endInterval;
    }

    // Update is called once per frame
    void Update()
    {
        nextSpawn -= Time.deltaTime;
        if (nextSpawn < 0)
        {
            Instantiate(slimePrefab, transform.position, Quaternion.identity);
            nextSpawn = Mathf.Lerp(startInterval, endInterval, Time.time / rampTime);
        }
    }
}
