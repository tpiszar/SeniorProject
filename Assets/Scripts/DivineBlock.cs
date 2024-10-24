using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit;

public class DivineBlock : MonoBehaviour
{
    public int hitDamage;

    public ShockWave shockWave;

    public Collider blockCollider;
    public Collider barrierTrigger;
    public Barrier barrier;
    public GameObject obstacle;

    public Transform miniBlock;
    public Transform miniMap;
    public float shrinkScale = 0.03f;

    List<BasicHealth> hitEnemies = new List<BasicHealth>();
    Dictionary<Transform, int> hitColliders = new Dictionary<Transform, int>();

    Vector3 slamPos;

    void Slam()
    {
        foreach (BasicHealth hit in hitEnemies)
        {
            hit.TakeDamage(hitDamage, DamageType.energy);
        }

        blockCollider.isTrigger = false;

        shockWave.Slam();
        Destroy(shockWave.gameObject);
        Destroy(miniBlock.GetComponent<MiniDivineBlock>());
        miniBlock.GetComponent<XRGrabInteractable>().enabled = false;
        Destroy(GetComponent<Rigidbody>());

        transform.position = slamPos;

        barrierTrigger.enabled = true;
        barrier.enabled = true;
        obstacle.SetActive(true);

        miniBlock.parent = miniMap;
        print(miniBlock.position);
        miniBlock.localPosition = transform.position * shrinkScale;
        print(transform.position * shrinkScale + " " + miniBlock.position);

        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.transform.name);
        if (other.CompareTag("Path"))
        {
            slamPos = transform.position;
            if (other.transform.lossyScale.x < other.transform.lossyScale.z)
            {
                slamPos.x = other.transform.position.x;
            }
            else
            {
                slamPos.z = other.transform.position.z;
            }
            Slam();
        }
        else if (other.CompareTag("Enemy"))
        {
            BasicHealth enemy = other.GetComponentInParent<BasicHealth>();

            if (hitColliders.ContainsKey(enemy.transform))
            {
                hitColliders[enemy.transform]++;
            }
            else
            {
                hitColliders[enemy.transform] = 1;
                hitEnemies.Add(enemy);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            BasicHealth enemy = other.GetComponentInParent<BasicHealth>();
            if (hitColliders.ContainsKey(enemy.transform))
            {
                hitColliders[enemy.transform]--;

                // If no more colliders are inside the laser area, remove the enemy
                if (hitColliders[enemy.transform] == 0)
                {
                    hitColliders.Remove(enemy.transform);
                    hitEnemies.Remove(enemy);
                }
            }
        }
    }
}
