using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificDamageHealth : BasicHealth
{
    public bool energy;
    public bool fire;
    public bool lightning;

    public override void TakeDamage(int damage, DamageType type)
    {
        if (type == DamageType.energy && !energy)
        {
            return;
        }
        if (type == DamageType.fire && !fire)
        {
            return;
        }
        if (type == DamageType.lightning && !lightning)
        {
            return;
        }
        base.TakeDamage(damage, type);
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
