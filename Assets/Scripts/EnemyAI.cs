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
        if (agent.enabled && agent.path.corners.Length > 2)
        {
            travellingDir = agent.path.corners[1];
            marker.position = agent.path.corners[1];
        }

        Debug.DrawRay(model.position, model.forward * attkRange, Color.red);

        if (close)
        {
            nextLocate -= Time.deltaTime;
            if (nextLocate < Time.time)
            {
                Locate();
                nextLocate = relocateInterval;
            }
        }

        if (attacking)
        {
            if (curAttackObj)
            {
                float distance = (transform.position - curAttackObj.position).magnitude;
                if (distance < attkRange * 2 && !chasingBarrier)
                {
                    chasingBarrier = true;
                    agent.SetDestination(curAttackObj.position);
                }
                if (distance < attkRange)
                {
                    if (!attackAnim)
                    {
                        animator.SetTrigger("Attack");
                        attackAnim = true;
                    }

                    nextAttk -= Time.deltaTime;
                    if (nextAttk < 0)
                    {
                        nextAttk = attkRate;
                        Attack();
                    }
                }
                else
                {

                }
            }
            else
            {
                DetermineBarrierAttack();
            }
        }
    }

    void Attack()
    {
        if (curAttackObj)
        {
            Barrier barrier = curAttackObj.GetComponent<Barrier>();
            if (barrier)
            {
                barrier.TakeDamage(damage);
                attackAnim = false;
            }
        }
    }

    public void isClose()
    {
        close = true;
        curAttackObj = player;
        attacking = true;
    }

    public void barrierClose(Transform obj)
    {
        attackObjs.Add(obj);
        Vector3 newDirection = (transform.position - obj.position);
        float newDotProduct = Vector3.Dot(travellingDir.normalized, newDirection.normalized);
        print(newDotProduct);
        if (!attacking || !curAttackObj)
        {
            if (newDotProduct > 0.8)
            {
                curAttackObj = obj;
                attacking = true;
            }
            return;
        }
        Vector3 direction = (transform.position - curAttackObj.position);
        //float dotProduct = Vector3.Dot(travellingDir.normalized, direction.normalized);
        if (newDotProduct > 0.8)
        {
            float distance = direction.magnitude;
            float newDistance = newDirection.magnitude;
            if (newDistance < distance)
            {
                curAttackObj = obj;
            }
        }
    }

    public void barrierFar(Transform obj)
    {
        attackObjs.Remove(obj);
    }

    void DetermineBarrierAttack()
    {
        for (int i = attackObjs.Count - 1; i >= 0; i--)
        {
            if (!attackObjs[i])
            {
                attackObjs.RemoveAt(i);
            }
        }

        int chosenBarrier = -1;
        float minDist = -1;
        for (int i = 0; i < attackObjs.Count; i++)
        {
            Vector3 newDirection = (transform.position - attackObjs[i].position);
            float newDotProduct = Vector3.Dot(travellingDir.normalized, newDirection.normalized);
            if (newDotProduct > 0.8)
            {
                float newDist = newDirection.magnitude;
                if (minDist > 0)
                {
                    if (newDist < minDist)
                    {
                        chosenBarrier = i;
                        minDist = newDist;
                    }
                }
                else
                {
                    chosenBarrier = i;
                    minDist = newDist;
                }
            }
        }

        if (chosenBarrier >= 0)
        {
            print(attackObjs[chosenBarrier].name);
            curAttackObj = attackObjs[chosenBarrier];
            attacking = true;
        }
        else
        {
            attacking = false;
            chasingBarrier = false;
            attackAnim = false;
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
        attacking = false;
        StartCoroutine(reStart(teleportTime));
    }

    IEnumerator reStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        agent.enabled = true;
        Locate();
        yield return new WaitForSeconds(0.2f);
        if (attackObjs.Count > 0)
        {
            DetermineBarrierAttack();
        }
    }

    private void OnDestroy()
    {
        Teleport.Instance.onTeleport -= onTeleport;
    }
}
