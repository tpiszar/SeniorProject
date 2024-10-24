using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{

    public int shockWaveDamage;

    public Transform block;

    List<BasicHealth> shockEnemies = new List<BasicHealth>();
    Dictionary<Transform, int> shockColliders = new Dictionary<Transform, int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = block.position;
    }

    public void Slam()
    {
        foreach (BasicHealth hit in shockEnemies)
        {
            hit.TakeDamage(shockWaveDamage, DamageType.energy);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            BasicHealth enemy = other.GetComponentInParent<BasicHealth>();

            if (shockColliders.ContainsKey(enemy.transform))
            {
                shockColliders[enemy.transform]++;
            }
            else
            {
                shockColliders[enemy.transform] = 1;
                shockEnemies.Add(enemy);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            BasicHealth enemy = other.GetComponentInParent<BasicHealth>();
            if (shockColliders.ContainsKey(enemy.transform))
            {
                shockColliders[enemy.transform]--;

                // If no more colliders are inside the laser area, remove the enemy
                if (shockColliders[enemy.transform] == 0)
                {
                    shockColliders.Remove(enemy.transform);
                    shockEnemies.Remove(enemy);
                }
            }
        }
    }
}
