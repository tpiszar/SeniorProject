using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public int maxHealth;
    int health;

    public bool annihilate = false;

    public bool startMini = false;
    GameObject mini;

    bool isPlayer;

    public TextMeshProUGUI healthText;
    Color startingColor;

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

        if (healthText)
        {
            startingColor = healthText.color;
            healthText.text = health.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        health -= damage;
        //print(gameObject.name + ": " + health);

        if (annihilate)
        {
            Destroy(attacker);
        }

        if (healthText)
        {
            healthText.text = health.ToString();
            float t = (float)health / maxHealth;
            Color lerpColor = Color.Lerp(Color.red, startingColor, t);
            healthText.color = lerpColor;
        }

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
        if (other.CompareTag("Root"))
        {
            EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
            if (enemy)
            {
                enemy.barrierClose(transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Root"))
        {
            EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
            if (enemy)
            {
                enemy.barrierFar(transform);
            }
        }
    }

    private void OnDestroy()
    {
        Destroy(mini);
    }
}
