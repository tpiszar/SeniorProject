using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : BasicHealth
{
    public EnemyAI enemyAI;
    public Animator animator;
    public float minSpeed;
    float maxSpeed;
    public float minAttk;
    float maxAttk;

    protected override void Start()
    {
        base.Start();

        maxSpeed = agent.speed;
        maxAttk = enemyAI.attkRate;
    }

    public override EnemyBarrier TakeDamage(int damage, DamageType type)
    {
        EnemyBarrier barrier = base.TakeDamage(damage, type);

        if (!agent)
        {
            return barrier;
        }

        float thresholdHealth = (minSpeed / maxSpeed) * maxHealth;
        float adjustedHealth = Mathf.Clamp(health - thresholdHealth, 0, maxHealth - thresholdHealth);
        float ratio = adjustedHealth / (maxHealth - thresholdHealth);

        baseSpeed = Mathf.Lerp(minSpeed, maxSpeed, ratio);
        agent.speed = baseSpeed;
        enemyAI.attkRate = Mathf.Lerp(minAttk, maxAttk, ratio);

        animator.SetFloat("AttackRate", 1 / (enemyAI.attkRate / enemyAI.attackAnimDuration));

        return barrier;
    }

    public override void SpeedBoost(float boost)
    {
        base.SpeedBoost(boost);
        maxSpeed *= boost;
    }

    public override void SpeedDown(float boost)
    {
        base.SpeedDown(boost);
        maxSpeed /= boost;
    }

    public override void RegularSpeed()
    {
        base.RegularSpeed();
    }
}
