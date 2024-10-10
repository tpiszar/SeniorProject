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
        BasicHealth enemy = collision.gameObject.GetComponent<BasicHealth>();
        if (enemy)
        {
            enemy.TakeDamage(damage, BasicHealth.DamageType.energy);
        }
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        BasicHealth enemy = other.gameObject.GetComponent<BasicHealth>();
        if (enemy)
        {
            enemy.TakeDamage(damage, BasicHealth.DamageType.energy);
        }
        else
        {
            Destroy(gameObject, delayDestroy);
        }
    }
}
