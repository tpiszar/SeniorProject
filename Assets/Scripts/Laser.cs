using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float damagePerSec = 20f;

    float damageRamp = 0f;

    public float length = 100;
    public LineRenderer laserLine;

    List<BasicHealth> enemies = new List<BasicHealth>();
    Dictionary<Transform, int> colliders = new Dictionary<Transform, int>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (enemies.Count == 0)
        {
            damageRamp = 0f;
            return;
        }

        damageRamp += Time.deltaTime * damagePerSec;
        int dmg = (int)damageRamp;
        damageRamp -= dmg;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (!enemies[i])
            {
                enemies.RemoveAt(i);
            }
            else
            {
                enemies[i].TakeDamage(dmg, DamageType.energy);
            }
        }
    }

    public void SetLaser(Vector3 ground, Vector3 hand)
    {
        transform.position = ground;
        Vector3 direction = (hand - ground).normalized;
        direction.y = Mathf.Clamp(direction.y, 0.35f, 1);
        transform.up = direction;

        laserLine.SetPosition(1, ground);
        laserLine.SetPosition(0, ground + direction.normalized * 100);
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
                }
            }
        }
    }
}
