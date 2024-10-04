using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeMove : MonoBehaviour
{
    public float minSpeed = 0.5f;
    public float maxSpeed = 2.0f;

    public float lerpTime = 2.0f;
    float curLerp = 0;
    bool rising = true;
    
    public Vector3 minSize = Vector3.one;
    public Vector3 maxSize = new Vector3(1.2f, 1.1f, 1.2f);

    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.velocity.magnitude > 0.2f) 
        {
            if (rising)
            {
                curLerp += Time.deltaTime;
                if (curLerp > lerpTime)
                {
                    rising = false;
                }
            }
            else
            {
                curLerp -= Time.deltaTime;
                if (curLerp < 0)
                {
                    rising = true;
                }
            }
        }
        else
        {
            rising = false;
            curLerp -= Time.deltaTime;
        }

        transform.localScale = Vector3.Lerp(minSize, maxSize, curLerp / lerpTime);
        agent.speed = Mathf.Lerp(minSpeed, maxSpeed, curLerp / lerpTime);
    }
}
