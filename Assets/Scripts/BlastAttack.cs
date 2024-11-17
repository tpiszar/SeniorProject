using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastAttack : MonoBehaviour
{
    public int damage;
    bool hit = false;
    public float delayDestroy = 0;

    public AudioClip hitSound;
    [Range(0.0001f, 1f)]
    public float hitVolume = 1;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 10);
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        
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

        Destroy(gameObject);
    }

    bool destroyDone = false;
    List<Transform> hits = new List<Transform>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {


            BasicHealth enemy = other.gameObject.GetComponentInParent<BasicHealth>();
            if (enemy)
            {
                if (hits.Contains(enemy.transform))
                {
                    return;
                }

                enemy.TakeDamage(damage, DamageType.energy);
                hits.Add(enemy.transform);
            }
            else
            {
                if (!destroyDone)
                {
                    Destroy(gameObject, delayDestroy);
                }
            }
        }
        else
        {
            if (!destroyDone)
            {
                Destroy(gameObject, delayDestroy);
            }
        }
    }

    private void OnDestroy()
    {
        print("BlastDestroyed " + Time.time);
        SoundManager.instance.PlayClip(hitSound, transform.position, hitVolume);
    }
}
