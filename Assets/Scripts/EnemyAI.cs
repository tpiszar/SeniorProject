using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        Teleport.Instance.onTeleport += onTeleport;
        agent = GetComponent<NavMeshAgent>();
        nextAttk = attkRate;
        Locate();
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
                    print(Vector3.Distance(transform.position, hit.point));
                    curAttackObj = hit.transform;
                    agent.SetDestination(hit.transform.position);
                    print("HIT " + hit.transform.name);
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
                }
            }
        }
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
        }
    }

    void Locate()
    {
        agent.SetDestination(player.position);
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
