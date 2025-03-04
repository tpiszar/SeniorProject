using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsychicTower : MonoBehaviour
{
    public GlobalTowerDetection detector;

    public int damage;

    public Transform chargeBall;
    float chargeSize = 0;

    public float enterDelay = 0.5f;

    public Transform attack;
    public Animator attackAnim;
    float attkHeight;

    public float fireRate;
    float nextAttk = 0;
    public float chargeRate;
    float chargeDone = 0;
    public float collapseRate;
    float collapseDone = 0;

    public Transform currentTarget;

    bool wasNull = false;

    public AudioSource chargeSound;
    bool playCollapse = true;
    public AudioSource collapesSound;
    public AudioClip hitSound;
    [Range(0.0001f, 1f)]
    public float hitVolume = 1;

    // Start is called before the first frame update
    void Start()
    {
        chargeSize = chargeBall.localScale.x;
        chargeBall.localScale = Vector3.zero;
        attack.gameObject.SetActive(false);

        attackAnim.speed = 1 / collapseRate;
    }

    // Update is called once per frame
    void Update()
    {
        nextAttk -= Time.deltaTime;
        if (nextAttk < 0)
        {
            currentTarget = detector.GetTarget();
            if (currentTarget)
            {
                if (wasNull)
                {
                    // Buffer to ensure strongest enemy has fully spawned in bunch
                    // Better acounts for time for enemy to reach the door
                    wasNull = false;
                    nextAttk = enterDelay;
                    return;
                }
                chargeDone = chargeRate;
                nextAttk = fireRate + chargeRate + collapseRate;

                attkHeight = currentTarget.GetComponent<EnemyAI>().headHeight;

                attack.gameObject.SetActive(true);

                chargeSound.Play();
            }
            else
            {
                wasNull = true;
            }
        }
        else if (chargeDone > 0)
        {
            chargeDone -= Time.deltaTime;
            if (!currentTarget)
            {
                currentTarget = detector.GetTarget();
                if (!currentTarget)
                {
                    chargeBall.localScale = Vector3.zero;

                    attack.position = Vector3.down * 1000;
                    attack.gameObject.SetActive(false);

                    nextAttk = 0;
                    return;
                }
                attkHeight = currentTarget.GetComponent<EnemyAI>().headHeight;
            }

            Vector3 attkPos = currentTarget.position;
            attkPos.y += attkHeight;
            attack.position = attkPos;

            float size = Mathf.Lerp(chargeSize, 0, chargeDone / chargeRate);
            //sprint(size + " " + chargeDone + " / " + chargeRate);
            chargeBall.localScale = Vector3.one * size;

            if (chargeDone < 0)
            {
                chargeBall.localScale = Vector3.one * chargeSize;

                collapseDone = collapseRate;
                attackAnim.SetTrigger("Attack");

                chargeSound.Stop();

                if (playCollapse)
                {
                    collapesSound.Play();
                    playCollapse = false;
                }
            }
        }
        else if (collapseDone > 0)
        {
            collapseDone -= Time.deltaTime;

            if (!currentTarget)
            {
                chargeDone += 0.2f;
                nextAttk += 0.2f + collapseRate;
                collapseDone = collapseRate;
            }

            Vector3 attkPos = currentTarget.position;
            attkPos.y += attkHeight;
            attack.position = attkPos;

            float size = Mathf.Lerp(0, chargeSize, collapseDone / collapseRate);
            chargeBall.localScale = Vector3.one * size;

            if (collapseDone < 0)
            {
                chargeBall.localScale = Vector3.zero;
            }
        }
        
    }

    public float GetChargeRatio()
    {
        return chargeDone / chargeRate;
    }

    public void Attack()
    {
        if (currentTarget)
        {
            BasicHealth health = currentTarget.GetComponent<BasicHealth>();
            if (health)
            {
                if (health.GetHealth() <= 0)
                {
                    chargeDone += 0.2f;
                    nextAttk += 0.2f + collapseRate;
                    collapseDone = collapseRate;

                    return;
                }

                SoundManager.instance.PlayClip(hitSound, currentTarget.position, hitVolume);

                health.TakeDamage(damage, DamageType.energy);
            }
        }
        else
        {
            chargeDone += 0.2f;
            nextAttk += 0.2f + collapseRate;
            collapseDone = collapseRate;

            return;
        }

        playCollapse = true;

        attack.position = Vector3.down * 1000;
        attack.gameObject.SetActive(false);
    }
}
