using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTutorial : Tutorial
{
    bool away = false;

    public int startBase = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (Teleport.Instance)
        {
            Teleport.Instance.onTeleport += OnTeleport;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTeleport(float teleportTime)
    {
        if (away && Teleport.Instance.curBase == startBase)
        {
            Complete();
        }
        away = true;
    }

    public override void Complete()
    {
        if (Teleport.Instance.curBase != startBase)
        {
            Teleport.Instance.startTeleport(startBase);
        }

        base.Complete();

        Destroy(this);
    }

    private void OnDestroy()
    {
        if (Teleport.Instance)
        {
            Teleport.Instance.onTeleport -= OnTeleport;
        }
    }
}
