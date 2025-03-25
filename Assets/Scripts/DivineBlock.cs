using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class DivineBlock : MonoBehaviour
{
    public bool left;

    public int hitDamage;

    public float activateVelocity;

    public ShockWave shockWave;

    public Collider blockCollider;
    public Collider barrierTrigger;
    public Barrier barrier;
    public GameObject obstacle;

    public ParticleSystem shockWaveParticle;

    public Transform miniBlock;
    public Transform miniMap;
    public float shrinkScale = 0.03f;

    public float burySize = 0.25f;

    List<BasicHealth> hitEnemies = new List<BasicHealth>();
    Dictionary<Transform, int> hitColliders = new Dictionary<Transform, int>();

    float lastY;
    float yVelocity;

    public AudioClip slamSound;
    [Range(0.0001f, 1f)]
    public float slamVolume = 1;

    void Start()
    {
        lastY = transform.position.y;
        transform.parent = null;
    }

    void Update()
    {
        yVelocity = (transform.position.y - lastY) / Time.deltaTime;
        lastY = transform.position.y;
    }

    void Slam()
    {
        print("SLAM");

        if (left)
        {
            DivineHands.leftDivine = true;
        }
        else
        {
            DivineHands.rightDivine = true;
        }

        Vector3 snapPoint = Vector3.zero;

        float angle = Vector3.Angle(transform.up, Vector3.up);

        Vector3 modifiedVector;

        bool up = true;
        float y = transform.rotation.eulerAngles.y;

        if (angle < 45f)
        {
            modifiedVector = Vector3.Project(transform.up, Vector3.up);
        }
        else
        {
            modifiedVector = Vector3.ProjectOnPlane(transform.up, Vector3.up).normalized;

            up = false;
        }

        modifiedVector = Quaternion.FromToRotation(transform.up, modifiedVector) * transform.up;

        transform.up = modifiedVector;

        float heightMod = -burySize;

        Vector3 rot = transform.rotation.eulerAngles;
        if (up)
        {
            rot.y = y;
            transform.rotation = Quaternion.Euler(rot);

            heightMod += transform.lossyScale.y / 2;
        }
        else
        {
            rot.x = 0;
            transform.rotation = Quaternion.Euler(rot);

            heightMod += transform.lossyScale.x / 2;
        }

        //Quaternion angledPath;
        snapPoint = MapPath.Instance.GetClosestPoint(transform.position);//, out angledPath);
        //if (!up)
        //{
        //    transform.rotation = angledPath;
        //}
        snapPoint.y += heightMod;
        transform.position = snapPoint;



        foreach (BasicHealth hit in hitEnemies)
        {
            if (hit)
            {
                hit.TakeDamage(hitDamage, DamageType.energy);
            }
        }

        blockCollider.isTrigger = false;

        shockWave.Slam();
        Destroy(shockWave.gameObject);
        miniBlock.GetComponent<XRGrabInteractable>().enabled = false;
        Destroy(GetComponent<Rigidbody>());

        barrierTrigger.enabled = true;
        barrier.enabled = true;
        obstacle.SetActive(true);
        miniBlock.parent = miniMap;
        miniBlock.localPosition = transform.position * shrinkScale;
        miniBlock.localRotation = transform.rotation;

        shockWaveParticle.transform.parent = null;
        snapPoint.y -= heightMod - 0.5f;
        shockWaveParticle.transform.position = snapPoint;
        shockWaveParticle.transform.rotation = Quaternion.identity;
        shockWaveParticle.Play();

        SoundManager.instance.PlayClip(slamSound, transform.position, slamVolume);

        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            BasicHealth enemy = other.GetComponentInParent<BasicHealth>();

            if (hitColliders.ContainsKey(enemy.transform))
            {
                hitColliders[enemy.transform]++;
            }
            else
            {
                hitColliders[enemy.transform] = 1;
                hitEnemies.Add(enemy);
            }
        }
    }

    bool slammed = false;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Path") && yVelocity < -50 && !slammed)
        {
            slammed = true;
            Slam();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            BasicHealth enemy = other.GetComponentInParent<BasicHealth>();
            if (hitColliders.ContainsKey(enemy.transform))
            {
                hitColliders[enemy.transform]--;

                // If no more colliders are inside the laser area, remove the enemy
                if (hitColliders[enemy.transform] == 0)
                {
                    hitColliders.Remove(enemy.transform);
                    hitEnemies.Remove(enemy);
                }
            }
        }
    }
}
