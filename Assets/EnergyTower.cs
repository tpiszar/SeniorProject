using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyTower : MonoBehaviour
{
    public BasicTowerDetection detector;
    public GameObject energyShot;
    public Transform shootPoint;
    public float fireRate;
    float nextShot = 0;

    public Transform currentTarget;

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

            GameObject newShot = Instantiate(energyShot, shootPoint.position, Quaternion.identity);
            newShot.GetComponent<TowerBlast>().target = currentTarget;
            print(currentTarget.name + " GIVEN");
            print(newShot.GetComponent<TowerBlast>().target.name + " RECIEVED");

            nextShot = fireRate;
        }
    }
}
