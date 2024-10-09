using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROneObjectSocket : XRSocketInteractor
{
    [SerializeField]
    public GameObject singleObject;

    //protected override void OnHoverEntered(HoverEnterEventArgs args)
    //{
    //    if (!singleObject)
    //    {
    //        singleObject = args.interactable.gameObject;
    //    }
    //    if (args.interactable.gameObject == singleObject)
    //    {
    //        base.OnHoverEntered(args);
    //    }

    //    if (args.interactable.gameObject != singleObject)
    //    {
    //        //interactionManager.CancelInteractableHover(args.interactableObject);
    //        //interactionManager.HoverExit(this, args.interactableObject);
    //    }
    //    else
    //    {
    //        base.OnHoverEntered(args);
    //    }
    //}

    //protected override void OnHoverExited(HoverExitEventArgs args)
    //{
    //    if (args.interactable.gameObject == singleObject)
    //    {
    //        base.OnHoverExited(args);
    //    }
    //}

    //protected override void OnSelectExited(SelectExitEventArgs args)
    //{
    //    showInteractableHoverMeshes = true;
    //    if (args.interactable.gameObject == singleObject)
    //    {
    //        base.OnSelectExited(args);
    //    }
    //}

    //protected override void OnSelectEntered(SelectEnterEventArgs args)
    //{
    //    showInteractableHoverMeshes = false;

    //    if (args.interactable.gameObject == singleObject)
    //    {
    //        base.OnSelectEntered(args);
    //    }
    //}

    public override bool CanHover(XRBaseInteractable interactable)
    {
        if (!singleObject)
        {
            singleObject = interactable.gameObject;
        }

        return interactable.gameObject == singleObject && base.CanHover(interactable);
    }

    public override bool CanSelect(XRBaseInteractable interactable)
    {
        return interactable.gameObject == singleObject && base.CanSelect(interactable);
    }
}
