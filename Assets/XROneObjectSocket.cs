using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROneObjectSocket : XRSocketInteractor
{
    [SerializeField]
    public XRGrabInteractable singleObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
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
        if (args.interactableObject == singleObject)
        {
            base.OnSelectExited(args);
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactableObject == singleObject)
        {
            base.OnSelectEntered(args);
        }
    }
}
