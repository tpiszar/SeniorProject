using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROneObjectSocket : XRSocketInteractor
{
    [SerializeField]
    public XRGrabInteractable singleObject;

#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (!singleObject)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            singleObject = args.interactable.GetComponent<XRGrabInteractable>();
#pragma warning restore CS0618 // Type or member is obsolete
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
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
}
