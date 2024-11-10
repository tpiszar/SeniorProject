using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicOrbTower : MonoBehaviour
{
    public float dps;
    float damageRamp = 0f;



    public float chargeTime;
    float chargeLevel = 0;

    public float checkRate = 0.5f;
    float nextCheck = 0;

    public BasicTowerDetection detector;
    public Transform shootPoint;

    Transform currentTarget;
    EnemyAI currentEnemy;
    BasicHealth currentHealth;

    public LineRenderer laser;

    public Transform top;
    public float rotationMin;
    public float rotationMax;

    public AudioSource chargeSound;
    public AudioSource laserSound;

    // Start is called before the first frame update
    void Start()
    {
        laser.SetPosition(0, shootPoint.position);
    }

    // Update is called once per frame
    void Update()
    {

        if (!currentTarget)
        {
            laser.SetPosition(1, shootPoint.position);

            laserSound.Stop();
            chargeSound.Stop();
            
            if (chargeLevel > 0)
            {
                chargeLevel -= Time.deltaTime;
            }
            else
            {
                chargeLevel = 0;
            }

            nextCheck += Time.deltaTime;
            if (nextCheck > checkRate)
            {
                currentTarget = detector.GetTarget();
                if (!currentTarget)
                {
                    nextCheck = 0;
                }
                else
                {
                    currentHealth = currentTarget.GetComponent<BasicHealth>();
                    currentEnemy = currentTarget.GetComponent<EnemyAI>();
                }
            }
        }
        if (currentTarget)
        {
            if (chargeLevel < chargeTime)
            {
                if (!chargeSound.isPlaying)
                {
                    chargeSound.Play();
                }

                chargeLevel += Time.deltaTime;
            }
            else
            {
                chargeLevel = chargeTime;

                damageRamp += Time.deltaTime * dps;
                int dmg = (int)damageRamp;
                damageRamp -= dmg;

                if (!laserSound.isPlaying)
                {
                    laserSound.Play();
                }

                EnemyBarrier barrier = currentHealth.TakeDamage(dmg, DamageType.energy);

                if (barrier)
                {
                    Vector3 barrierPos = barrier.transform.position + (shootPoint.position - barrier.transform.position).normalized * barrier.transform.lossyScale.x / 2;
                    laser.SetPosition(1, barrierPos);
                }
                else
                {
                    laser.SetPosition(1, currentTarget.position);
                }

            }

            if (!detector.enemies.Contains(currentEnemy))
            {
                currentTarget = null;
            }
        }

        float rotationSpeed = Mathf.Lerp(rotationMin, rotationMax, chargeLevel / chargeTime);

        top.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
