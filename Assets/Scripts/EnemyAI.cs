using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IComparable
{
    public Transform player;
    protected NavMeshAgent agent;
    public float attkRange;
    public float attkRate;
    float nextAttk;
    public int damage;

    public float rayInterval;
    float nextRay = 0;
    public LayerMask hitMask;

    public float relocateInterval;
    float nextLocate = 0;

    public float distCheckInterval = 0.5f;
    float nextDist;

    bool close = false;
    List<Transform> attackObjs = new List<Transform>();
    Transform curAttackObj;

    public Transform model;
    bool attacking = false;
    bool chasingBarrier = false;

    public Transform marker;

    Vector3 travellingDir;

    public Animator animator;
    bool attackAnim = false;

    public float rotationSpeed;

    float distance;

    public int CompareTo(object obj)
    {
        GetDistance();
        EnemyAI other = obj as EnemyAI;
        if (GetDistance() < other.GetDistance())
        {
            return -1;
        }
        if (distance > other.distance)
        {
            return 1;
        }

        return 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        Teleport.Instance.onTeleport += onTeleport;
        agent = GetComponent<NavMeshAgent>();
        nextAttk = attkRate;
        nextDist = distCheckInterval;
        Locate();

        // Will need to be changed in the future
        animator.speed = 1 / attkRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.enabled && agent.path.corners.Length >= 2 && !close)
        {
            travellingDir = agent.path.corners[1];
            marker.position = agent.path.corners[1];
        }
        else
        {
            travellingDir = transform.forward + transform.position;
            marker.position = transform.forward + transform.position;
        }

        travellingDir.y = transform.position.y;
        Vector3 direction1 = (travellingDir - transform.position).normalized;

        Debug.DrawRay(transform.position, direction1 * attkRange * 2, Color.red);
        Debug.DrawRay(transform.position, transform.forward * attkRange, Color.blue);

        if (close)
        {
            nextLocate -= Time.deltaTime;
            if (nextLocate < Time.time)
            {
                Locate();
                nextLocate = relocateInterval;
            }

            Vector3 direction = player.position - transform.position;
            direction.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (attacking)
        {
            nextRay -= Time.deltaTime;
            if (curAttackObj)
            {
                nextAttk -= Time.deltaTime;

                if (nextAttk < 0 && nextRay < 0)
                {
                    nextRay = rayInterval;

                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.forward, out hit, attkRange, hitMask))
                    {
                        nextAttk = attkRate;
                        Attack();
                    }
                    else
                    {
                        CleanUpBarriers();
                    }
                }
            }
            else if (nextRay < 0)
            {
                if (chasingBarrier)
                {
                    chasingBarrier = false;
                    agent.SetDestination(player.position);
                }
                //if (agent.enabled && agent.path.corners.Length >= 2)
                //{
                //    travellingDir = agent.path.corners[1];
                //    marker.position = agent.path.corners[1];
                //}
                //else
                //{
                //    travellingDir = transform.forward + transform.position;
                //    marker.position = transform.forward + transform.position;
                //}

                travellingDir.y = transform.position.y;
                Vector3 direction = (travellingDir - transform.position).normalized;

                nextRay = rayInterval;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction, out hit, attkRange * 2, hitMask))
                {
                    curAttackObj = hit.transform;
                    GetDistance();
                    agent.SetDestination(hit.transform.position);
                    chasingBarrier = true;
                }
                else
                {
                    CleanUpBarriers();
                    curAttackObj = null;
                }
            }
        }
    }

    void Attack()
    {
        Barrier barrier = curAttackObj.GetComponent<Barrier>();
        if (barrier)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void Hit()
    {
        if (!this || !curAttackObj)
        {
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, attkRange, hitMask))
        {
            Barrier barrier = curAttackObj.GetComponent<Barrier>();
            if (barrier)
            {
                barrier.TakeDamage(damage);

                if (!barrier)
                {
                    agent.SetDestination(player.position);
                    chasingBarrier = false;
                }
            }
        }
    }

    public float GetDistance()
    {
        if (Time.time > nextDist && !chasingBarrier)
        {
            if (agent && !agent.pathPending && agent.pathStatus != NavMeshPathStatus.PathInvalid)
            {
                if (agent.path.corners.Length == 0)
                {
                    return (player.position - transform.position).magnitude;
                }

                nextDist = Time.time + distCheckInterval;
                //distance = (player.position - transform.position).sqrMagnitude;

                distance = 0;
                for (int i = 0; i < agent.path.corners.Length - 1; i++)
                {
                    distance += Vector3.Distance(agent.path.corners[i], agent.path.corners[i + 1]);
                }
            }

        }
        return distance;
    }

    public void isClose()
    {
        close = true;
        attackObjs.Add(player);
        curAttackObj = player;
        attacking = true;
        agent.updateRotation = false;
    }

    public void isFar()
    {
        close = false;
        CleanUpBarriers();
        agent.updateRotation = true;
        attackObjs.Remove(player);
    }

    public void barrierClose(Transform obj)
    {
        attackObjs.Add(obj);
        attacking = true;
    }

    public void barrierFar(Transform obj)
    {
        attackObjs.Remove(obj);
    }

    void CleanUpBarriers()
    {
        for (int i = attackObjs.Count - 1; i >= 0; i--)
        {
            if (!attackObjs[i])
            {
                if (attackObjs[i] == curAttackObj)
                {
                    curAttackObj = null;
                }
                attackObjs.RemoveAt(i);
            }
        }

        if (attackObjs.Count == 0)
        {
            attacking = false;
            curAttackObj = null;
            agent.SetDestination(player.position);
            chasingBarrier = false;
        }
    }

    void Locate()
    {
        agent.SetDestination(player.position);
        chasingBarrier = false;
    }

    private void onTeleport(float teleportTime)
    {
        agent.enabled = false;
        chasingBarrier = false;
        close = false;
        curAttackObj = null;
        attacking = false;
        StartCoroutine(reStart(teleportTime));
    }

    IEnumerator reStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        agent.enabled = true;
        Locate();
        yield return new WaitForSeconds(0.2f);
        CleanUpBarriers();
        if (attackObjs.Count > 0)
        {
            attacking = true;
        }
    }

    private void OnDestroy()
    {
        Teleport.Instance.onTeleport -= onTeleport;
    }
}
