using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTowerDetection : TowerDetection
{
    WaveManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = WaveManager.Instance;
    }

    public override Transform GetTarget()
    {
        return manager.GetTarget(detectionType);
    }
}
