using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GreaterAura : MonoBehaviour
{
    public float duration = 20;

    public Wand wand;

    public ParticleSystem auraSystem;

    public GameObject greatBlast;
    public GameObject greatFireball;

    public int greaterLightningDamage;
    public float greaterLightningRange = 50;
    public float greaterLightningRadius;
    public float greaterMaxChargeMod = 2;
    public int greaterJumpCount = 3;
    public float greaterJumpMod = 0.5f;
    public float greaterJumpRadius;

    GameObject baseBlast;
    GameObject baseFireball;

    int baseLightningDamage;
    float baseLightningRange;
    float baseLightningRadius;
    float baseMaxChargeMod;
    int baseJumpCount;
    float baseJumpMod;
    float baseJumpRadius;

    // Start is called before the first frame update
    void Start()
    {
        baseBlast = wand.spells[0].attackPrefab;
        baseFireball = wand.spells[1].attackPrefab;

        baseLightningDamage = wand.lightningDamage;
        baseLightningRange = wand.lightningRange;
        baseLightningRadius = wand.lightningRadius;
        baseMaxChargeMod = wand.maxChargeMod;
        baseJumpCount = wand.jumpCount;
        baseJumpMod = wand.jumpMod;
        baseJumpRadius = wand.jumpRadius;
    }

    public void Activate()
    {
        auraSystem.Play();

        wand.spells[0].attackPrefab = greatBlast;
        wand.spells[1].attackPrefab = greatFireball;

        wand.lightningDamage = greaterLightningDamage;
        wand.lightningRange = greaterLightningRange;
        wand.lightningRadius = greaterLightningRadius;
        wand.maxChargeMod = greaterMaxChargeMod;
        wand.jumpCount = greaterJumpCount;
        wand.jumpMod = greaterJumpMod;
        wand.jumpRadius = greaterJumpRadius;

        StartCoroutine(RunTime());
    }

    IEnumerator RunTime()
    {
        yield return new WaitForSeconds(duration);
        Deactivate();

        Destroy(this);
    }


    void Deactivate()
    {
        auraSystem.Stop();

        wand.spells[0].attackPrefab = baseBlast;
        wand.spells[1].attackPrefab = baseFireball;

        wand.lightningDamage = baseLightningDamage;
        wand.lightningRange = baseLightningRange;
        wand.lightningRadius = baseLightningRadius;
        wand.maxChargeMod = baseMaxChargeMod;
        wand.jumpCount = baseJumpCount;
        wand.jumpMod = baseJumpMod;
        wand.jumpRadius = baseJumpRadius;
    }
}
