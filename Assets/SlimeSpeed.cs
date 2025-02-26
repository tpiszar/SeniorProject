using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeSpeed : MonoBehaviour
{
    public NavMeshAgent agent;

    public float slowPerc = 0.8f;

    public void SlowDown()
    {
        agent.speed *= (1 - slowPerc);
    }
    
    public void SpeedUp()
    {
        agent.speed /= (1 - slowPerc);
    }
}
