using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHealth : MonoBehaviour
{
    public int maxHealth;
    protected int health;

    public int manaGain;

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
            Mana.Instance.GainMana(manaGain);
            Destroy(gameObject);
        }
    }
}
