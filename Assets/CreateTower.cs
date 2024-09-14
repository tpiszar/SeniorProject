using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CreateTower : MonoBehaviour
{
    public Transform socket;

    public Transform miniReference;

    public GameObject towerPrefab;

    public float mapScale;

    GameObject tower;

    public float overlap = 0;

    // Start is called before the first frame update
    void Start()
    { 
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Create()
    {
        Vector3 relativeSpot = miniReference.InverseTransformPoint(transform.position) * mapScale;
        tower = Instantiate(towerPrefab, relativeSpot, Quaternion.identity);
    }

    private void OnDestroy()
    {
        if (tower)
        {
            Destroy(tower);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CreateTower tower = other.GetComponent<CreateTower>();
        if (tower)// && other.transform != transform)
        {
            overlap++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CreateTower tower = other.GetComponent<CreateTower>();
        if (tower)// && other.transform != transform)
        {
            overlap--;
        }
    }
}
