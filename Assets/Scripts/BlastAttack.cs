using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastAttack : MonoBehaviour
{
    public int damage;
    bool hit = false;

    public bool passThrough = false;
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
        if (!passThrough)
        {
            Destroy(gameObject);
        }
        else if (passThrough && !enemy)
        {
            Destroy(gameObject, delayDestroy);
        }
    }
}
