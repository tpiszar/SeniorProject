using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyProtecter : MonoBehaviour
{
    public EnemyBarrier barrier;
    public BasicHealth ghoul;

    public float speedBoost = 0.2f;

    List<BasicHealth> enemies = new List<BasicHealth>();
    Dictionary<Transform, int> colliders = new Dictionary<Transform, int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
                enemy.SpeedBoost(speedBoost);
                enemy.SetInvincible(true, barrier);
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
                    enemy.SpeedDown(speedBoost);
                    enemy.SetInvincible(false, barrier);
                }
            }
        }
    }

    private void OnEnable()
    {
        if (ghoul)
        {
            ghoul.SetInvincible(true, barrier);
            ghoul.SpeedBoost(speedBoost);
        }
    }

    private void OnDisable()
    {
        if (ghoul)
        {
            ghoul.SetInvincible(false, barrier);
            //ghoul.RegularSpeed();
            ghoul.SpeedDown(speedBoost);
        }
        foreach (BasicHealth enemy in enemies)
        {
            if (enemy)
            {
                enemy.SetInvincible(false, barrier);
                //enemy.RegularSpeed();
                enemy.SpeedDown(speedBoost);
            }
        }
    }
}
