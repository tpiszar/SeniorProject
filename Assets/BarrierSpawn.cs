using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSpawn : MonoBehaviour
{
    public float extraSize = 0.1f;
    public float growDuration = 0.2f;
    public float shrinkDuration = 0.075f;

    public bool singleUse = true;

    Vector3 sizeVector = Vector3.one;
    Vector3 bigSize = Vector3.one;

    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        timer = 0;

        sizeVector = transform.localScale;

        bigSize = sizeVector * (1 + extraSize);

        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer < growDuration)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, bigSize, timer / growDuration);
            print("growing "  + timer / growDuration + " " + Time.time);
        }
        else if (timer < growDuration + shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(bigSize, sizeVector, (timer - growDuration) / shrinkDuration);
            print("shrinking " + (timer - growDuration)/ shrinkDuration + " " + Time.time);
        }
        else
        {
            transform.localScale = sizeVector;

            if (singleUse)
            {
                Destroy(this);
            }
        }
    }
}
