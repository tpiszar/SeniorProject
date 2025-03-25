using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarPool : MonoBehaviour
{
    public static List<MortarPool> pools = new List<MortarPool>();
    bool added = false;
    public float overrideDistance = 0.5f;

    public ParticleSystem particle;

    public float duration;
    float timer;

    public float dps;
    float damageRamp = 0f;

    List<BasicHealth> enemies = new List<BasicHealth>();
    Dictionary<Transform, int> colliders = new Dictionary<Transform, int>();

    public AudioSource hitSound;
    public AudioSource bubbleSound;

    // Start is called before the first frame update
    void Start()
    {
        foreach (MortarPool pool in pools)
        {
            if (Vector3.Distance(transform.position, pool.transform.position) < overrideDistance)
            {
                pool.Stop();
            }
        }
        pools.Add(this);
        added = true;

        hitSound.Play();
        bubbleSound.Play();

        particle.Play();
        particle.transform.parent = null;
        timer = duration;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Stop();
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
                enemies[i].TakeDamage(dmg, DamageType.overTime);
            }
        }
    }

    public void Stop()
    {
        particle.Stop();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (added)
        {
            pools.Remove(this);
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
