using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public int maxHealth;
    int health;

    public bool startMini = false;
    GameObject mini;

    bool isPlayer;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        if (startMini)
        {
            mini = MiniMapTracker.instance.AddMapBarrier(transform);
        }
        
        if (health == 0)
        {
            isPlayer = true;
        }
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
            if (isPlayer)
            {
                GetComponent<PlayerDeath>().Die();
            }
            else
            {
                Destroy(gameObject);
            }
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

    private void OnDestroy()
    {
        Destroy(mini);
    }
}
