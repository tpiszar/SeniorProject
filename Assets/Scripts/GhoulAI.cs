using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhoulAI : EnemyAI
{
    public EnemyBarrier barrier;
    public bool barrierUp = false;

    public float restoreRate;
    float nextRestore;

    bool restoring = false;

    float speed;

    public bool matchWithAttack = true;

    protected override void Start()
    {
        base.Start();
        nextRestore = restoreRate / 3 + UnityEngine.Random.Range(-0.25f, 1f);
        speed = agent.speed;
    }

    protected override void Update()
    {
        if (restoring)
        {
            return;
        }

        base.Update();

        if (!barrierUp)
        {
            nextRestore -= Time.deltaTime;
        }
        if (!curAttackObj && nextRestore < 0)
        {
            restoring = true;
            agent.enabled = false;
            if (matchWithAttack)
            {
                animator.SetTrigger("Attack");

                attackSound.Play();
            }
            else
            {
                animator.SetTrigger("Barrier");

                attackSound.Play();
            }

            nextRestore = restoreRate;
        }
    }

    public override void Hit()
    {
        if (!this)
        {
            return;
        }

        if (restoring)
        {
            restoring = false;
            barrier.Restore();

            return;
        }

        base.Hit();
    }

    public override void AttackDone()
    {
        agent.enabled = true;
        Locate();
    }
}
