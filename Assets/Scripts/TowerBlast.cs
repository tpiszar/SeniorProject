using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBlast : MonoBehaviour
{
    public float straightDistance;
    Vector3 startPoint;
    bool starting = true;

    public Transform target;
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
            direction = (target.position - transform.position).normalized * speed;
            if (rotateTowards)
            {
                transform.LookAt(target.position);
            }
        }
        else
        {
            if (redirect)
            {
                target = tower.detector.GetTarget();
                smoothing /= 2;

                if (!target)
                {
                    redirect = false;
                    print("FAILED REDIRECT");
                }
                else
                {
                    print("REDIRECT");
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
                    Destroy(this.gameObject);
                }
            }
        }
        if (starting)
        {
            float distTraveled = (transform.position - startPoint).magnitude;
            if (distTraveled > straightDistance)
            {
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

    private void OnCollisionEnter(Collision collision)
    {
        BasicHealth enemy = collision.gameObject.GetComponent<BasicHealth>();
        if (enemy)
        {
            enemy.TakeDamage(damage);
            //Destroy(gameObject);
        }

        Destroy(this.gameObject);
        //if (!target)
        //{
        //    Destroy(this.gameObject);
        //}
    }
}
