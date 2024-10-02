using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public int damage = 10;

    public Transform flameBlast;
    public float expandRate;
    float blastRadius;

    public GameObject mesh;

    // Start is called before the first frame update
    void Start()
    {
        blastRadius = flameBlast.localScale.x;
        flameBlast.localScale = Vector3.zero;
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
        BasicHealth enemy = collision.gameObject.GetComponent<BasicHealth>();
        if (enemy)
        {
            enemy.TakeDamage(damage);
        }

        StartCoroutine(ExpandFlame());

        mesh.SetActive(false);
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    IEnumerator ExpandFlame()
    {
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
