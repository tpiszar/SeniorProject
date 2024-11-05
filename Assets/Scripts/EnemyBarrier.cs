using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBarrier : BasicHealth
{
    public GhoulAI ghoul;

    protected override void Start()
    {
        health = maxHealth;
    }

    public override EnemyBarrier TakeDamage(int damage, DamageType type)
    {
        WaveManager.totalDamage += damage;
        //print(WaveManager.totalDamage);

        health -= damage;
        //print(gameObject.name + ": " + health);
        if (health <= 0)
        {
            ghoul.barrierUp = false;
            gameObject.SetActive(false);
        }

        return this;
    }

    public override void Burn(float duration, float rate, int tick)
    {
        return;
    }

    public override EnemyBarrier Shock(int damage, float jumpMod, int jumpCount, float jumpRadius, LayerMask lightningMask, LightningDrawer drawer, float jumpDelay = 0, Transform shocker = null)
    {
        return TakeDamage(damage, DamageType.lightning);
    }

    public void Restore()
    {
        health = maxHealth;
        ghoul.barrierUp = true;
        gameObject.SetActive(true);
    }
}
