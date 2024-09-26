using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROneObjectSocket : XRSocketInteractor
{
    [SerializeField]
    public XRGrabInteractable singleObject;

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (!singleObject)
        {
            singleObject = args.interactable.GetComponent<XRGrabInteractable>();
        }
        if (args.interactableObject == singleObject)
        {
            base.OnHoverEntered(args);
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        if (args.interactableObject == singleObject)
        {
            base.OnHoverExited(args);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        showInteractableHoverMeshes = true;
        if (args.interactableObject == singleObject)
        {
            base.OnSelectExited(args);
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        showInteractableHoverMeshes = false;
        if (args.interactableObject == singleObject)
        {
            base.OnSelectEntered(args);
        }
    }
}
