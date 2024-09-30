using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WandRayDynamic : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        XRRayInteractor interactor = GetComponent<XRRayInteractor>();
        interactor.hoverEntered.AddListener(makeGrabDynamic);
        interactor.hoverExited.AddListener(endGrabDynamic);
        interactor.selectExited.AddListener(endGrabDynamic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public void makeGrabDynamic(HoverEnterEventArgs args)
    {
        args.interactable.GetComponent<XRGrabInteractable>().useDynamicAttach = true;
    }

    public void endGrabDynamic(HoverExitEventArgs args)
    {
        args.interactable.GetComponent<XRGrabInteractable>().useDynamicAttach = false;
    }

    public void endGrabDynamic(SelectExitEventArgs args)
    {
        args.interactable.GetComponent<XRGrabInteractable>().useDynamicAttach = false;
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
