using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyBarrier : BasicHealth
{
    public GhoulAI ghoul;

    public GameObject shatterBarrier;

    Material barrierMat;
    Color baseColor;
    Color curColor;
    Color maxColor;
    float startIntensity = 1f;
    public float endIntensity = .5f;
    public float maxIntensity = 1.5f;

    //public float antiFlashRate = 1f;
    //float nextFlash = 0;

    protected override void Start()
    {
        maxHealth = (int)(WaveManager.buff * maxHealth);

        health = maxHealth;

        barrierMat = mainRend.material;

        baseColor = barrierMat.GetColor("_Color");

        curColor = baseColor;

        maxColor = curColor * (maxIntensity / startIntensity);
    }

    void Update()
    {
        //nextFlash -= Time.deltaTime;
    }

    public override EnemyBarrier TakeDamage(int damage, DamageType type)
    {
        WaveManager.totalDamage += damage;
        //print(WaveManager.totalDamage);

        health -= damage;
        //print(gameObject.name + ": " + health);
        if (health <= 0)
        {
            ghoul.barrierUp = false;
            gameObject.SetActive(false);

            barrierMat.SetColor("_Color", baseColor);

            curColor = baseColor;

            return this;
        }
        else
        {
            if (Time.time > nextInterval)
            {
                nextInterval = Time.time + soundInterval;
                SoundManager.instance.PlayClip(damageSound, transform.position, damageVolume);
            }

            //SoundManager.instance.PlayClip(damageSound, transform.position, damageVolume);
        }

        float newIntensity = Mathf.Lerp(endIntensity, startIntensity, (float)health / maxHealth);

        curColor = baseColor * (newIntensity / startIntensity);

        maxColor = curColor * (maxIntensity / startIntensity);

        //if (nextFlash > 0)
        //{
        //    barrierMat.SetColor("_Color", curColor);

        //}
        //else
        //{
        //    if (currentFlash != null)
        //    {
        //        StopCoroutine(currentFlash);
        //    }

        //    currentFlash = StartCoroutine(DamageFlash());
        //}

        barrierMat.SetColor("_Color", curColor);

        if (currentFlash != null)
        {
            StopCoroutine(currentFlash);
        }

        currentFlash = StartCoroutine(DamageFlash());

        return this;
    }

    protected override IEnumerator DamageFlash()
    {
        float timer = 0;

        while (timer < flashSpeed)
        {
            timer += Time.deltaTime;

            barrierMat.SetColor("_Color", Color.Lerp(curColor, maxColor, timer / flashSpeed));

            yield return null;
        }

        barrierMat.SetColor("_Color", maxColor);

        timer = 0;
        while (timer < flashSpeed)
        {
            timer += Time.deltaTime;

            barrierMat.SetColor("_Color", Color.Lerp(maxColor, curColor, timer / flashSpeed));

            yield return null;
        }

        barrierMat.SetColor("_Color", curColor);
    }

    public override void Burn(float duration, float rate, int tick)
    {
        return;
    }

    public override EnemyBarrier Shock(int damage, float jumpMod, int jumpCount, float jumpRadius, LayerMask lightningMask, LightningDrawer drawer, float jumpDelay = 0, Transform shocker = null)
    {
        return TakeDamage(damage, DamageType.lightning);
    }

    public void Restore()
    {
        health = maxHealth;
        ghoul.barrierUp = true;
        gameObject.SetActive(true);
    }

    protected override void OnDestroy()
    {
        return;
    }

    private void OnDisable()
    {

        if (UIScript.sceneLoading) return;


        if (shatterBarrier)
        {
            Instantiate(shatterBarrier, transform.position, transform.rotation);

            SoundManager.instance.PlayClip(deathSound, transform.position, deathVolume);
        }
    }
}
