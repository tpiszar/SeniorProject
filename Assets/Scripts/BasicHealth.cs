using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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

    int invincible = 0;
    List<EnemyBarrier> enemyBarriers = new List<EnemyBarrier>();

    protected NavMeshAgent agent;
    protected float baseSpeed;

    //public LineRenderer lightningRender;

    float delayDeath = 0;

    public AudioClip damageSound;
    [Range(0.0001f, 1f)]
    public float damageVolume = 1;

    public float soundInterval = 2;
    float nextInterval = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = maxHealth;
        mainColor = mainRend.material.color;
        //flashSpeed /= 2;
        agent = GetComponent<NavMeshAgent>();
        if (agent)
        {
            baseSpeed = agent.speed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        overTimeDamageThisFrame = false;
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

    public void CleanUpBarriers()
    {
        for (int i = enemyBarriers.Count - 1; i >= 0; i--)
        {
            if (!enemyBarriers[i])
            {
                enemyBarriers.RemoveAt(i);
            }
        }
    }

    public void SetInvincible(bool increase, EnemyBarrier barrier)
    {
        if (increase)
        {
            invincible++;
            if (!enemyBarriers.Contains(barrier))
            {
                enemyBarriers.Add(barrier);
            }
        }
        else
        {
            invincible--;
            enemyBarriers.Remove(barrier);
        }

        burnDuration = 0;
        burnRate = 0;
        burnRamp = 0;
        tickBurn = 0;
    }

    public bool IsInvincible()
    {
        return invincible > 0;
    }

    public virtual void SpeedBoost(float boost)
    {
        if (!agent) { return; }
        agent.speed *= (1 + boost);
    }
    public virtual void SpeedDown(float boost)
    {
        if (!agent) { return; }
        agent.speed /= (1 + boost);
    }
    public virtual void RegularSpeed()
    {
        if (!agent) { return; }
        agent.speed = baseSpeed;
    }


    bool overTimeDamageThisFrame = true;
    public virtual EnemyBarrier TakeDamage(int damage, DamageType type)
    {
        if (health <= 0)
        {
            return null;
        }

        if (type == DamageType.overTime)
        {
            if (overTimeDamageThisFrame)
            {
                return null;
            }
            overTimeDamageThisFrame = true;

            type = DamageType.fire;
        }

        if (invincible > 0)
        {
            //CleanUpBarriers();
            
            if (enemyBarriers[invincible - 1] && type != DamageType.fire)
            {
                return enemyBarriers[invincible - 1].TakeDamage(damage, type);
            }
            return null;
        }

        WaveManager.totalDamage += damage;
        //print(WaveManager.totalDamage);

        health -= damage;
        //print(gameObject.name + ": " + health);
        if (health <= 0)
        {
            Mana.Instance.GainMana(manaGain);

            SoundManager.instance.PlayDeathClip(type, transform.position);

            WaveManager.kills++;

            Destroy(gameObject, delayDeath);
        }
        else
        {
            if (Time.time > nextInterval)
            {
                nextInterval = Time.time + soundInterval;
                SoundManager.instance.PlayClip(damageSound, transform.position, damageVolume);
            }
        }

        if (currentFlash != null)
        {
            StopCoroutine(currentFlash);
        }

        currentFlash = StartCoroutine(DamageFlash());

        return null;
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
        if (invincible > 0)
        {
            return;
        }

        burnDuration = duration + 0.1f;
        burnRate = rate;
        burnRamp = 0;
        tickBurn = tick;
    }

    public virtual EnemyBarrier Shock(int damage, float jumpMod, int jumpCount, float jumpRadius, LayerMask lightningMask, LightningDrawer drawer, float jumpDelay = 0, Transform shocker = null)
    {
        
        delayDeath = jumpDelay + .1f;

        jumpCount--;
        if (jumpCount > 0 && invincible == 0)
        {

            StartCoroutine(ShockJump((int)(damage * jumpMod + 0.5f), jumpMod, jumpCount, jumpRadius, lightningMask, drawer, jumpDelay, shocker));
        }

        return TakeDamage(damage, DamageType.lightning);
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

                drawer.Draw(transform.position, newEn.transform.position, jumpCount - 1, jumpDelay);

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
