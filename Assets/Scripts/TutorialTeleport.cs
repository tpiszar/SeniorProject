using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTeleport : MonoBehaviour
{
    public int amount = 2;
    int count = 0;

    public TutorialManager manager;

    // Start is called before the first frame update
    void Start()
    {
        Teleport.Instance.onTeleport += OnTeleport;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTeleport(float teleportTime)
    {
        count++;
        if (count == amount)
        {
            manager.NextTutorial();
        }
    }
}
