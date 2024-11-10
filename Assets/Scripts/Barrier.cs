using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public int maxHealth;
    public int startingHealth;
    int health;

    public bool annihilate = false;

    public bool startMini = false;
    GameObject mini;

    bool isPlayer;

    public TextMeshProUGUI healthText;
    Color startingColor;

    MeshRenderer mesh;
    Color wardColor;

    public AudioClip damageSound;
    [Range(0.0001f, 1f)]
    public float damageVolume = 1;
    public AudioClip destroySound;
    [Range(0.0001f, 1f)]
    public float destroyVolume = 1;

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

        if (!isPlayer && !healthText)
        {
            health = startingHealth;
            mesh = GetComponent<MeshRenderer>();
            wardColor = mesh.material.color;
            wardColor.a = Mathf.Lerp(0.2f, 0.8f, (float)health / maxHealth);
            mesh.material.color = wardColor;
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

        if (mesh)
        {
            wardColor.a = Mathf.Lerp(0.2f, 0.8f, (float)health / maxHealth);
            mesh.material.color = wardColor;
        }

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
                SoundManager.instance.PlayClip(destroySound, transform.position, destroyVolume);
                Destroy(gameObject);
            }
        }
        else
        {
            SoundManager.instance.PlayClip(damageSound, transform.position, damageVolume);
        }
    }

    public bool CanMaximizeCharge(int gain)
    {
        return (health + gain) <= maxHealth;
    }

    public void GainHealth(int gain)
    {
        health += gain;
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        if (mesh)
        {
            wardColor.a = Mathf.Lerp(0.2f, 0.8f, (float)health / maxHealth);
            mesh.material.color = wardColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Root") || other.CompareTag("Shade Root"))
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
        if (other.CompareTag("Root") || other.CompareTag("Shade Root"))
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
        if (mini)
        {
            Destroy(mini);
        }
    }
}
