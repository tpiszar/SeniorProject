using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedOrbAI : EnemyAI
{
    // Update is called once per frame
    protected override void Update()
    {
        nextAttk = attkRate;

        base.Update();
    }

    public override float GetDistance()
    {
        return -10000 + base.GetDistance();
    }
}
