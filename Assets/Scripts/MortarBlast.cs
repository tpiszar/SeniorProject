using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarBlast : MonoBehaviour
{
    public int impactDmg;

    public Transform target;
    public Vector3 targetPoint;
    Vector3 startPoint;
    float groundHeight = 1001;

    public LayerMask downCheck;

    public GameObject shot;
    public MortarPool pool;

    public float launchSpeed;
    public float gravity;

    float arrivalTime = 0;
    float arriving = 0;

    bool passedTarget = false;

    List<BasicHealth> enemies = new List<BasicHealth>();

    public ParticleSystem staticParticle;

    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            targetPoint = target.position;
        }
        else
        {
            if (groundHeight > 1000)
            {
                RaycastHit hit;
                if (Physics.Raycast(targetPoint, Vector3.down, out hit, 10f, downCheck))
                {
                    groundHeight = hit.point.y;
                }
                else
                {
                    groundHeight = targetPoint.y;
                }
            }
        }

        arriving += Time.deltaTime;
        arrivalTime = (launchSpeed + Mathf.Sqrt(launchSpeed * launchSpeed + 2 * gravity * (targetPoint.y - startPoint.y))) / gravity;

        Vector3 newPosition = targetPoint;

        if (arriving < arrivalTime)
        {
            newPosition = Vector3.Lerp(startPoint, targetPoint, arriving / arrivalTime);
        }

        newPosition.y = startPoint.y + launchSpeed * arriving - 0.5f * gravity * arriving * arriving;

        if (newPosition.y > targetPoint.y)
        {
            passedTarget = true;
        }

        if (newPosition.y < targetPoint.y && passedTarget)
        {
            if (groundHeight > 1000)
            {
                RaycastHit hit;
                if (Physics.Raycast(targetPoint, Vector3.down, out hit, 10f, downCheck))
                {
                    groundHeight = hit.point.y;
                }
                else
                {
                    groundHeight = targetPoint.y;
                }
            }

            if (target)
            {
                Explode();
                newPosition.y = groundHeight;
                transform.position = newPosition;
                Destroy(this);
            }
            else if (newPosition.y < groundHeight)
            {
                newPosition.y = groundHeight;
                transform.position = newPosition;
                Explode();
                Destroy(this);
            }
        }
        transform.position = newPosition;
    }

    void Explode()
    {
        foreach(BasicHealth enemy in enemies)
        {
            if (enemy)
            {
                enemy.TakeDamage(impactDmg, DamageType.energy);
            }
        }

        staticParticle.transform.parent = null;

        shot.SetActive(false);
        pool.enabled = true;

        staticParticle.Stop();
        Destroy(staticParticle, 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Root") || other.CompareTag("Shade Root"))
        {
            BasicHealth enemy = other.GetComponentInParent<BasicHealth>();
            if (enemy && !enemy.IsInvincible())
            {
                enemies.Add(enemy);
            }
        }
        else
        {
            EnemyBarrier barrier = other.GetComponentInParent<EnemyBarrier>();
            if (barrier)
            {
                enemies.Add(barrier);
            }
        }
    }
}
