using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MiniMapSocket : MonoBehaviour
{
    public GameObject mini;
    XROneObjectSocket socket;
    public Collider mapCollider;
    bool slotted = false;
    int miniNum;

    Material hoverMat;

    // Start is called before the first frame update
    void Start()
    {
        socket = GetComponent<XROneObjectSocket>();
        socket.hoverExited.AddListener(HoverExit);
        socket.selectEntered.AddListener(Release);
        socket.selectExited.AddListener(Stolen);
        miniNum = Respawns.instance.getObjNum(transform.parent.gameObject);
        if (transform.parent)
        {
            transform.parent = null;
        }

        hoverMat = socket.interactableHoverMeshMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HoverExit(HoverExitEventArgs args)
    {
        
        if (socket.singleObject != args.interactable.gameObject)
        {
            return;
        }

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
        if (socket.singleObject != args.interactable.gameObject)
        {
            return;
        }

        if (!slotted)
        {
            mapCollider.enabled = false;
            mini.GetComponent<DropReturn>().enabled = false;
            socket.interactableHoverMeshMaterial = socket.interactableCantHoverMeshMaterial;
            slotted = true;

            //POSSIBLY MOVE FOR HIGHER UP DROP
            //transform.position = transform.GetChild(0).position;
            //transform.GetChild(0).position = transform.position;

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
