using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedOrbHealth : BasicHealth
{
    public Color hurtColor;

    public override int GetHealth()
    {
        return int.MaxValue - maxHealth + base.GetHealth();
    }

    protected override IEnumerator DamageFlash()
    {
        mainColor = Color.Lerp(hurtColor, Color.red, (float)health / maxHealth);
        return base.DamageFlash();
    }
}
