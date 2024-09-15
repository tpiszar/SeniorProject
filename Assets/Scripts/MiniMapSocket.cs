using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MiniMapSocket : MonoBehaviour
{
    public GameObject mini;
    XRSocketInteractor socket;
    public Collider mapCollider;
    bool slotted = false;
    bool justSlotted = true;
    int miniNum;

    // Start is called before the first frame update
    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.hoverExited.AddListener(HoverExit);
        socket.selectEntered.AddListener(Release);
        socket.selectExited.AddListener(Stolen);
        miniNum = Respawns.instance.getObjNum(transform.parent.gameObject);
        if (transform.parent)
        {
            transform.parent = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HoverExit(HoverExitEventArgs args)
    {
        if (slotted)
        {
            Destroy(mini.gameObject);
            Destroy(gameObject);
        }
        else
        {
            transform.position = Vector3.down * 1000;
        }
    }

    public void Release(SelectEnterEventArgs args)
    {
        if (!slotted)
        {
            mapCollider.enabled = false;
            mini.GetComponent<DropReturn>().enabled = false;
            socket.interactableHoverMeshMaterial = socket.interactableCantHoverMeshMaterial;
            slotted = true;

            mini.GetComponent<CreateTower>().Create();

            if (miniNum >= 0)
            {
                Respawns.instance.earlyRespawn(0, miniNum);
            }
        }
    }

    public void Stolen(SelectExitEventArgs args)
    {
        
    }
}
