using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastAttack : MonoBehaviour
{
    public int damage;
    bool hit = false;
    public float delayDestroy = 0;

    // Start is called before the first frame update
    void Start()
    {
        
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
                enemy.TakeDamage(damage, BasicHealth.DamageType.energy);
            }
        }

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

                enemy.TakeDamage(damage, BasicHealth.DamageType.energy);
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
}
