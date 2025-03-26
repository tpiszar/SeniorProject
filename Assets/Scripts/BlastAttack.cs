using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastAttack : MonoBehaviour
{
    public float force = 30;
    public float maxDistance = 10;

    public int damage;
    bool hit = false;
    public float delayDestroy = 0;

    public AudioClip hitSound;
    [Range(0.0001f, 1f)]
    public float hitVolume = 1;

    Vector3 start;

    public bool passThrough = false;

    public ParticleSystem staticParticle;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);

        start = transform.position;

        //Destroy(gameObject, 10);

        staticParticle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(start, transform.position) > maxDistance)
        {
            HitDestroy();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hit)
        {
            return;
        }

        hit = true;
        if (collision.transform.CompareTag("Enemy"))
        {
            BasicHealth enemy = collision.gameObject.GetComponentInParent<BasicHealth>();
            if (enemy)
            {
                enemy.TakeDamage(damage, DamageType.energy);
            }
        }
        print("Wand Blast Hit: " + collision.gameObject.name);

        HitDestroy();
    }

    bool destroyDone = false;
    List<Transform> hits = new List<Transform>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (hit && !passThrough)
        {
            return;
        }

        hit = true;

        if (other.CompareTag("Enemy"))
        {
            BasicHealth enemy = other.gameObject.GetComponentInParent<BasicHealth>();
            if (enemy)
            {
                if (enemy.GetHealth() <= 0)
                {
                    hit = false;
                    return;
                }

                if (passThrough && hits.Contains(enemy.transform))
                {
                    return;
                }

                enemy.TakeDamage(damage, DamageType.energy);
                hits.Add(enemy.transform);

                if (passThrough)
                {
                    SoundManager.instance.PlayClip(hitSound, transform.position, hitVolume);
                    return;
                }
            }
        }
        if (!destroyDone)
        {
            destroyDone = true;
            if (passThrough)
            {
                Invoke("HitDestroy", delayDestroy);
            }
            else
            {
                HitDestroy();
            }
            SoundManager.instance.PlayClip(hitSound, transform.position, hitVolume);
        }
    }

    void HitDestroy()
    {
        staticParticle.transform.parent = null;
        staticParticle.Stop();
        Destroy(staticParticle, 2);

        Destroy(gameObject);
    }
}
