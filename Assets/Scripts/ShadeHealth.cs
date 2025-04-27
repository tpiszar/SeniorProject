using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShadeHealth : BasicHealth
{
    Color hurtColor = Color.red;

    public GameObject shadeDestroyParticle;

    // Start is called before the first frame update
    protected override void Start()
    {
        maxHealth = (int)(WaveManager.buff * maxHealth);

        matColors = new Color[1];

        matColors[0] = mainRend.materials[1].color;

        health = maxHealth;
        //mainColor = mainRend.material.color;
        //flashSpeed /= 2;
        agent = GetComponent<NavMeshAgent>();
        if (agent)
        {
            baseSpeed = agent.speed;
        }
    }

    protected override IEnumerator DamageFlash()
    {
        float timer = 0;

        hurtColor = Color.red;
        hurtColor.a = mainRend.materials[1].color.a;
        mainRend.materials[1].color = hurtColor;

        timer = 0;
        while (timer < flashSpeed)
        {
            timer += Time.deltaTime;

            hurtColor = Color.Lerp(Color.red, matColors[0], timer / flashSpeed);
            hurtColor.a = mainRend.materials[1].color.a;
            mainRend.materials[1].color = hurtColor;

            yield return null;
        }

        hurtColor = matColors[0];
        hurtColor.a = mainRend.materials[1].color.a;
        mainRend.materials[1].color = hurtColor;
    }

    protected override void OnDestroy()
    {
        if (UIScript.sceneLoading || !shadeDestroyParticle) { return; }

        if (health > 0) //ANIHALATE
        {
            annihilateParticle.transform.parent = null;
            annihilateParticle.Play();

            SoundManager.instance.Annihilate(transform.position);
        }
        else //KILL
        {
            Instantiate(shadeDestroyParticle, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}
