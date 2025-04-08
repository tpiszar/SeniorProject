using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgHealth : BasicHealth
{
    // Start is called before the first frame update
    protected override void Start()
    {
        return;
    }

    // Update is called once per frame
    void Update()
    {
        return;
    }

    public override EnemyBarrier TakeDamage(int damage, DamageType type)
    {
        return null;
    }

    public override EnemyBarrier Shock(int damage, float jumpMod, int jumpCount, float jumpRadius, LayerMask lightningMask, LightningDrawer drawer, float jumpDelay = 0, Transform shocker = null)
    {
        Destroy(gameObject);

        return null;
    }

    public override void Burn(float duration, float rate, int tick)
    {
        return;
    }

    protected override void OnDestroy()
    {
        return;
    }
}
