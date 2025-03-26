using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningTower : MonoBehaviour
{
    public BasicTowerDetection detector;
    public Transform shootPoint;

    public float fireRate;
    float nextShot = 0;

    public int lightningDamage;
    public LayerMask lightningMask;
    public int jumpCount = 3;
    public float jumpMod = 1f;
    public float jumpRadius;
    public float jumpDelay = 0;

    public LightningDrawer lightningDrawer;

    Transform currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        nextShot -= Time.deltaTime;
        if (nextShot <= 0f)
        {
            currentTarget = detector.GetTarget();
            if (!currentTarget)
            {
                return;
            }

            BasicHealth enemy = currentTarget.GetComponent<BasicHealth>();
            EnemyBarrier barrier = enemy.Shock(lightningDamage, jumpMod, jumpCount, jumpRadius, lightningMask, lightningDrawer, jumpDelay);

            if (barrier)
            {
                Vector3 barrierPos = barrier.transform.position + (shootPoint.position - barrier.transform.position).normalized * barrier.transform.lossyScale.x / 2;
                lightningDrawer.Draw(shootPoint.position, barrierPos, jumpCount - 1, jumpDelay);
            }
            else
            {
                lightningDrawer.Draw(shootPoint.position, enemy.GetHitPosition(), jumpCount - 1, jumpDelay);
            }


            nextShot = fireRate;
        }
    }
}
