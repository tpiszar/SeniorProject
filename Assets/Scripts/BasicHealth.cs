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
    Coroutine currentFlash;
    protected Color mainColor;

    //public LineRenderer lightningRender;

    float delayDeath = 0;

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

    public virtual int GetHealth()
    {
        return health;
    }

    public virtual void TakeDamage(int damage, DamageType type)
    {
        WaveManager.totalDamage += damage;
        //print(WaveManager.totalDamage);

        health -= damage;
        //print(gameObject.name + ": " + health);
        if (health <= 0)
        {
            Mana.Instance.GainMana(manaGain);
            Destroy(gameObject, delayDeath);
        }

        if (currentFlash != null)
        {
            StopCoroutine(currentFlash);
        }

        currentFlash = StartCoroutine(DamageFlash());
    }

    protected virtual IEnumerator DamageFlash()
    {
        float timer = 0;

        //while (timer < flashSpeed)
        //{
        //    timer += Time.deltaTime;
        //    mainRend.material.color = Color.Lerp(mainColor, Color.red, timer / flashSpeed);
        //    yield return null;
        //}

        mainRend.material.color = Color.red;

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

    public virtual void Shock(int damage, float jumpMod, int jumpCount, float jumpRadius, LayerMask lightningMask, LightningDrawer drawer, float jumpDelay = 0, Transform shocker = null)
    {
        delayDeath = jumpDelay + .1f;

        TakeDamage(damage, DamageType.lightning);

        jumpCount--;
        if (jumpCount > 0)
        {

            StartCoroutine(ShockJump((int)(damage * jumpMod + 0.5f), jumpMod, jumpCount, jumpRadius, lightningMask, drawer, jumpDelay, shocker));
        }
    }

    IEnumerator ShockJump(int damage, float jumpMod, int jumpCount, float jumpRadius, LayerMask lightningMask, LightningDrawer drawer, float jumpDelay, Transform shocker)
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
            minEn.Shock(damage, jumpMod, jumpCount, jumpRadius, lightningMask, drawer, jumpDelay, transform);

            drawer.Draw(transform.position, minEn.transform.position, jumpCount - 1, jumpDelay);

            //StartCoroutine(DrawLightning(minEn.transform.position, jumpDelay, lines[jumpCount - 1]));
        }
        else if (shockerClose && shocker)
        {
            BasicHealth newEn = shocker.GetComponent<BasicHealth>();
            if (newEn)
            {
                newEn.Shock(damage, jumpMod, jumpCount, jumpRadius, lightningMask, drawer, jumpDelay, transform);

                drawer.Draw(transform.position, minEn.transform.position, jumpCount - 1, jumpDelay);

                //StartCoroutine(DrawLightning(newEn.transform.position, jumpDelay, lines[jumpCount - 1]));
            }
        }
        else
        {
            // Could have enemy take the extra shock damage if there is nothing left to chain to?
        }
        delayDeath = 0;
    }
}
