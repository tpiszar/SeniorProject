using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawn : MonoBehaviour
{
    public float growDuration = 1f;
    public float landDuration = 0.25f;
    public float riseHeight = 0.75f;

    Vector3 startPos;
    Vector3 risePos;

    Vector3 sizeVector = Vector3.one;

    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        sizeVector = transform.localScale;

        transform.localScale = Vector3.zero;

        startPos = transform.position;

        risePos = startPos;
        risePos.y += riseHeight;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer < growDuration)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, sizeVector, timer / growDuration);
            transform.position = Vector3.Lerp(startPos, risePos, timer / growDuration);
        }
        else if (timer < growDuration + landDuration)
        {
            transform.localScale = sizeVector;
            transform.position = Vector3.Lerp(risePos, startPos, (timer - growDuration) / landDuration);
        }
        else
        {
            transform.position = startPos;
            transform.localScale = sizeVector;
            Destroy(this);
        }
    }
}
