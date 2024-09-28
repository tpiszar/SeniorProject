using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableTwoAttach : XRGrabInteractable
{
    [SerializeField]
    public Transform attachLeft;
    [SerializeField]
    public Transform attachRight;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (attachLeft == attachRight)
        {
            base.OnSelectEntered(args);
            return;
        }

        if (args.interactorObject.transform.CompareTag("Left Hand"))
        {
            if (attachTransform == attachRight)
            {
                attachTransform = attachLeft;

                interactionManager.SelectExit(args.interactorObject, args.interactableObject);
                interactionManager.SelectEnter(args.interactorObject, args.interactableObject);
            }
        }
        else //if (args.interactorObject.transform.CompareTag("Right Hand"))
        {
            if (attachTransform == attachLeft)
            {
                attachTransform = attachRight;
                interactionManager.SelectExit(args.interactorObject, args.interactableObject);
                interactionManager.SelectEnter(args.interactorObject, args.interactableObject);
            }
        }

        base.OnSelectEntered(args);
    }
}
