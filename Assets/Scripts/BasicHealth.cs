using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BasicHealth : MonoBehaviour
{
    public int maxHealth;
    protected int health;

    public int manaGain;

    float burnDuration = 0;
    float burnRate = 0;
    float burnRamp = 0;
    int tickBurn = 0;

    public Renderer mainRend;
    public float flashSpeed = 0.5f;
    Color mainColor;

    public LineRenderer lightningRender;

    float delayDeath = 0;

    public enum DamageType
    {
        energy,
        fire,
        lightning
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = maxHealth;
        mainColor = mainRend.material.color;
        //flashSpeed /= 2;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (burnDuration > 0)
        {
            burnDuration -= Time.deltaTime;
            burnRamp += Time.deltaTime;
            if (burnRamp > burnRate)
            {
                TakeDamage(tickBurn, DamageType.fire);
                burnRamp = 0;
            }
        }
    }

    public virtual void TakeDamage(int damage, DamageType type)
    {
        health -= damage;
        //print(gameObject.name + ": " + health);
        if (health <= 0)
        {
            Mana.Instance.GainMana(manaGain);
            Destroy(gameObject, delayDeath);
        }
        StartCoroutine(DamageFlash());
    }

    IEnumerator DamageFlash()
    {
        float timer = 0;

        //while (timer < flashSpeed)
        //{
        //    timer += Time.deltaTime;
        //    mainRend.material.color = Color.Lerp(mainColor, Color.red, timer / flashSpeed);
        //    yield return null;
        //}

        timer = 0;
        while (timer < flashSpeed)
        {
            timer += Time.deltaTime;
            mainRend.material.color = Color.Lerp(Color.red, mainColor, timer / flashSpeed);
            yield return null;
        }
        mainRend.material.color = mainColor;
    }

    public virtual void Burn(float duration, float rate, int tick)
    {
        burnDuration = duration + 0.1f;
        burnRate = rate;
        burnRamp = 0;
        tickBurn = tick;
    }

    public virtual void Shock(int damage, float jumpMod, int jumpCount, float jumpRadius, LayerMask lightningMask, float jumpDelay = 0, Transform shocker = null)
    {
        delayDeath = jumpDelay + .1f;

        TakeDamage(damage, DamageType.lightning);

        jumpCount--;
        if (jumpCount > 0)
        {
            StartCoroutine(ShockJump((int)(damage * jumpMod + 0.5f), jumpMod, jumpCount, jumpRadius, lightningMask, jumpDelay, shocker));
        }
    }

    IEnumerator ShockJump(int damage, float jumpMod, int jumpCount, float jumpRadius, LayerMask lightningMask, float jumpDelay, Transform shocker)
    {
        yield return new WaitForSeconds(jumpDelay);

        bool shockerClose = false;
        Collider[] hits = Physics.OverlapSphere(transform.position, jumpRadius, lightningMask);

        float minDist = float.MaxValue;
        BasicHealth minEn = null;
        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i].transform.CompareTag("Root"))
            {
                continue;
            }
            BasicHealth newEn = hits[i].GetComponentInParent<BasicHealth>();

            if (newEn.transform == shocker) // Avoids reshocking the same enemy that shocked it unless there are no other options
            {
                shockerClose = true;
                continue;
            }

            if (!newEn || newEn == this)
            {
                continue;
            }

            float newDist = (transform.position - hits[i].transform.position).sqrMagnitude;
            if (newDist < minDist)
            {
                minDist = newDist;
                minEn = newEn;
            }
        }

        if (minEn)
        {
            minEn.Shock(damage, jumpMod, jumpCount, jumpRadius, lightningMask, jumpDelay, transform);

            StartCoroutine(DrawLightning(minEn.transform.position, jumpDelay));
        }
        else if (shockerClose)
        {
            BasicHealth newEn = shocker.GetComponent<BasicHealth>();
            if (newEn)
            {
                newEn.Shock(damage, jumpMod, jumpCount, jumpRadius, lightningMask, jumpDelay, transform);

                StartCoroutine(DrawLightning(newEn.transform.position, jumpDelay));
            }
        }
        else
        {
            // Could have enemy take the extra shock damage if there is nothing left to chain to?
        }
        delayDeath = 0;
    }

    IEnumerator DrawLightning(Vector3 enemy, float duration)
    {
        float distance = Vector3.Distance(transform.position, enemy);
        int numSegments = Mathf.CeilToInt(distance * Wand.lightningSpikesPerUnit);

        lightningRender.positionCount = numSegments + 2;

        lightningRender.SetPosition(0, transform.position);

        for (int i = 1; i <= numSegments; i++)
        {
            float t = (float)i / (numSegments + 1);
            Vector3 interpolatedPos = Vector3.Lerp(transform.position, enemy, t);

            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-Wand.lightningSpikeOffset, Wand.lightningSpikeOffset),
                UnityEngine.Random.Range(-Wand.lightningSpikeOffset, Wand.lightningSpikeOffset),
                UnityEngine.Random.Range(-Wand.lightningSpikeOffset, Wand.lightningSpikeOffset)
            );

            lightningRender.SetPosition(i, interpolatedPos + randomOffset);
        }
        lightningRender.SetPosition(lightningRender.positionCount - 1, enemy);

        lightningRender.enabled = true;

        yield return new WaitForSeconds(duration);

        lightningRender.enabled = false;
    }
}
