using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHealth : MonoBehaviour
{
    public float maxHealth;
    float health;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        print(gameObject.name + ": " + health);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
