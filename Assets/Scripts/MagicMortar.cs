using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMortar : MonoBehaviour
{
    public GlobalTowerDetection detector;

    public GameObject blastPrefab;

    public Transform shootPoint;
    public float fireRate;
    float nextShot = 0;

    Transform currentTarget;

    public Transform barrel;
    public Transform dropPoint;
    public float dropTime;
    float dropping = 0;

    MortarBlast currentBlast;

    public AudioSource fireSound;

    // Start is called before the first frame update
    void Start()
    {
        nextShot = fireRate;
    }

    // Update is called once per frame
    void Update()
    {

        if (dropping > 0)
        {
            dropping -= Time.deltaTime;

            barrel.position = Vector3.Lerp(dropPoint.position, shootPoint.position, dropping / dropTime);

            if (dropping < 0)
            {
                currentBlast.gameObject.SetActive(true);
            }
        }
        else
        {
            barrel.position = Vector3.Lerp(shootPoint.position, dropPoint.position, nextShot / fireRate);
        }

        nextShot -= Time.deltaTime;
        if (nextShot <= 0f)
        {
            currentTarget = detector.GetTarget();
            if (!currentTarget)
            {
                return;
            }

            fireSound.Play();

            dropping = dropTime;

            currentBlast = Instantiate(blastPrefab, shootPoint.position, Quaternion.identity).GetComponent<MortarBlast>();
            currentBlast.gameObject.SetActive(false);
            currentBlast.target = currentTarget;
            currentBlast.targetPoint = currentTarget.position;

            nextShot = fireRate;
        }
    }
}
