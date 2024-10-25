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

    public float burySize = 0.25f;

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

        float angle = Vector3.Angle(transform.up, Vector3.up);

        Vector3 modifiedVector;

        bool up = true;
        float y = transform.rotation.eulerAngles.y;

        if (angle < 45f)
        {
            modifiedVector = Vector3.Project(transform.up, Vector3.up);
        }
        else
        {
            modifiedVector = Vector3.ProjectOnPlane(transform.up, Vector3.up).normalized;

            up = false;
        }

        modifiedVector = Quaternion.FromToRotation(transform.up, modifiedVector) * transform.up;

        transform.up = modifiedVector;

        Vector3 rot = transform.rotation.eulerAngles;
        if (up)
        {
            rot.y = y;
            transform.rotation = Quaternion.Euler(rot);

            slamPos.y = path.transform.position.y + path.transform.lossyScale.y / 2 + transform.lossyScale.y / 2 - burySize;
        }
        else
        {
            rot.x = 0;
            transform.rotation = Quaternion.Euler(rot);

            slamPos.y = path.transform.position.y + path.transform.lossyScale.y / 2 + transform.lossyScale.x / 2 - burySize;
        }

        transform.position = slamPos;

        barrierTrigger.enabled = true;
        barrier.enabled = true;
        obstacle.SetActive(true);
        miniBlock.parent = miniMap;
        miniBlock.localPosition = transform.position * shrinkScale;
        miniBlock.localRotation = transform.rotation;

        Destroy(this);
    }

    Transform path;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Path"))
        {
            if (!path || path.transform.position.y > other.transform.position.y)
            {
                path = other.transform;
            }
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
