using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerDeath : MonoBehaviour
{
    public WaveManager manager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        foreach (Transform enemy in manager.enemies)
        {
            enemy.GetComponent<EnemyAI>().enabled = false;
            enemy.GetComponent<NavMeshAgent>().enabled = false;
        }
        if (manager)
        {
            manager.gameObject.SetActive(false);
        }
        Time.timeScale = 0;
    }
}
