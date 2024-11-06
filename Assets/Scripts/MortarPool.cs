using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarPool : MonoBehaviour
{
    public ParticleSystem particle;

    public float duration;
    float timer;

    public float dps;
    float damageRamp = 0f;

    List<BasicHealth> enemies = new List<BasicHealth>();
    Dictionary<Transform, int> colliders = new Dictionary<Transform, int>();

    // Start is called before the first frame update
    void Start()
    {
        particle.Play();
        timer = duration;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            particle.Stop();
            Destroy(gameObject);
        }

        if (enemies.Count == 0)
        {
            damageRamp = 0f;
            return;
        }

        damageRamp += Time.deltaTime * dps;
        int dmg = (int)damageRamp;
        damageRamp -= dmg;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (!enemies[i])
            {
                enemies.RemoveAt(i);
            }
            else
            {
                enemies[i].TakeDamage(dmg, DamageType.fire);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            BasicHealth enemy = other.GetComponentInParent<BasicHealth>();

            if (colliders.ContainsKey(enemy.transform))
            {
                colliders[enemy.transform]++;
            }
            else
            {
                colliders[enemy.transform] = 1;
                enemies.Add(enemy);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            BasicHealth enemy = other.GetComponentInParent<BasicHealth>();
            if (colliders.ContainsKey(enemy.transform))
            {
                colliders[enemy.transform]--;

                // If no more colliders are inside the laser area, remove the enemy
                if (colliders[enemy.transform] == 0)
                {
                    colliders.Remove(enemy.transform);
                    enemies.Remove(enemy);
                }
            }
        }
    }
}
