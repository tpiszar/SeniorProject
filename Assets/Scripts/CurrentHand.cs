using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CurrentHand : MonoBehaviour
{
    public XRNode inputSource;
    public XRBaseController controller;
    public bool noHand = true;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.selectEntered.AddListener(Grabbed);
    }

    public void Grabbed(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("Right Hand"))
        {
            inputSource = XRNode.RightHand;
            noHand = false;
        }
        else if (args.interactorObject.transform.CompareTag("Left Hand"))
        {
            inputSource = XRNode.LeftHand;
            noHand = false;
        }
        else
        {
            noHand = true;
        }
        controller = args.interactor.GetComponent<XRBaseController>();
    }
}
