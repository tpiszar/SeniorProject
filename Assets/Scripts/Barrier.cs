using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public int maxHealth;
    int health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        //print(gameObject.name + ": " + health);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy)
        {
            enemy.barrierClose(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy)
        {
            enemy.barrierFar(transform);
        }
    }
}
