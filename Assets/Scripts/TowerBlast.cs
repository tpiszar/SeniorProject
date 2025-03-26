using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBlast : MonoBehaviour
{
    public float straightDistance;
    Vector3 startPoint;
    bool starting = true;

    public BasicHealth target;
    public EnergyTower tower;
    Vector3 direction;
    Rigidbody rig;

    public float speed;
    public float startMod;
    public int damage;

    float missingTarget = 0;

    public bool redirect;
    public float smoothing;
    Vector3 refVel = Vector3.zero;

    public bool rotateTowards;

    public AudioSource boostSound;
    public AudioClip hitSound;
    [Range(0.0001f, 1f)]
    public float hitVolume = 1;

    public ParticleSystem particle;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        startPoint = transform.position;
        rig.velocity = Vector3.zero;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target)
        {
            direction = (target.GetHitPosition() - transform.position).normalized * speed;
            if (rotateTowards)
            {
                transform.LookAt(target.GetHitPosition());
            }
        }
        else
        {
            if (aHit)
            {
                aHit.TakeDamage(damage, DamageType.energy);
                HitDestroy();
                return;
            }
            if (redirect)
            {
                target = tower.detector.GetTarget().GetComponent<BasicHealth>();
                smoothing /= 2;

                if (!target)
                {
                    redirect = false;
                }
            }
            else
            {
                missingTarget += Time.deltaTime;
                if (starting)
                {
                    starting = false;
                    direction *= 1 / startMod;
                }

                if (missingTarget > 5)
                {
                    HitDestroy();
                }
            }
        }
        if (starting)
        {
            float distTraveled = (transform.position - startPoint).magnitude;
            if (distTraveled > straightDistance)
            {
                if (boostSound)
                {
                    boostSound.Play();
                }

                starting = false;
            }
            else
            {
                direction.y = 0f;
                direction.Normalize();
                direction *= speed * startMod;
            }
            rig.velocity = direction;
        }
        else
        {
            rig.velocity = Vector3.SmoothDamp(rig.velocity, direction, ref refVel, smoothing);
        }
    }

    bool hit = false;
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (hit)
    //    {
    //        return;
    //    }

    //    print(collision.transform.name + " " + Time.time);
    //    if (collision.transform.CompareTag("Enemy"))
    //    {
    //        BasicHealth enemy = collision.gameObject.GetComponentInParent<BasicHealth>();
    //        if (enemy && (!target || enemy.transform == target))
    //        {
    //            enemy.TakeDamage(damage, DamageType.energy);
    //            hit = true;
    //            Destroy(gameObject);
    //        }

    //        if (enemy)
    //        {
    //            print(enemy.transform.name + " " + Time.time);
    //        }
    //    }
        

    //    //StopAllCoroutines();
    //    //Destroy(this.gameObject);

    //    //if (!target)
    //    //{
    //    //    Destroy(this.gameObject);
    //    //}
    //}

    void HitDestroy()
    {
        if (particle)
        {
            particle.transform.parent = null;
            particle.Stop();
            Destroy(particle, 2);
        }

        Destroy(gameObject);
    }

    BasicHealth aHit;
    private void OnTriggerEnter(Collider other)
    {
        if (hit)
        {
            return;
        }

        if (other.transform.CompareTag("Enemy"))
        {
            BasicHealth enemy = other.gameObject.GetComponentInParent<BasicHealth>();
            if (enemy)
            {
                EnemyBarrier barrier = other.GetComponent<EnemyBarrier>();
                if (!target || enemy == target || barrier)
                {
                    enemy.TakeDamage(damage, DamageType.energy);
                    hit = true;

                    SoundManager.instance.PlayClip(hitSound, transform.position, hitVolume);

                    HitDestroy();
                }
                else
                {
                    aHit = enemy;
                }
            }
            else
            {
                if (aHit)
                {
                    aHit.TakeDamage(damage, DamageType.energy);
                }
                HitDestroy();
            }
        }
    }

    //IEnumerator Damage()
    //{

    //}
}
