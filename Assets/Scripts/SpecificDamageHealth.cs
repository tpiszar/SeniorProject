using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificDamageHealth : BasicHealth
{
    public bool energy;
    public bool fire;
    public bool lightning;

    public override EnemyBarrier TakeDamage(int damage, DamageType type)
    {
        if (type == DamageType.energy && !energy)
        {
            return null;
        }
        if (type == DamageType.fire && !fire)
        {
            return null;
        }
        if (type == DamageType.lightning && !lightning)
        {
            return null;
        }
        return(base.TakeDamage(damage, type));
    }

    public override void Burn(float duration, float rate, int tick)
    {
        if (!fire)
        {
            return;
        }

        base.Burn(duration, rate, tick);
    }
}
