using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanoidAnim : MonoBehaviour
{
    public NavMeshAgent agent;
    public EnemyAI enemyAI;
    public Animator animator;

    public float optimalSpeed = 3;
    float speedMod;
    float startSpeed;

    // Start is called before the first frame update
    void Start()
    {
        speedMod = agent.speed / optimalSpeed;
        startSpeed = agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent)
        {
            animator.SetFloat("Speed", 0);
            animator.SetFloat("X", 0);
            animator.SetFloat("Y", 0);

            Destroy(this);

            return;
        }

        Vector3 direction1 = (enemyAI.travellingDir - transform.position);
        direction1.y = 0;
        direction1.Normalize();

        Vector3 direction = transform.InverseTransformDirection(agent.velocity.normalized); // Maybe better?
        //print(direction1 + " 0: " + direction);
        //print(agent.velocity.magnitude + " / " + agent.speed + " = " + agent.velocity.magnitude / agent.speed);

        speedMod = agent.speed / optimalSpeed;

        animator.SetFloat("SpeedRatio", agent.velocity.magnitude / startSpeed);
        animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed * speedMod);
        animator.SetFloat("X", direction1.x);
        animator.SetFloat("Y", direction1.z);
    }
}
