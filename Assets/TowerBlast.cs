using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBlast : MonoBehaviour
{
    public float straightDistance;
    Vector3 startPoint;
    bool starting = true;

    public Transform target;
    Vector3 direction;
    Rigidbody rig;

    public float speed;
    public float startMod;
    public float damage;

    float missingTarget = 0;

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
        }
        else
        {
            missingTarget += Time.deltaTime;
            if (missingTarget > 5)
            {
                Destroy(this.gameObject);
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
        }
        rig.velocity = direction;
        print(rig.velocity + " " + rig.velocity.magnitude);
    }

    private void OnCollisionEnter(Collision collision)
    {
        BasicHealth enemy = collision.gameObject.GetComponent<BasicHealth>();
        if (enemy)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }

        if (!target)
        {
            Destroy(this.gameObject);
        }
    }
}
