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
    public float destoryDelay = 0.2f;
    int miniNum;

    Material hoverMat;

    public GameObject rangeSphere;

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
        if (slotted && destoryDelay > 0)
        {
            destoryDelay -= Time.deltaTime;
        }
    }

    public void HoverExit(HoverExitEventArgs args)
    {

        if (socket.singleObject != args.interactable.gameObject)
        {
            return;
        }

        if (slotted)
        {
            if (destoryDelay < 0)
            {
                Destroy(mini.gameObject);
                Destroy(gameObject);
            }
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

            Transform attach = transform.GetChild(0);
            Vector3 placePos = attach.position;
            attach.localPosition = Vector3.zero;
            transform.position = placePos;


            socket.manaRequirement = -1;

            if (rangeSphere)
            {
                Destroy(rangeSphere);
            }

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
