using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFlame : MonoBehaviour
{
    public int damage;
    public float burnDuration;
    public float burnRate;
    public int tickDamage;

    public List<Transform> hitThisCycle = new List<Transform>();

    public int soundCap = 15;
    public int soundCount = 0;

    public AudioClip burnSound;
    [Range(0.0001f, 1f)]
    public float burnVolume = 1;

    public GameObject hitParticle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

            BasicHealth enemy = other.gameObject.GetComponentInParent<BasicHealth>();
            if (enemy)
            {
                if (hitThisCycle.Contains(enemy.transform))
                {
                    return;
                }

                if (soundCount < soundCap)
                {
                    //SoundManager.instance.PlayClip(burnSound, enemy.transform.position, burnVolume);
                    soundCount++;
                }


                EnemyBarrier barrier = enemy.TakeDamage(damage, DamageType.fire);

                if (!barrier)
                {
                    enemy.Burn(burnDuration, burnRate, tickDamage);
                    Instantiate(hitParticle, enemy.GetHitPosition(), Quaternion.identity);
                }

                hitThisCycle.Add(enemy.transform);


            }
        }

    }
}
