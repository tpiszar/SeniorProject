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
}
