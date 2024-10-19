using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFlame : MonoBehaviour
{
    public int damage;
    public float burnDuration;
    public float burnRate;
    public int tickDamage;

    public List<Transform> hitThisCycle = new List<Transform>();

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

            BasicHealth enemy = other.gameObject.GetComponentInParent<BasicHealth>();
            if (enemy)
            {
                if (hitThisCycle.Contains(enemy.transform))
                {
                    return;
                }

                enemy.TakeDamage(damage, DamageType.fire);
                enemy.Burn(burnDuration, burnRate, tickDamage);

                hitThisCycle.Add(enemy.transform);
            }
        }

    }
}
