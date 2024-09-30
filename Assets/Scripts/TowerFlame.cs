using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFlame : MonoBehaviour
{
    public int damage;
    public float burnDuration;
    public float burnRate;

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
        BasicHealth enemy = other.gameObject.GetComponent<BasicHealth>();
        if (enemy)
        {
            enemy.TakeDamage(damage);
            enemy.Burn(burnDuration, burnRate);
        }
    }
}
