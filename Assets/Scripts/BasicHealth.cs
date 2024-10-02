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
    public float lightningDuration;

    // Start is called before the first frame update
    void Start()
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
                TakeDamage(tickBurn);
                burnRamp = 0;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        //print(gameObject.name + ": " + health);
        if (health <= 0)
        {
            Mana.Instance.GainMana(manaGain);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(DamageFlash());
        }
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

    public void Burn(float duration, float rate, int tick)
    {
        burnDuration = duration + 0.1f;
        burnRate = rate;
        burnRamp = 0;
        tickBurn = tick;
    }

    public void Shock(int damage, float jumpMod, int jumpCount, float jumpRadius, LayerMask lightningMask, float jumpDelay = 0)
    {
        TakeDamage(damage);

        jumpCount--;
        if (jumpCount > 0)
        {
            StartCoroutine(ShockJump((int)(damage * jumpMod), jumpMod, jumpCount, jumpRadius, lightningMask, jumpDelay));
        }
    }

    IEnumerator ShockJump(int damage, float jumpMod, int jumpCount, float jumpRadius, LayerMask lightningMask, float jumpDelay)
    {
        yield return new WaitForSeconds(jumpDelay);

        Collider[] hits = Physics.OverlapSphere(transform.position, jumpRadius, lightningMask);

        float minDist = float.MaxValue;
        BasicHealth minEn = null;
        for (int i = 0; i < hits.Length; i++)
        {
            BasicHealth newEn = hits[i].GetComponent<BasicHealth>();
            float newDist = (transform.position - hits[i].transform.position).sqrMagnitude;
            if (newDist < minDist)
            {
                minDist = newDist;
                minEn = newEn;
            }
        }

        if (minEn)
        {
            minEn.Shock(damage, jumpMod, jumpCount, jumpRadius, lightningMask, jumpDelay);

            StartCoroutine(DrawLightning(minEn.transform));
        }
    }

    IEnumerator DrawLightning(Transform nextEnemy)
    {
        lightningRender.SetPosition(0, transform.position);
        lightningRender.SetPosition(1, nextEnemy.position);

        lightningRender.enabled = true;

        yield return new WaitForSeconds(lightningDuration);

        lightningRender.enabled = false;
    }
}
