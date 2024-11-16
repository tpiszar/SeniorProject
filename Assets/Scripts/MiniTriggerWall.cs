using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniTriggerWall : MonoBehaviour
{
    public bool trackerOnly = false;

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
        if (other.CompareTag("Root"))
        {
            EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
            if (enemy)
            {
                MiniMapTracker.instance.AddMapTracker(enemy.transform, enemy.type);

                if (!trackerOnly)
                {
                    WaveManager.Instance.enemies.Add(enemy);
                    WaveManager.Instance.healths.Add(enemy.GetComponent<BasicHealth>());
                }
            }
        }
    }
}
