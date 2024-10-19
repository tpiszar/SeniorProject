using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsychicAttack : MonoBehaviour
{
    public PsychicTower tower;

    public float rotationMin;
    public float rotationMax;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }

    // Update is called once per frame
    void Update()
    {
        float rotationSpeed = Mathf.Lerp(rotationMax, rotationMin, tower.GetChargeRatio());

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    public void Damage()
    {
        tower.Attack();
    }
}
