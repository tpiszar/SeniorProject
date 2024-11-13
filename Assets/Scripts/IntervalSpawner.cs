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
    public float startDelay;

    // Start is called before the first frame update
    void Start()
    {
        nextSpawn = startDelay;
    }

    // Update is called once per frame
    void Update()
    {
        nextSpawn -= Time.deltaTime;
        if (nextSpawn < 0)
        {
            GameObject newSLime = Instantiate(slimePrefab, transform.position, Quaternion.identity);
            MiniMapTracker.instance.AddMapTracker(newSLime.transform, Enemytype.Slime);
            nextSpawn = Mathf.Lerp(startInterval, endInterval, Time.time - startDelay / rampTime);
        }
    }
}
