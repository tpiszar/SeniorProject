using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedOrbHealth : BasicHealth
{
    public Color hurtColor;
    public GameObject deathShatter;

    public override int GetHealth()
    {
        return int.MaxValue - maxHealth + base.GetHealth();
    }

    protected override IEnumerator DamageFlash()
    {
        for (int i = 0; i < matColors.Length; i++)
        {
            matColors[i] = Color.Lerp(hurtColor, Color.red, (float)health / maxHealth);
        }

        return base.DamageFlash();
    }

    protected override void OnDestroy()
    {
        if (UIScript.sceneLoading) return;

        if (health > 0)
        {
            annihilateParticle.transform.parent = null;
            annihilateParticle.Play();

            SoundManager.instance.Annihilate(transform.position);
        }
        else
        {
            Instantiate(deathShatter, animator.transform.position, animator.transform.rotation);
        }
    }
}
