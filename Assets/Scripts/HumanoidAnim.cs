using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanoidAnim : MonoBehaviour
{
    public NavMeshAgent agent;
    public EnemyAI enemyAI;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {

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

        animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
        animator.SetFloat("X", direction1.x);
        animator.SetFloat("Y", direction1.z);
    }
}
