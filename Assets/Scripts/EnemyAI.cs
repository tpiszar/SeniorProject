using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IComparable
{
    public Enemytype type;

    public Transform player;
    protected NavMeshAgent agent;
    public float attkRange;
    public float attkRate;
    protected float nextAttk;
    public int damage;

    public float rayInterval;
    protected float nextRay = 0;
    public LayerMask hitMask;

    [Range(0f, 1f)]
    public float barrierBlendAmount = 0.5f;

    public float relocateInterval;
    protected float nextLocate = 0;

    public float distCheckInterval = 0.5f;
    protected float nextDist;

    bool close = false;
    List<Transform> attackObjs = new List<Transform>();
    protected Transform curAttackObj;

    public Transform model;
    bool attacking = false;
    bool chasingBarrier = false;

    public Transform marker;

    public Vector3 travellingDir;

    public Animator animator;
    public float attackAnimDuration = 1;

    public float rotationSpeed;

    protected float distance;

    public float headHeight;

    public AudioSource passiveSound;
    public float passiveMax = 12;
    public float passiveMin = 5;
    float playPassive;
    public AudioSource attackSound;

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
    protected virtual void Start()
    {
        if (Teleport.Instance)
        {
            Teleport.Instance.onTeleport += OnTeleport;
        }
        agent = GetComponent<NavMeshAgent>();
        nextAttk = 0;
        nextDist = 0;
        Locate();

        // Will need to be changed in the future
        //animator.speed = 1 / attkRate;

        if (animator)
        {
            animator.SetFloat("AttackRate", 1 / (attkRate / attackAnimDuration));
        }

        playPassive = UnityEngine.Random.Range(passiveMin / 2, passiveMin);
    }

    private void OnEnable()
    {
        if (player) { Locate(); }
    }

    public Vector3 destination;
    // Update is called once per frame
    protected virtual void Update()
    {
        playPassive -= Time.deltaTime;
        if (playPassive < 0)
        {
            playPassive = UnityEngine.Random.Range(passiveMin, passiveMax);
            passiveSound.Play();
        }

        destination = agent.destination;
        if (agent.enabled && agent.path.corners.Length >= 2 && !close)
        {
            travellingDir = agent.path.corners[1];
            //marker.position = agent.path.corners[1];
        }
        else
        {
            travellingDir = transform.forward + transform.position;
            //marker.position = transform.forward + transform.position;
        }

        travellingDir.y = transform.position.y;
        Vector3 MovingDir = (travellingDir - transform.position).normalized;

        Debug.DrawRay(transform.position, MovingDir * attkRange, Color.red);
        Debug.DrawRay(transform.position, (transform.forward + MovingDir).normalized * attkRange, Color.green);
        Debug.DrawRay(transform.position, transform.forward * attkRange, Color.blue);

        if (close)
        {
            if (!curAttackObj || curAttackObj == player)
            {
                nextLocate -= Time.deltaTime;
                if (nextLocate < Time.time)
                {
                    Locate();
                    nextLocate = relocateInterval;
                }
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
                    if (Physics.Raycast(transform.position,
                        //MovingDir,
                        //transform.forward,
                        (transform.forward + MovingDir),
                        out hit, attkRange, hitMask))
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
                if (Physics.Raycast(transform.position, direction, out hit, attkRange, hitMask))
                {
                    curAttackObj = hit.transform;
                    GetDistance();

                    if (hit.transform.CompareTag("Player"))
                    {
                        agent.SetDestination(hit.point);
                    }
                    else
                    {
                        Vector3 blend = (hit.point - hit.transform.position) * barrierBlendAmount + hit.transform.position;

                        agent.SetDestination(blend);// hit.transform.position);
                    }

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
            attackSound.Play();

            animator.SetTrigger("Attack");
        }
    }

    public virtual void Hit()
    {
        if (!this || !curAttackObj)
        {
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, attkRange, hitMask))
        {

            Barrier barrier = hit.transform.GetComponent<Barrier>();//curAttackObj.GetComponent<Barrier>();
            if (barrier)
            {
                barrier.TakeDamage(damage, gameObject);

                if (!barrier)
                {
                    agent.SetDestination(player.position);
                    chasingBarrier = false;
                }
            }
        }
    }

    public virtual void AttackDone()
    {
        return;
    }

    public virtual float GetDistance()
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
        //curAttackObj = player;
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

    public void Locate()
    {
        if (!player || !agent)
        {
            return;
        }

        agent.SetDestination(player.position);
        chasingBarrier = false;
    }

    protected virtual void OnTeleport(float teleportTime)
    {
        if (type == Enemytype.Slime)
        {
            animator.SetTrigger("ForceIdle");
        }

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

        if (type == Enemytype.Slime)
        {
            animator.SetTrigger("ForceIdle");
        }

        yield return new WaitForSeconds(0.2f);
        CleanUpBarriers();
        if (attackObjs.Count > 0)
        {
            attacking = true;
        }
    }

    private void OnDestroy()
    {
        if (Teleport.Instance)
        {
            Teleport.Instance.onTeleport -= OnTeleport;
        }
    }
}
