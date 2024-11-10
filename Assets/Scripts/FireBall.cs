using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public int damage = 10;

    public Transform flameBlast;
    TowerFlame flame;
    public float expandRate;
    float blastRadius;

    public GameObject mesh;

    public AudioClip hitSound;
    [Range(0.0001f, 1f)]
    public float hitVolume = 1;

    // Start is called before the first frame update
    void Start()
    {
        blastRadius = flameBlast.localScale.x;
        flameBlast.localScale = Vector3.zero;

        flame = flameBlast.GetComponent<TowerFlame>();
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    bool hit = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (hit)
        {
            return;
        }

        hit = true;
        if (collision.transform.CompareTag("Enemy"))
        {
            BasicHealth enemy = collision.gameObject.GetComponentInParent<BasicHealth>();
            if (enemy)
            {
                enemy.TakeDamage(damage, DamageType.fire);
            }
        }

        SoundManager.instance.PlayClip(hitSound, transform.position, hitVolume);

        StartCoroutine(ExpandFlame());

        mesh.SetActive(false);
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    IEnumerator ExpandFlame()
    {
        flame.hitThisCycle.Clear();
        float timer = 0;
        while (timer < expandRate)
        {
            timer += Time.deltaTime;

            flameBlast.localScale = Vector3.one * Mathf.Lerp(0, blastRadius, timer / expandRate);

            yield return null;
        }
        flameBlast.localScale = Vector3.zero;

        Destroy(gameObject);
    }
}
