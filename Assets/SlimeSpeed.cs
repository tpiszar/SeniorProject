using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeSpeed : MonoBehaviour
{
    public NavMeshAgent agent;

    private Animator animator;

    public float slowPerc = 0.8f;

    public float optimalSpeed = 3;
    float speedMod;
    float baseSpeed;

    private void Start()
    {
        baseSpeed = agent.speed;

        speedMod = agent.speed / optimalSpeed;

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!agent)
        {
            animator.SetFloat("Speed", 0);

            Destroy(this);

            return;
        }

        animator.SetFloat("SpeedRatio", agent.velocity.magnitude / baseSpeed);
        animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed * speedMod);
    }

    public void SlowDown()
    {
        agent.speed *= (1 - slowPerc);
        baseSpeed *= (1 - slowPerc);
    }
    
    public void SpeedUp()
    {
        agent.speed /= (1 - slowPerc);
        baseSpeed /= (1 - slowPerc);
    }
}
