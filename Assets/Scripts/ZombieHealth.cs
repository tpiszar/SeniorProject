using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : BasicHealth
{
    public NavMeshAgent agent;
    public EnemyAI enemyAI;
    public Animator animator;
    public float minSpeed;
    float maxSpeed;
    public float minAttk;
    float maxAttk;

    protected override void Start()
    {
        maxSpeed = agent.speed;
        maxAttk = enemyAI.attkRate;

        base.Start();
    }

    public override void TakeDamage(int damage, DamageType type)
    {
        base.TakeDamage(damage, type);

        if (!agent)
        {
            return;
        }

        float thresholdHealth = (minSpeed / maxSpeed) * maxHealth;
        float adjustedHealth = Mathf.Clamp(health - thresholdHealth, 0, maxHealth - thresholdHealth);
        float ratio = adjustedHealth / (maxHealth - thresholdHealth);

        baseSpeed = Mathf.Lerp(minSpeed, maxSpeed, ratio);
        agent.speed = baseSpeed;
        enemyAI.attkRate = Mathf.Lerp(minAttk, maxAttk, ratio);

        animator.SetFloat("AttackRate", 1 / (enemyAI.attkRate / enemyAI.attackAnimDuration));
    }
}
