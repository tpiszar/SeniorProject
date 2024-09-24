using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHit : MonoBehaviour
{
    public EnemyAI enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void doHit()
    {
        enemy.Hit();
    }
}
